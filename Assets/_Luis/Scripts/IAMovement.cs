using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IAMovement : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Coloca todas las referencias necesarias para que el AI agent pueda encontrar y seguir al player")]
    public Transform target;
    private NavMeshAgent agent;
    private PlayerMovement playerMovement;

    [Header("Configuración de Velocidad")]
    [SerializeField] private float normalSpeed = 3.5f;
    [SerializeField] private float lightSpeed = 7f;

    [Header("Eventos de Juego")]
    [Tooltip("Aquí puedes arrastrar tu Canvas/Fondo de Game Over y poner GameObject.SetActive a true")]
    public UnityEvent onGameOver;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();


        agent.updateRotation = false;

        if (target != null)
        {
            playerMovement = target.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        if (target == null) return;


        agent.destination = target.position;


        if (playerMovement != null && playerMovement.IsBoostActive)
        {
            ChangeSpeed(lightSpeed);
        }
        else
        {
            ChangeSpeed(normalSpeed);
        }


        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direccion = agent.velocity.normalized;
            float anguloObjetivo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;


            transform.rotation = Quaternion.Euler(90f, 0f, -anguloObjetivo);
        }
    }

    public void ChangeSpeed(float newSpeed)
    {
        if (agent.speed != newSpeed)
        {
            agent.speed = newSpeed;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            AtraparJugador(collision.gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AtraparJugador(other.gameObject);
        }
    }

    private void AtraparJugador(GameObject jugador)
    {

        agent.isStopped = true;
        agent.velocity = Vector3.zero;


        Animator playerAnim = jugador.GetComponent<Animator>();
        if (playerAnim != null)
        {

            playerAnim.SetTrigger("hit");
        }


        PlayerMovement movJugador = jugador.GetComponent<PlayerMovement>();
        if (movJugador != null)
        {
            movJugador.enabled = false;
        }


        if (onGameOver != null)
        {
            onGameOver.Invoke();
        }
    }
}