using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float aceleraccion = 15f;
    [SerializeField] private float VelocidadMaxima = 20f;

    [Header("Configuración de Boost")]
    [SerializeField] private float multiplicadorBoost = 2.5f;
    [SerializeField] private Light luzBoost;

    [Header("Filtros de Colisión")]
    [SerializeField] private string tagPared = "Pared";
    [SerializeField] private LayerMask layerPared;

    private Rigidbody rb;
    private Vector3 direccionActual;
    private bool estaDeslizando = false;
    private bool boostActivo = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        // CRÍTICO: Debe ser Continuous para no atravesar paredes
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (luzBoost != null) luzBoost.enabled = false;
    }

    void Update()
    {
        boostActivo = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);
        if (luzBoost != null) luzBoost.enabled = boostActivo;

        if (!estaDeslizando)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");

            if (moveX != 0 || moveZ != 0)
            {
                Vector3 nuevaDir = (moveX != 0) ? new Vector3(moveX, 0, 0) : new Vector3(0, 0, moveZ);

                // SEGURIDAD: Solo arrancar si no hay una pared inmediatamente enfrente
                if (!Physics.Raycast(transform.position, nuevaDir.normalized, 0.6f, layerPared))
                {
                    direccionActual = nuevaDir.normalized;
                    estaDeslizando = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (estaDeslizando)
        {
            float fuerzaFinal = boostActivo ? aceleraccion * multiplicadorBoost : aceleraccion;
            float velMaxFinal = boostActivo ? VelocidadMaxima * multiplicadorBoost : VelocidadMaxima;

            if (rb.linearVelocity.magnitude < velMaxFinal)
            {
                rb.AddForce(direccionActual * fuerzaFinal, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagPared))
        {
            LiberarJugador(collision);
        }
    }

    // Backup por si se queda vibrando contra la pared
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagPared) && estaDeslizando)
        {
            LiberarJugador(collision);
        }
    }

    private void LiberarJugador(Collision collision)
    {
        // 1. Detener física
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2. Desbloquear control
        estaDeslizando = false;
        direccionActual = Vector3.zero;

        // 3. EMPUJÓN DE SEGURIDAD (Anti-Atrapamiento)
        // Obtenemos el punto de contacto para empujar al jugador hacia afuera
        ContactPoint contact = collision.contacts[0];
        Vector3 pushDir = contact.normal;

        // Movemos al jugador un poco hacia afuera de la pared
        transform.position += pushDir * 0.05f;
    }
}