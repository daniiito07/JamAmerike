using UnityEngine;

public class EnvironmentAudioManager : MonoBehaviour
{
    [Header("Música de Fondo (BGM)")]
    [SerializeField] private AudioClip musica;
    [Range(0f, 1f)][SerializeField] private float volumenMusica = 0.5f;

    [Header("Sonido Ambiental (Viento, zumbido, etc.)")]
    [SerializeField] private AudioClip ambiente;
    [Range(0f, 1f)][SerializeField] private float volumenAmbiente = 0.5f;

    private AudioSource sourceMusica;
    private AudioSource sourceAmbiente;

    void Awake()
    {
        if (musica != null)
        {
            sourceMusica = gameObject.AddComponent<AudioSource>();
            ConfigurarAudioSource(sourceMusica, musica, volumenMusica);
            sourceMusica.Play();
        }

        if (ambiente != null)
        {
            sourceAmbiente = gameObject.AddComponent<AudioSource>();
            ConfigurarAudioSource(sourceAmbiente, ambiente, volumenAmbiente);
            sourceAmbiente.Play();
        }
    }

    private void ConfigurarAudioSource(AudioSource source, AudioClip clip, float volumen)
    {
        source.clip = clip;
        source.volume = volumen;
        source.loop = true;          
        source.spatialBlend = 0f;    
        source.playOnAwake = false;
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (sourceMusica == null) return;
        sourceMusica.clip = nuevaMusica;
        sourceMusica.Play();
    }

    public void DetenerAmbiente()
    {
        if (sourceAmbiente != null) sourceAmbiente.Stop();
    }
}