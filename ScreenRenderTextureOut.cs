using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class ScreenRenderTextureOut : MonoBehaviour
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
    [ShowAssetPreview, ReadOnly]
    public RenderTexture preview;
    private Vector2 prevScreenSize;

    void Update()
    {
        if (Seb.screenSize == prevScreenSize) { return; }
        BindRenderTexture();
    }

    private void OnValidate() => BindRenderTexture();
    private void OnEnable() => BindRenderTexture();

    private void BindRenderTexture()
    {
        if (!cam) { return; }

        cam.targetTexture = preview = new RenderTexture((int)(Screen.width / downsampling),
            (int)(Screen.height / downsampling), 0, format);
        Shader.SetGlobalTexture(bufferName, cam.targetTexture);

        prevScreenSize = Seb.screenSize;
    }

    private void OnDisable()
    {
        if (!cam) { return; }

        _cam.targetTexture = null;
    }
}