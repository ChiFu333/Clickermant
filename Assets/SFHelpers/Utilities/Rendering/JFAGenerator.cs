using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace SFH {
    public class JFAGenerator {
        private const int threadGroupWidth = 8;

        private ComputeShader computeShader;
        private RenderTexture floodTexture;
        private int seedKernel;
        private int floodKernel;
        private Vector2Int groupThreadCount;

        public JFAGenerator() {
            computeShader = Object.Instantiate(Resources.Load<ComputeShader>(nameof(JFAGenerator)));
            computeShader.hideFlags = HideFlags.HideAndDontSave;
            seedKernel = computeShader.FindKernel(nameof(seedKernel));
            floodKernel = computeShader.FindKernel(nameof(floodKernel));
        }

        public RenderTexture GetFloodTexture() => floodTexture;

        public RenderTexture GenerateFloodTexture(Texture sourceTexture, float sourceValueThreshold, int borderThickness) {
            Update(sourceTexture, sourceValueThreshold, borderThickness);
            return GetFloodTexture();
        }

        public void Update(Texture sourceTexture, float sourceValueThreshold, int borderThickness) {
            if (!sourceTexture) return;
            Vector2Int dimensions = new Vector2Int(sourceTexture.width + borderThickness * 2,sourceTexture.height + borderThickness * 2);
            if (!floodTexture || floodTexture.width != dimensions.x || floodTexture.height != dimensions.y) {
                //Regenerate flood texture
                Release();
                floodTexture = RTUtils.CreateSimpleTexture("FloodTexture", dimensions, GraphicsFormat.R32G32B32A32_SFloat);
                computeShader.SetTexture(seedKernel, ShaderIDs.FloodTex, floodTexture);
                computeShader.SetTexture(floodKernel, ShaderIDs.FloodTex, floodTexture);
                computeShader.SetInt(ShaderIDs.BorderOffset, borderThickness);
                computeShader.SetInts(ShaderIDs.Dimensions, new int[] { dimensions.x, dimensions.y });
                computeShader.SetVector(ShaderIDs.TexelSize, floodTexture.texelSize);
                groupThreadCount = new Vector2Int(
                    Mathf.CeilToInt(dimensions.x / (float)threadGroupWidth),
                    Mathf.CeilToInt(dimensions.y / (float)threadGroupWidth));
            }

            //Seed
            computeShader.SetTexture(seedKernel, ShaderIDs.SeedTexRead, sourceTexture);
            computeShader.SetFloat(ShaderIDs.SeedThreshold, sourceValueThreshold);
            computeShader.Dispatch(seedKernel, groupThreadCount.x, groupThreadCount.y, 1);

            //Flood
            int sizeMax = Mathf.Max(dimensions.x,dimensions.y);
            int stepMax = (int)Mathf.Log(Mathf.NextPowerOfTwo(sizeMax),2);
            for (int n = stepMax; n >= 0; n--) {
                int stepSize = n > 0 ? (int)Mathf.Pow(2,n) : 1;
                computeShader.SetInt(ShaderIDs.StepSize, stepSize);
                computeShader.Dispatch(floodKernel, dimensions.x, dimensions.y, 1);
            }
        }

        public void Release() {
            floodTexture?.Release();
            floodTexture = null;
        }

        #region Internal

        private static class ShaderIDs {
            public static readonly int FloodTex         = Shader.PropertyToID(nameof(FloodTex));
            public static readonly int Dimensions       = Shader.PropertyToID(nameof(Dimensions));
            public static readonly int TexelSize        = Shader.PropertyToID(nameof(TexelSize));
            public static readonly int SeedTexRead      = Shader.PropertyToID(nameof(SeedTexRead));
            public static readonly int SeedThreshold    = Shader.PropertyToID(nameof(SeedThreshold));
            public static readonly int StepSize         = Shader.PropertyToID(nameof(StepSize));
            public static readonly int BorderOffset         = Shader.PropertyToID(nameof(BorderOffset));
        }

        #endregion
    }
}