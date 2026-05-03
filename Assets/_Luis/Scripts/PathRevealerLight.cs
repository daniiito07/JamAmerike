using UnityEngine;
using UnityEngine.Rendering;

public class PathRevealerLight : MonoBehaviour
{
    [Header("Configuración de la Luz Reveladora")]
    [Tooltip("Arrastra aquí la Point Light que creaste")]
    [SerializeField] private Light luzReveladora;

    [Tooltip("¿Cuántos segundos dura la luz encendida?")]
    [SerializeField] private float tiempoActiva = 5f;

    [Tooltip("Velocidad a la que se apaga (Fade out)")]
    [SerializeField] private float velocidadApagado = 2f;

    [Tooltip("Intensidad máxima a la que llegará la luz")]
    [SerializeField] private float intensidadMaxima = 5f;

    private float temporizador = 0f;
    private bool estaRevelando = false;

    private void Start()
    {  
        if (luzReveladora != null)
        {
            luzReveladora.intensity = 180f;
        }
        else
        {
            Debug.LogWarning("¡Olvidaste asignar la luz reveladora en el Inspector!");
        }
    }

    private void Update()
    {
        // EJEMPLO: Activar con la tecla E. 
        // Si quieres que se active al agarrar un ítem, puedes borrar esta parte del Input
        // y simplemente llamar al método ActivarLuz() desde tu script de items.
        if (Input.GetKeyDown(KeyCode.E) && !estaRevelando)
        {
            ActivarLuz();
        }

        if (estaRevelando)
        {
            temporizador += Time.deltaTime;

            if (temporizador >= tiempoActiva)
            {
                luzReveladora.intensity = Mathf.Lerp(luzReveladora.intensity, 0f, Time.deltaTime * velocidadApagado);

                if (luzReveladora.intensity < 0.05f)
                {
                    luzReveladora.intensity = 0f;
                    estaRevelando = false;
                }
            }
        }
    }

    public void ActivarLuz()
    {
        if (luzReveladora != null)
        {
            luzReveladora.intensity = intensidadMaxima;
            temporizador = 0f;
            estaRevelando = true;
        }
    }
}