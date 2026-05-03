using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí tu Main Camera (la que tiene el componente Camera)")]
    [SerializeField] private Camera camaraPrincipal;

    [Header("Configuración de Zoom (Orthographic Size)")]
    [SerializeField] private float sizeNormal = 18f;
    [SerializeField] private float sizeBoost = 100f;

    [Header("Animación")]
    [SerializeField] private float velocidadTransicion = 4f;

    private float sizeObjetivo;

    private void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = GetComponent<Camera>();

        sizeObjetivo = sizeNormal;

        if (camaraPrincipal != null) camaraPrincipal.orthographicSize = sizeNormal;

        
    }

    public void SetBoost(bool isBoosting)
    {
        sizeObjetivo = isBoosting ? sizeBoost : sizeNormal;
        
    }

    private void LateUpdate()
    {
        if (camaraPrincipal != null)
        {
            camaraPrincipal.orthographicSize = Mathf.Lerp(camaraPrincipal.orthographicSize, sizeObjetivo, Time.deltaTime * velocidadTransicion);
        }
    }
}