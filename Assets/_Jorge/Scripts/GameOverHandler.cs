using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    // Esta es la función que debes seleccionar en el On Click() del botón
    public void ReiniciarEscena()
    {
        // IMPORTANTE: Primero restaurar el tiempo, luego cargar
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}