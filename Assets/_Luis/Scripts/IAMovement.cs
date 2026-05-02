using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class IAMovement : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    [SerializeField] private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
        ChangeSpeed(speed);
    }

    public void ChangeSpeed(float speed)
    {
        agent.speed = speed;
    }
}
