using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public class VignetteController : MonoBehaviour
{
    public Volume globalVolume;
    private Vignette vignette;
    private float intensidadObjetivo = 0.8f;

    void Start()
    {
        globalVolume.profile.TryGet(out vignette);
    }

    // Esta función será llamada por tu PlayerMovement
    public void SetBoost(bool isBoosting)
    {
        intensidadObjetivo = isBoosting ? 0.3f : 1f;
    }

    void Update()
    {
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, intensidadObjetivo, Time.deltaTime * 5f);
        }
    }
}