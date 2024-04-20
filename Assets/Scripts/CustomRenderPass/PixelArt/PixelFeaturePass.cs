using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.Universal
{
    public class PixelFeaturePass : ScriptableRenderPass
    {
        Material matBlit;
        float pixelDensity;

        ProfilingSampler m_ProfilingSampler;
        RenderStateBlock m_RenderStateBlock;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        FilteringSettings m_FilteringSettings;

        static int pixelTexID = Shader.PropertyToID("_PixelTexture");
        static int pixelDepthID = Shader.PropertyToID("_DepthTex");

        public PixelFeaturePass(RenderPassEvent renderEvent, Material bM, float pD, int lM)
        {
            m_ProfilingSampler = new ProfilingSampler("BasicFeature");
            this.renderPassEvent = renderEvent;
            matBlit = bM;
            pixelDensity = pD;
            matBlit.SetFloat("_PixelDensity", pixelDensity);

            m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, lM);

            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;

            //Get the camera info of the default rendering pass
            ScriptableRenderer renderer = renderingData.cameraData.renderer;
            RenderTargetIdentifier colorHandle = renderer.cameraColorTarget;
            RenderTargetIdentifier depthHandle = renderer.cameraDepthTarget;

            //Generate necessary data for the pixel renderer
            DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
            ref CameraData cameraData = ref renderingData.cameraData;
            Camera camera = cameraData.camera;
            int pixelWidth = (int)(camera.pixelWidth / pixelDensity);
            int pixelHeight = (int)(camera.pixelHeight / pixelDensity);
            CommandBuffer cmd = CommandBufferPool.Get("BasicFeature");

            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                //set the render target and buffers for the pixel renderer
                cmd.GetTemporaryRT(pixelTexID, pixelWidth, pixelHeight, 0, FilterMode.Point);
                cmd.GetTemporaryRT(pixelDepthID, pixelWidth, pixelHeight, 24, FilterMode.Point, RenderTextureFormat.Depth);
                cmd.SetRenderTarget(pixelTexID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                                    pixelDepthID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
                //clear screen
                cmd.ClearRenderTarget(true, true, Color.clear);
                //do all of the above and start fresh
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                //draw the pixel pixel renderer
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
                //set the render texture back to the default to blend correctly
                cmd.SetRenderTarget(colorHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store,
                                    depthHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
                //render everything with the blit shader
                cmd.Blit(new RenderTargetIdentifier(pixelTexID), BuiltinRenderTextureType.CurrentActive, matBlit);
                //remove buffers to not have memoryLeak
                cmd.ReleaseTemporaryRT(pixelTexID);
                cmd.ReleaseTemporaryRT(pixelDepthID);
                //do all of the above and start fresh
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}