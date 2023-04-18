using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class ScreenRenderTextureOut : CustomMono
{
    [SerializeField]
    private Camera _cam;
    private Camera cam
    {
        get
        {
            if (!_cam) { _cam = GetComponent<Camera>(); }
            return _cam;
        }
        set => _cam = value;
    }
    public string bufferName;
    public GraphicsFormat format;
    [Range(1, 5)]
    public float downsampling;
    [PreviewField(100, ObjectFieldAlignment.Center)]
    public RenderTexture preview;
    
    protected override void OnEditorAwake() => OnEnable();
    private void OnEnable()
    {
        OnScreenSizeChange += BindRenderTexture;
        BindRenderTexture();
    }

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
        if (!cam) { return; }

        _cam.targetTexture = null;
    }
}