#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace SFH {

    public class SpriteGenerator : EditorWindow {
        [MenuItem("Tools/SpriteGen")]
        public static SpriteGenerator OpenWindow() {
            SpriteGenerator sg = GetWindow<SpriteGenerator>();
            sg.minSize = new Vector2(blockMinWidths.Sum(), 200);
            sg.previewMaterial = Object.Instantiate(Resources.Load<Material>("PreviewMaterial"));
            sg.jfaGenerator = new JFAGenerator();
            sg.sdfGenerator = new SDFGenerator();
            sg.Show();
            return sg;
        }

        public void SetSpriteAsset(Texture2D sprite) {
            spriteAsset = sprite;
            string assetPath = AssetDatabase.GetAssetPath(sprite);
            spriteImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        }

        private const string sdfPath = "Resources/SDFs/";
        private Rect[] rectangularBlocks = new Rect[2];
        private static float[] blockWidths = { 300f, 900f };
        private static float[] blockMinWidths = { 200f, 400f };

        private Rect dropZoneRect;
        public Material previewMaterial;

        //Asset
        private Texture2D spriteAsset;
        private TextureImporter spriteImporter;
        //Modules
        JFAGenerator jfaGenerator;
        SDFGenerator sdfGenerator;
        //Generator settings
        private float valueThreshold = 0.5f;
        private int borderThickness = 0;
        private bool useCompression = true;

        private void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            float availableWidth = EditorGUIUtility.currentViewWidth;
            for (int i = 0; i < rectangularBlocks.Length; i++) {
                float blockWidth = Mathf.Max(availableWidth * blockWidths[i] / Mathf.Max(blockWidths), blockMinWidths[i]);
                CreateBlockOfIndex(i, blockWidth);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateBlockOfIndex(int index, float width) {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.MinWidth(blockMinWidths[index]), GUILayout.ExpandHeight(true));
            switch (index + 1) {
                case 1:
                    CreateFirstBlock(width);
                    break;
                case 2:
                    GUILayout.Label($"Block {index + 1}");
                    break;
            }
            EditorGUILayout.EndVertical();
        }

        private void CreateFirstBlock(float width) {
            //Drag and drop zone
            dropZoneRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100));
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.gray;
            GUI.Box(dropZoneRect, GUIContent.none);
            GUI.backgroundColor = originalColor;
            //Label
            float labelWidth = width - 20f;
            string spriteName = spriteAsset != null ? spriteAsset.name : "Drag and Drop Sprite Here";
            Rect labelRect = GUILayoutUtility.GetLastRect();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(labelRect, spriteName, labelStyle);
            //Preview
            if (spriteAsset != null) {
                float previewWidth = 100f;
                float previewHeight = 100f;
                Rect previewRect = new Rect(dropZoneRect.x + dropZoneRect.width - previewWidth, dropZoneRect.y, previewWidth, previewHeight);
                EditorGUI.DrawPreviewTexture(previewRect, spriteAsset, previewMaterial);
            }
            //Float values
            valueThreshold = EditorGUILayout.FloatField("Value threshold:", valueThreshold, GUILayout.Width(width));
            valueThreshold = Mathf.Clamp(valueThreshold, 0, 1);
            borderThickness = EditorGUILayout.IntField("Border thickness:", borderThickness, GUILayout.Width(width));
            borderThickness = Mathf.Max(0, borderThickness);
            useCompression = EditorGUILayout.Toggle("Use compression:", useCompression, GUILayout.Width(width));
            EditorGUILayout.Space(10f);
            //Applies required settings, renders SDF and adds secondary textures
            if (GUILayout.Button("Apply")) {
                TextureImporterSettings settings = new TextureImporterSettings();
                spriteImporter.ReadTextureSettings(settings);
                //Set full rect
                settings.spriteMeshType = SpriteMeshType.FullRect;
                //Render SDF
                Texture floodTexture = jfaGenerator.GenerateFloodTexture(spriteAsset,valueThreshold, borderThickness);
                Texture2D sdfTexture = sdfGenerator.GenerateSDFTexture(floodTexture).ToTexture2D();
                //Save texture to disk
                Texture2D savedSDF = SaveTextureToFile(sdfTexture,sdfPath + spriteAsset.name + ".png");
                spriteImporter.secondarySpriteTextures = CreateSecondarySpriteTexturesArray(savedSDF);
                //Save settings
                spriteImporter.SetTextureSettings(settings);
                spriteImporter.SaveAndReimport();
                //Release generators
                RenderTexture.active = null;
                jfaGenerator.Release();
                sdfGenerator.Release();
            }
            HandleDragAndDrop();
        }

        private void HandleDragAndDrop() {
            Event currentEvent = Event.current;
            switch (currentEvent.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropZoneRect.Contains(currentEvent.mousePosition))
                        return;
                    if (DragAndDrop.objectReferences.Length > 0) {
                        Object draggedObject = DragAndDrop.objectReferences[0];
                        Texture2D sprite = draggedObject as Texture2D;

                        if (sprite != null) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (currentEvent.type == EventType.DragPerform) {
                                DragAndDrop.AcceptDrag();
                                SetSpriteAsset(sprite);
                            }
                        }
                    }
                    currentEvent.Use();
                    break;
            }
        }

        private SecondarySpriteTexture[] CreateSecondarySpriteTexturesArray(Texture2D savedTexture) {
            SecondarySpriteTexture sdfSST = new SecondarySpriteTexture(){
                name = "_SDF",
                texture = savedTexture
            };
            //Set secondary textures
            SecondarySpriteTexture[] ssts = spriteImporter.secondarySpriteTextures;
            return ssts.Where(i => i.name != "_SDF").Concat(new[] { sdfSST }).ToArray();
        }

        private Texture2D SaveTextureToFile(Texture2D sourceTexture, string path = "", bool printDirectoty = false) {
            //Create Texture2D
            Texture2D tex = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, sourceTexture.width, sourceTexture.height), 0, 0);
            //Save file
            byte[] bytes;
            bytes = tex.EncodeToPNG();
            if (path.IsNullOrEmpty()) {
                path = AssetDatabase.GetAssetPath(sourceTexture) + ".png";
            }
            //Create directory
            string absolutePath = Application.dataPath+"/"+path;
            System.IO.FileInfo file = new System.IO.FileInfo(absolutePath);
            file.Directory.Create();
            //Save to disk and load asset
            System.IO.File.WriteAllBytes(absolutePath, bytes);
            AssetDatabase.ImportAsset("Assets/" + path);
            if (printDirectoty) Debug.Log("Saved to " + path);
            //Apply importer settings
            TextureImporter saveTextureImporter = AssetImporter.GetAtPath("Assets/" + path) as TextureImporter;

            TextureImporterPlatformSettings tips = saveTextureImporter.GetDefaultPlatformTextureSettings();
            tips.textureCompression = useCompression ? TextureImporterCompression.CompressedHQ : TextureImporterCompression.Uncompressed;
            saveTextureImporter.SetPlatformTextureSettings(tips);
            saveTextureImporter.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/" + path);
        }
    }
}
#endif