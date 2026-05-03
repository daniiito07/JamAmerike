using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
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

    [Header("Audio")]
    [SerializeField] private AudioClip[] sonidosPasos;
    [SerializeField] private float tiempoEntrePasos = 0.3f;
    [SerializeField] private AudioClip sonidoEstatica;

    private AudioSource audioSource;
    private float temporizadorPasos;

    [Header("Victoria y UI")]
    [SerializeField] private string tagPared = "Pared";
    [SerializeField] private LayerMask layerPared;
    [SerializeField] private string tagVictoria = "Victoria";
    [Tooltip("Arrastra aquí el objeto del Panel de Victoria desde tu Canvas")]
    [SerializeField] private GameObject panelVictoria; // <-- REFERENCIA DIRECTA AL PANEL

    [Header("Ajustes de Impacto")]
    [SerializeField] private float fuerzaRebote = 0.1f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 direccionActual;
    private bool estaDeslizando = false;
    private bool yaCambioDireccion = false;
    private bool boostActivo = false;
    private bool requiereSoltarTecla = false;
    private bool juegoPausado = false;

    public bool IsBoostActive => boostActivo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (luzBoost != null) luzBoost.enabled = false;

        // Aseguramos que el panel inicie apagado por seguridad
        if (panelVictoria != null) panelVictoria.SetActive(false);

        audioSource.loop = true;
        audioSource.clip = sonidoEstatica;
    }

    void Update()
    {
        if (juegoPausado) return;

        bool presionandoTecla = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);

        if (!presionandoTecla)
        {
            requiereSoltarTecla = false;
        }

        boostActivo = presionandoTecla && !requiereSoltarTecla;

        if (luzBoost != null)
            luzBoost.enabled = boostActivo;

        if (camaraScript != null) camaraScript.SetBoost(boostActivo);

        ActualizarParametrosAnimacion();
        ProcesarRotacionZ();
        ManejarAudio();

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

    private void ManejarAudio()
    {
        if (boostActivo)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }

        if (estaDeslizando)
        {
            temporizadorPasos -= Time.deltaTime;

            float limiteTiempo = boostActivo ? tiempoEntrePasos * 0.6f : tiempoEntrePasos;

            if (temporizadorPasos <= 0f)
            {
                ReproducirPaso();
                temporizadorPasos = limiteTiempo;
            }
        }
        else
        {
            temporizadorPasos = 0f;
        }
    }

    private void ReproducirPaso()
    {
        if (sonidosPasos.Length > 0)
        {
            int index = Random.Range(0, sonidosPasos.Length);
            audioSource.PlayOneShot(sonidosPasos[index], 0.7f);
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
        if (juegoPausado) return;

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

        if (collision.gameObject.CompareTag(tagVictoria))
        {
            GanarJuego();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagVictoria))
        {
            GanarJuego();
        }
    }

    private void GanarJuego()
    {
        if (juegoPausado) return;

        juegoPausado = true;
        estaDeslizando = false;

        if (audioSource.isPlaying) audioSource.Stop();

        // --- ACTIVACIÓN DIRECTA DEL PANEL ---
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No asignaste el panel de victoria en el inspector.");
        }

        // Pausamos el juego
        Time.timeScale = 0f;
    }

    private void GestionarImpacto(Collision collision)
    {
        estaDeslizando = false;
        yaCambioDireccion = false;

        if (animator != null) animator.SetTrigger("hit");

        requiereSoltarTecla = true;
        boostActivo = false;

        if (luzBoost != null) luzBoost.enabled = false;

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