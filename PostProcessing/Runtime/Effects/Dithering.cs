using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    internal sealed class Dithering
    {
        int m_NoiseTextureIndex = 0;
        System.Random m_Random = new System.Random(1234);
        private string message = "PostProcessingV2: Dithering Failed on Forked Version.";

        internal void Render(PostProcessRenderContext context)
        {
            // check is null context.resources or context.uberSheet
            if (context == null || context.resources == null || context.uberSheet == null) {
                YNSGames.Logger.LogWarning(message);
                return;
            }
            
            var blueNoise = context.resources.blueNoise64;
            if (blueNoise == null || blueNoise.Length == 0) { // check blueNoise and Size
                YNSGames.Logger.LogWarning(message);
                return;
            }
            /*Assert.IsTrue(blueNoise != null && blueNoise.Length > 0);*/

#if POSTFX_DEBUG_STATIC_DITHERING // Used by QA for automated testing
            m_NoiseTextureIndex = 0;
            float rndOffsetX = 0f;
            float rndOffsetY = 0f;
#else
            if (++m_NoiseTextureIndex >= blueNoise.Length)
                m_NoiseTextureIndex = 0;

            float rndOffsetX = (float)m_Random.NextDouble();
            float rndOffsetY = (float)m_Random.NextDouble();
#endif

            var noiseTex = blueNoise[m_NoiseTextureIndex]; // 'IndexOutOfRangeException'
            var uberSheet = context.uberSheet;
            if (noiseTex == null) {
                YNSGames.Logger.LogWarning(message);
                return;
            }

            uberSheet.properties.SetTexture(ShaderIDs.DitheringTex, noiseTex);
            uberSheet.properties.SetVector(ShaderIDs.Dithering_Coords, new Vector4(
                (float)context.screenWidth / (float)noiseTex.width,
                (float)context.screenHeight / (float)noiseTex.height,
                rndOffsetX,
                rndOffsetY
            ));
        }
    }
}
