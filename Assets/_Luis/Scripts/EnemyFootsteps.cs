using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFootsteps : MonoBehaviour
{
    [Header("Librería de Sonidos")]
    [Tooltip("Arrastra aquí tus 5 o más variantes de pasos")]
    [SerializeField] private AudioClip[] sonidosDePasos;

    [Header("Configuración de Pasos")]
    [Tooltip("Distancia que debe recorrer el enemigo para que suene un paso")]
    [SerializeField] private float distanciaEntrePasos = 1.5f;

    [Tooltip("Volumen del paso")]
    [Range(0f, 1f)]
    [SerializeField] private float volumen = 0.8f;

    private AudioSource audioSource;
    private NavMeshAgent agent;
    private float distanciaAcumulada = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        // Aseguramos por código que el AudioSource sea 3D
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        // Si el agente se está moviendo (velocidad mayor a 0.1 para evitar ruido estando quieto)
        if (agent.velocity.magnitude > 0.1f)
        {
            // Calculamos cuánta distancia ha recorrido en este frame
            distanciaAcumulada += agent.velocity.magnitude * Time.deltaTime;

            // Si ya recorrió la distancia necesaria para un paso
            if (distanciaAcumulada >= distanciaEntrePasos)
            {
                ReproducirPaso();
                distanciaAcumulada = 0f; // Reiniciamos el contador
            }
        }
        else
        {
            // Si se detiene, reseteamos la distancia para que el próximo paso sea inmediato al arrancar
            distanciaAcumulada = 0f;
        }
    }

    private void ReproducirPaso()
    {
        // Medida de seguridad por si olvidaste poner los sonidos
        if (sonidosDePasos.Length == 0) return;

        // 1. Elegimos un sonido al azar de las variantes
        int indiceAleatorio = Random.Range(0, sonidosDePasos.Length);
        AudioClip pasoElegido = sonidosDePasos[indiceAleatorio];

        // 2. Modificamos el "Pitch" ligeramente para dar aún más variedad (opcional pero muy recomendado)
        // Esto hace que el mismo archivo suene un poquito más grave o agudo cada vez
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        // 3. Reproducimos el sonido sin cortar el anterior (PlayOneShot)
        audioSource.PlayOneShot(pasoElegido, volumen);
    }
}