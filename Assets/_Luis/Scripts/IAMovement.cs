using UnityEngine;
using UnityEngine.AI;

public class IAMovement : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.velocity = new Vector3(0.5f,0.5f,0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
    }
}
