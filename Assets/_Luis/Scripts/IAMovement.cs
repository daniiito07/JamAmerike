using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class IAMovement : MonoBehaviour
{
    [Tooltip("Coloca todas las referencias necesarias para que en AI agent pueda encontrar y seguir al player o target puesto")]
    [Header("Referencias")]
    public Transform target;
    private NavMeshAgent agent;
    [SerializeField] private float normalSpeed, lightSpeed;
    [SerializeField] private PlayerMovement playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (target != null)
        {
            playerMovement = target.GetComponent<PlayerMovement>();
        }
    }

    // Update is called once per frame
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

    }

    public void ChangeSpeed(float newSpeed)
    {
        if (agent.speed != newSpeed)
        {
            agent.speed = newSpeed;
        }
    }
}

