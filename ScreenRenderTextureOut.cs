using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class ScreenRenderTextureOut : CustomMono
{
    [SerializeField]
    private Camera cam;
    public string bufferName;
    public GraphicsFormat format = GraphicsFormat.R32G32B32A32_SFloat;
    [Range(1, 5)]
    public float downsampling = 1;
    [PreviewField(100, ObjectFieldAlignment.Center)]
    public RenderTexture preview;
    
    private void OnEnable()
    {
        OnScreenSizeChange += BindRenderTexture;
        BindRenderTexture();
    }

    [Button]
    private void BindRenderTexture()
    {
        if (preview != null)
        {
            preview.Release();
            preview = null;
        }
        
        if (!cam || Screen.width < 50) { return; }

        cam.targetTexture = preview = new RenderTexture((int)(Screen.width / downsampling),
            (int)(Screen.height / downsampling), 0, format);
        Shader.SetGlobalTexture(bufferName, cam.targetTexture);
    }

    private void OnDisable()
    {
        OnScreenSizeChange -= BindRenderTexture;
        if (preview != null)
        {
            preview.Release();
            preview = null;
        }
        if (!cam) { return; }

        cam.targetTexture = null;
    }
}