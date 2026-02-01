using UnityEngine;
using UnityEngine.UI;

public class PauseBackgroundBlur : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera captureCamera;   // meist Main Camera
    [SerializeField] private RawImage target;         // RawImage (BlurBG)

    [Header("Fake Blur Settings")]
    [Range(0.05f, 1f)]
    [SerializeField] private float resolutionScale = 0.2f; // 0.15-0.25 = nice blur

    private RenderTexture rt;

    private void Awake()
    {
        if (captureCamera == null) captureCamera = Camera.main;
    }

    public void Capture()
    {
        if (captureCamera == null || target == null) return;

        int w = Mathf.Max(64, Mathf.RoundToInt(Screen.width * resolutionScale));
        int h = Mathf.Max(64, Mathf.RoundToInt(Screen.height * resolutionScale));

        if (rt == null || rt.width != w || rt.height != h)
        {
            if (rt != null) rt.Release();
            rt = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32);
            rt.filterMode = FilterMode.Bilinear; // wichtig fürs weiche Hochskalieren
            rt.Create();
        }

        var prev = captureCamera.targetTexture;
        captureCamera.targetTexture = rt;
        captureCamera.Render();
        captureCamera.targetTexture = prev;

        target.texture = rt;
    }
}
