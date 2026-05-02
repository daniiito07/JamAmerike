using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float aceleraccion = 15f;
    [SerializeField] private float VelocidadMaxima = 20f;
    [SerializeField] private float linearDampingAlChocar = 1f;

    [Header("Configuración de Boost")]
    [SerializeField] private float multiplicadorBoost = 2.5f;
    [SerializeField] private Light luzBoost;

    [Header("Filtros de Colisión")]
    [SerializeField] private string tagPared = "Pared";
    [SerializeField] private LayerMask layerPared;

    [Header("Ajustes de Impacto")]
    [SerializeField] private float fuerzaRebote = 0.1f;

    private Rigidbody rb;
    private Vector3 direccionActual;
    private bool estaDeslizando = false;
    private bool yaCambioDireccion = false; 
    private bool boostActivo = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (luzBoost != null) luzBoost.enabled = false;
    }

    void Update()
    {
        boostActivo = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);
        if (luzBoost != null) luzBoost.enabled = boostActivo;

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

            Debug.Log("¡Cambio único utilizado!");
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
        direccionActual = Vector3.zero;

        ContactPoint contact = collision.contacts[0];
        Vector3 direccionRebote = contact.normal;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direccionRebote * fuerzaRebote, ForceMode.Impulse);
        rb.linearDamping = linearDampingAlChocar;
        transform.position += direccionRebote * 0.05f;
    }
}