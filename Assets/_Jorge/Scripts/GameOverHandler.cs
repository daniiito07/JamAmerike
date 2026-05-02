using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class GameOverHandler : MonoBehaviour
{
    // Esta función aparecerá en el menú desplegable del botón (On Click)
    public void ReiniciarEscena()
    {
        // Muy importante: devolver el tiempo a la normalidad antes de cargar
        Time.timeScale = 1f;

        // Recarga la escena en la que estamos actualmente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu(string nombreMenu)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreMenu);
    }
}