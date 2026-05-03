using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFootsteps : MonoBehaviour
{
    [Header("Librería de Sonidos")]
    [Tooltip("Arrastra aquí tus 8 o más variantes de pasos")]
    [SerializeField] private AudioClip[] sonidosDePasos;

    [Header("Configuración de Pasos")]
    [Tooltip("Distancia que debe recorrer el enemigo para que suene un paso")]
    [SerializeField] private float distanciaEntrePasos = 1.5f;

    [Tooltip("Volumen del paso")]
    [Range(0f, 1f)]
    [SerializeField] private float volumen = 0.2f;

    private AudioSource audioSource;
    private NavMeshAgent agent;
    private float distanciaAcumulada = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();


        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {

        if (agent.velocity.magnitude > 0.1f)
        {

            distanciaAcumulada += agent.velocity.magnitude * Time.deltaTime;


            if (distanciaAcumulada >= distanciaEntrePasos)
            {
                ReproducirPaso();
                distanciaAcumulada = 0f; 
            }
        }
        else
        {

            distanciaAcumulada = 0f;
        }
    }

    private void ReproducirPaso()
    {

        if (sonidosDePasos.Length == 0) return;

        int indiceAleatorio = Random.Range(0, sonidosDePasos.Length);
        AudioClip pasoElegido = sonidosDePasos[indiceAleatorio];

        audioSource.pitch = Random.Range(0.9f, 1.8f);


        audioSource.PlayOneShot(pasoElegido, volumen);
    }
}