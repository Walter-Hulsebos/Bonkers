using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class MultiPassRendering : ScriptableRendererFeature
{
    public List<string> lightModePasses;
    private MultiPassPass mainPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(mainPass);
    }
    public override void Create()
    {
        mainPass = new MultiPassPass(lightModePasses);
    }

    public class MultiPassPass : ScriptableRenderPass
    {
        private List<ShaderTagId> m_Tags;

        public MultiPassPass(List<string> tags)
        {
            m_Tags = new List<ShaderTagId>();
            foreach (string tag in tags)
            {
                m_Tags.Add(new ShaderTagId(tag));
            }

            this.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            FilteringSettings filteringSettings = FilteringSettings.defaultValue;

            foreach (ShaderTagId pass in m_Tags)
            {
                DrawingSettings drawingSettings = CreateDrawingSettings(pass, ref renderingData, SortingCriteria.CommonOpaque);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }

            context.Submit();
        }
    }
}


