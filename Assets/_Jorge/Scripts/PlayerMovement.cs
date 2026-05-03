using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float aceleraccion = 15f;
    [SerializeField] private float VelocidadMaxima = 20f;
    [SerializeField] private float linearDampingAlChocar = 1f;

    [Header("Configuración de Rotación")]
    [SerializeField] private float velocidadRotacion = 15f;
    [SerializeField] private float rotacionXFija = 90f;

    [Header("Configuración de Boost")]
    [SerializeField] private float multiplicadorBoost = 2.5f;
    [SerializeField] private Light luzBoost;

    [Header("Referencias Externas")]
    [SerializeField] private TopDownCameraController camaraScript;

    [Header("Filtros de Colisión")]
    [SerializeField] private string tagPared = "Pared";
    [SerializeField] private LayerMask layerPared;

    [Header("Ajustes de Impacto")]
    [SerializeField] private float fuerzaRebote = 0.1f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 direccionActual;
    private bool estaDeslizando = false;
    private bool yaCambioDireccion = false;
    private bool boostActivo = false;

    // NUEVA VARIABLE: Para evitar que la luz se encienda sola tras chocar si sigues apretando el botón
    private bool requiereSoltarTecla = false;

    public bool IsBoostActive => boostActivo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (luzBoost != null) luzBoost.enabled = false;
    }

    void Update()
    {
        // 1. Detectamos si el jugador está tocando físicamente la tecla
        bool presionandoTecla = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);

        // 2. Si el jugador suelta la tecla, reseteamos el bloqueo del choque
        if (!presionandoTecla)
        {
            requiereSoltarTecla = false;
        }

        // 3. El boost solo se activa si estás presionando la tecla Y NO estás bloqueado por un choque
        boostActivo = presionandoTecla && !requiereSoltarTecla;

        if (luzBoost != null)
            luzBoost.enabled = boostActivo;

        if (camaraScript != null) camaraScript.SetBoost(boostActivo);

        ActualizarParametrosAnimacion();
        ProcesarRotacionZ();

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveZ != 0)
        {
            Vector3 nuevaDir = Vector3.zero;
            if (moveX != 0) nuevaDir = new Vector3(moveX, 0, 0).normalized;
            else if (moveZ != 0) nuevaDir = new Vector3(0, 0, moveZ).normalized;

            if (!estaDeslizando)
            {
                IniciarMovimiento(nuevaDir);
            }
            else if (nuevaDir != direccionActual && !yaCambioDireccion)
            {
                CambiarRumboUnico(nuevaDir);
            }
        }
    }

    private void ProcesarRotacionZ()
    {
        if (direccionActual == Vector3.zero) return;

        float anguloObjetivo = Mathf.Atan2(direccionActual.x, direccionActual.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(rotacionXFija, 0, -anguloObjetivo);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * velocidadRotacion);
    }

    private void ActualizarParametrosAnimacion()
    {
        if (animator == null) return;
        animator.SetBool("isMoving", estaDeslizando);
        animator.SetBool("isBoosting", boostActivo);
    }

    private void IniciarMovimiento(Vector3 dir)
    {
        if (!Physics.Raycast(transform.position, dir, 0.8f, layerPared))
        {
            rb.linearDamping = 0;
            direccionActual = dir;
            estaDeslizando = true;
            yaCambioDireccion = false;
        }
    }

    private void CambiarRumboUnico(Vector3 dir)
    {
        if (!Physics.Raycast(transform.position, dir, 0.8f, layerPared))
        {
            rb.linearVelocity = Vector3.zero;
            direccionActual = dir;
            yaCambioDireccion = true;
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
            GestionarImpacto(collision);
        }
    }

    private void GestionarImpacto(Collision collision)
    {
        estaDeslizando = false;
        yaCambioDireccion = false;

        if (animator != null) animator.SetTrigger("hit");

        // --- AQUÍ APLICAMOS LA PENALIZACIÓN ---
        requiereSoltarTecla = true; // Bloqueamos el input hasta que suelte la tecla
        boostActivo = false;        // Apagamos el estado de boost

        if (luzBoost != null) luzBoost.enabled = false;

        // Corregimos el error CS0029: Accedemos al primer contacto de la colisión
        ContactPoint contact = collision.GetContact(0);
        Vector3 direccionRebote = contact.normal;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direccionRebote * fuerzaRebote, ForceMode.Impulse);
        rb.linearDamping = linearDampingAlChocar;
        transform.position += direccionRebote * 0.05f;

        direccionActual = Vector3.zero;
    }
}