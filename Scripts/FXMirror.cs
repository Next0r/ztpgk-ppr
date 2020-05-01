using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXMirror : MonoBehaviour
{
    public Shader effectShader;
    private Material material;
    public Camera mirrorCamera;
    private Texture2D mirrorTexture;
    public Texture2D mirrorFrameTexture;
    public Texture2D mirrorFactorTexture;

    private Rect camRect;

    [Range(0.0f, 1.0f)]
    public float offsetX = 0.55f;
    [Range(0.0f, 1.0f)]
    public float offsetY = 0.1f;
    [Range(0.1f, 1.0f)]
    public float scaleX = 0.4f;
    [Range(0.1f, 1.0f)]
    public float scaleY = 0.4f;

    [Range(0, 1.0f)]
    public float frameOffsetX = 0.3f;
    [Range(0, 1.0f)]
    public float frameOffsetY = 0.7f;
    [Range(0.1f, 1.0f)]
    public float frameScaleX = 0.4f;
    [Range(0.1f, 1.0f)]
    public float frameScaleY = 0.3f;

    [Range(0.0f, 1.0f)]
    public float alpha = 0.75f;

    public Color reflectionColor = new Color(1f, 1f, 0.8f, 1f);

    private void Awake() {
        material = new Material(effectShader); 
    }

    private void Start()
    {
        mirrorTexture = new Texture2D(mirrorCamera.pixelWidth, mirrorCamera.pixelHeight);
    }

    private void Update()
    {
        if (mirrorTexture.width != mirrorCamera.pixelWidth || mirrorTexture.height != mirrorCamera.pixelHeight)
        {
            mirrorTexture = new Texture2D(mirrorCamera.pixelWidth, mirrorCamera.pixelHeight);
        }
    }
    private void OnPreRender()
    {
        var currentRT = RenderTexture.active;
        RenderTexture.active = mirrorCamera.targetTexture;

        mirrorCamera.Render();
        mirrorTexture.ReadPixels(new Rect(0, 0, mirrorCamera.pixelWidth, mirrorCamera.pixelHeight), 0, 0);
        mirrorTexture.Apply();

        RenderTexture.active = currentRT;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        material.SetFloat("_offsetX", offsetX * 1 / scaleX);
        material.SetFloat("_offsetY", offsetY * 1 / scaleY);
        material.SetFloat("_scaleX", 1 / scaleX);
        material.SetFloat("_scaleY", 1 / scaleY);

        material.SetFloat("_frameOffsetX", -frameOffsetX * 1 / frameScaleX);
        material.SetFloat("_frameOffsetY", -frameOffsetY * 1 / frameScaleY);
        material.SetFloat("_frameScaleX", 1 / frameScaleX);
        material.SetFloat("_frameScaleY", 1 / frameScaleY);

        material.SetFloat("_alpha", alpha);
        material.SetVector("_reflectionColor", reflectionColor);

        material.SetTexture("_MirrorTex", mirrorTexture);
        material.SetTexture("_FrameTex", mirrorFrameTexture);
        material.SetTexture("_FactorTex", mirrorFactorTexture);
        Graphics.Blit(src, dest, material);
    }

    

}
