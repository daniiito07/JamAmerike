using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI textoContador;
    [SerializeField] private Slider sliderTiempo;

    [Header("Game Over Settings")]
    [Tooltip("Arrastra aquí el objeto 'Canvas Perdio'")]
    [SerializeField] private GameObject canvasPerdio;

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoInicialSegundos = 180f;

    [Header("Configuración del Drenaje de Boost")]
    [Tooltip("Segundos extra que se pierden POR CADA SEGUNDO de boost activado")]
    [SerializeField] private float multiplicadorDrenajeBoost = 10f;

    private float tiempoActual;
    private bool tiempoAgotado = false;

    void Start()
    {
        tiempoActual = tiempoInicialSegundos;

        // Aseguramos que el panel de derrota esté apagado al empezar
        if (canvasPerdio != null) canvasPerdio.SetActive(false);

        if (sliderTiempo != null)
        {
            sliderTiempo.minValue = 0;
            sliderTiempo.maxValue = tiempoInicialSegundos;
            sliderTiempo.value = tiempoInicialSegundos;
        }
    }

    void Update()
    {
        if (tiempoAgotado) return;

        // Detección del Boost
        bool estaUsandoBoost = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);
        float flujoTiempo = Time.deltaTime;

        if (estaUsandoBoost)
        {
            flujoTiempo += multiplicadorDrenajeBoost * Time.deltaTime;
            textoContador.color = Color.red;
        }
        else
        {
            textoContador.color = Color.white;
        }

        // Aplicar reducción
        if (tiempoActual > 0)
        {
            tiempoActual -= flujoTiempo;
            ActualizarInterfaz();
        }
        else
        {
            TiempoFinalizado();
        }
    }

    void ActualizarInterfaz()
    {
        float tiempoVisual = Mathf.Max(0, tiempoActual);
        int minutos = Mathf.FloorToInt(tiempoVisual / 60);
        int segundos = Mathf.FloorToInt(tiempoVisual % 60);
        textoContador.text = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (sliderTiempo != null)
        {
            sliderTiempo.value = tiempoVisual;
        }
    }

    void TiempoFinalizado()
    {
        tiempoActual = 0;
        tiempoAgotado = true;
        textoContador.text = "00:00";
        if (sliderTiempo != null) sliderTiempo.value = 0;

        if (canvasPerdio != null)
        {
            canvasPerdio.SetActive(true); // Muestra el panel de derrota
            Time.timeScale = 0f;          // Pausa el juego
            Cursor.lockState = CursorLockMode.None; // Libera el mouse
            Cursor.visible = true;
        }

        Debug.Log("¡Tiempo agotado!");
    }
}