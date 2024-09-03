using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace SFH {
    public class SDFGenerator {
        private const int threadGroupWidth = 8;

        private ComputeShader computeShader;
        private RenderTexture sdfTexture;
        private int SDFKernel;
        private Vector2Int groupThreadCount;
        public SDFGenerator() {
            computeShader = Object.Instantiate(Resources.Load<ComputeShader>(nameof(SDFGenerator)));
            computeShader.hideFlags = HideFlags.HideAndDontSave;
            SDFKernel = computeShader.FindKernel(nameof(SDFKernel));
        }

        public RenderTexture GetSDFTexture() => sdfTexture;

        public RenderTexture GenerateSDFTexture(Texture floodTexture) {
            Update(floodTexture);
            return GetSDFTexture();
        }

        public void Update(Texture floodTexture) {
            if (!floodTexture) return;
            Vector2Int dimensions = new Vector2Int(floodTexture.width,floodTexture.height);
            if (!sdfTexture || sdfTexture.width != dimensions.x || sdfTexture.height != dimensions.y) {
                //Regenerate sdf texture
                Release();
                sdfTexture = RTUtils.CreateSimpleTexture("SDFTexture", dimensions, GraphicsFormat.R32G32B32A32_SFloat);
                computeShader.SetTexture(SDFKernel, ShaderIDs.SdfTex, sdfTexture);
                computeShader.SetTexture(SDFKernel, ShaderIDs.FloodTexRead, floodTexture);
                computeShader.SetInts(ShaderIDs.Dimensions, new int[] { dimensions.x, dimensions.y });
                computeShader.SetVector(ShaderIDs.TexelSize, floodTexture.texelSize);
                groupThreadCount = new Vector2Int(
                    Mathf.CeilToInt(dimensions.x / (float)threadGroupWidth),
                    Mathf.CeilToInt(dimensions.y / (float)threadGroupWidth));
            }
            //Compute SDF
            computeShader.Dispatch(SDFKernel, groupThreadCount.x, groupThreadCount.y, 1);
        }

        public void Release() {
            sdfTexture?.Release();
            sdfTexture = null;
        }

        #region Internal

        private static class ShaderIDs {
            public static readonly int SdfTex           = Shader.PropertyToID(nameof(SdfTex));
            public static readonly int Dimensions       = Shader.PropertyToID(nameof(Dimensions));
            public static readonly int TexelSize        = Shader.PropertyToID(nameof(TexelSize));
            public static readonly int FloodTexRead     = Shader.PropertyToID(nameof(FloodTexRead));
        }

        #endregion
    }
}