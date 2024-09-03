using UnityEngine;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SFH {
    public static class RTUtils {
        static RenderTexture previousRt;

        #region Rendering RT
        public static void BeginOrthographicRendering(this RenderTexture rt, float orthoSize, in Vector3 position, in Quaternion rotation, float zBegin = -100, float zEnd = 100) {
            float aspect = (float)rt.width / rt.height;
            Matrix4x4 projectionMatrix = Matrix4x4.Ortho(-orthoSize * aspect, orthoSize * aspect, -orthoSize, orthoSize, zBegin, zEnd);
            Matrix4x4 viewMatrix = Matrix4x4.TRS(position, rotation, new Vector3(1, 1, -1));
            Matrix4x4 cameraMatrix = projectionMatrix * viewMatrix.inverse;

            rt.BeginRendering(cameraMatrix);
        }
        public static void BeginPerspectiveRendering(this RenderTexture rt, float fov, in Vector3 position, in Quaternion rotation, float zBegin = -100, float zEnd = 100) {
            float aspect = (float)rt.width / rt.height;
            Matrix4x4 projectionMatrix = Matrix4x4.Perspective(fov, aspect, zBegin, zEnd);
            Matrix4x4 viewMatrix = Matrix4x4.TRS(position, rotation, new Vector3(1, 1, -1));
            Matrix4x4 cameraMatrix = projectionMatrix * viewMatrix.inverse;
            rt.BeginRendering(cameraMatrix);
        }

        public static void BeginRendering(this RenderTexture rt, Matrix4x4 projectionMatrix) {
            if (Camera.current != null) projectionMatrix *= Camera.current.worldToCameraMatrix.inverse;
            previousRt = RenderTexture.active;
            RenderTexture.active = rt;
            GL.PushMatrix();
            GL.LoadProjectionMatrix(projectionMatrix);
        }

        public static void EndRendering(this RenderTexture rt) {
            GL.PopMatrix();
            GL.invertCulling = false;
            RenderTexture.active = previousRt;
        }

        public static void DrawMesh(this RenderTexture rt, Mesh mesh, Material material, in Matrix4x4 objectMatrix, int pass = 0) {
            bool canRender = material.SetPass(0);
            if (canRender) {
                Graphics.DrawMeshNow(mesh, objectMatrix);
            }
        }

        public static void DrawRenderTexture(RenderTexture rt, Mesh quadMesh, Matrix4x4 textureMatrix, Material material, Camera camera) {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetTexture("_MainTex", rt);
            Graphics.DrawMesh(quadMesh, textureMatrix, material, 0, camera, 0, mpb);
        }

        #endregion

        public static Texture2D ToTexture2D(this RenderTexture renderTexture) {
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            return texture;
        }

        public static RenderTexture CreateSimpleTexture(string name, Vector2Int dimensions, GraphicsFormat format) {
            RenderTexture texture = new RenderTexture(dimensions.x,dimensions.y, 0, format, 0);
            texture.name = name;
            texture.autoGenerateMips = false;
            texture.enableRandomWrite = true;
            texture.Create();
            return texture;
        }
#if UNITY_EDITOR
        public static void SaveRTToFile(RenderTexture renderTexture, string path = "", bool printDirectoty = false) {
            //Create Texture2D
            RenderTexture previousTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            RenderTexture.active = previousTexture;
            //Save file
            byte[] bytes;
            bytes = tex.EncodeToPNG();
            if (path.IsNullOrEmpty()) {
                path = AssetDatabase.GetAssetPath(renderTexture) + ".png";
            }
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path);
            if (printDirectoty) Debug.Log("Saved to " + path);
        }
#endif
    }
}