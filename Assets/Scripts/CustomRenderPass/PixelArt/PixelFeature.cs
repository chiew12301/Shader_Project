using UnityEngine.Rendering.Universal;

namespace UnityEngine.Experimental.Rendering.Universal
{
    public class PixelFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class PixelFeatureSettings
        {
            public LayerMask layerMask = 0;
            public RenderPassEvent evt = RenderPassEvent.BeforeRenderingTransparents;
            public Material matBlit = null;
            [Range(1.0f, 15.0f)]
            public float pixelDensity = 1.0f;
        }

        public PixelFeatureSettings settings = new PixelFeatureSettings();

        PixelFeaturePass pass;

        public override void Create()
        {
            pass = new PixelFeaturePass(settings.evt, settings.matBlit, settings.pixelDensity, settings.layerMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }
    }
}