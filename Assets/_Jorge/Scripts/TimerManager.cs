using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI textoContador;
    [SerializeField] private Slider sliderTiempo;

    [Header("Game Over Settings")]
    [SerializeField] private GameObject canvasPerdio;

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoInicialSegundos = 180f;
    [SerializeField] private float multiplicadorDrenajeBoost = 10f;

    private float tiempoActual;
    private bool tiempoAgotado = false;

    void Start()
    {
        Time.timeScale = 1f;
        tiempoActual = tiempoInicialSegundos;
        if (canvasPerdio != null) canvasPerdio.SetActive(false);

        if (sliderTiempo != null)
        {
            sliderTiempo.maxValue = tiempoInicialSegundos;
            sliderTiempo.value = tiempoInicialSegundos;
        }
    }

    void Update()
    {
        if (tiempoAgotado) return;

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

        if (sliderTiempo != null) sliderTiempo.value = tiempoVisual;
    }

    public void TiempoFinalizado()
    {
        tiempoActual = 0;
        tiempoAgotado = true;

        if (canvasPerdio != null)
        {
            canvasPerdio.SetActive(true);

            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}