using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí a tu Player (La Ratita) desde la jerarquía")]
    [SerializeField] private Transform jugador;

    [Tooltip("Arrastra aquí esta misma cámara")]
    [SerializeField] private Camera camaraPrincipal;

    [Header("Configuración de Seguimiento")]
    [Tooltip("La distancia que mantendrá la cámara respecto al jugador")]
    [SerializeField] private Vector3 offset = new Vector3(0, 20f, 0f);
    [Tooltip("Qué tan suave sigue al jugador")]
    [SerializeField] private float suavidadSeguimiento = 5f;

    [Header("Configuración de Zoom (Orthographic Size)")]
    [SerializeField] private float sizeNormal = 18f;
    [SerializeField] private float sizeBoost = 25f;
    [SerializeField] private float velocidadZoom = 4f;

    private float sizeObjetivo;

    private void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = GetComponent<Camera>();
        sizeObjetivo = sizeNormal;

        if (camaraPrincipal != null) camaraPrincipal.orthographicSize = sizeNormal;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void SetBoost(bool isBoosting)
    {
        sizeObjetivo = isBoosting ? sizeBoost : sizeNormal;
    }

    private void LateUpdate()
    {
        if (jugador == null) return;


        Vector3 posicionObjetivo = jugador.position + offset;
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, Time.deltaTime * suavidadSeguimiento);

        // 2. HACER ZOOM
        if (camaraPrincipal != null)
        {
            camaraPrincipal.orthographicSize = Mathf.Lerp(camaraPrincipal.orthographicSize, sizeObjetivo, Time.deltaTime * velocidadZoom);
        }
    }
}