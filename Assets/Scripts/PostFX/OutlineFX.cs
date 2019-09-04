using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class OutlineFX : MonoBehaviour {

    [SerializeField] Color color;
    [SerializeField] [Range(0.1f, 5.0f)] float lineWidth = 1.0f;

    private Material m_effectMaterial;
    public Material stencilMaterial
    {
        get
        {
            if (m_effectMaterial == null)
            {
                m_effectMaterial = new Material(Shader.Find("Hidden/StencilBufferReader"));
                m_effectMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_effectMaterial;
        }
    }
    private Material m_outlineMaterial;
    public Material outlineMaterial
    {
        get
        {
            if (m_outlineMaterial == null)
            {
                m_outlineMaterial = new Material(Shader.Find("Hidden/OutlineFX"));
                m_outlineMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_outlineMaterial;
        }
    }
    private CommandBuffer commandBuffer;

    private RenderTexture buffer;

    private Camera m_Camera;
    public new Camera camera
    {
        get
        {
            if (m_Camera == null)
            {
                m_Camera = GetComponent<Camera>();
            }

            return m_Camera;
        }
    }


    private void OnEnable()
    {
        if (commandBuffer == null)
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "commandBuffer";

            int cachedScreenImageID = Shader.PropertyToID("_Temp");
            commandBuffer.GetTemporaryRT(cachedScreenImageID, -1, -1, 0);

            commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, cachedScreenImageID);
            commandBuffer.Blit(cachedScreenImageID, BuiltinRenderTextureType.CameraTarget, stencilMaterial);

            commandBuffer.SetGlobalTexture("_CachedScreenImage", cachedScreenImageID);
            camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, commandBuffer);
        }
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture cachedScreenRT = Shader.GetGlobalTexture("_CachedScreenImage") as RenderTexture;

        //texture.ReadPixels(new Rect(Screen.width / 2, Screen.height / 2, texture.width, texture.height), 0, 0, false);
        //Debug.Log(texture.GetPixel(0, 0));
        buffer = RenderTexture.GetTemporary(cachedScreenRT.width, cachedScreenRT.height);

        Graphics.Blit(cachedScreenRT, buffer);

        outlineMaterial.SetTexture("_Stencil", source); // the source texture is the resultant of stencil buffer
        outlineMaterial.SetColor("_Color", color);
        outlineMaterial.SetFloat("_Precision", lineWidth * 0.001f);
        Graphics.Blit(buffer, destination, outlineMaterial);


        RenderTexture.ReleaseTemporary(buffer);
    }
}

