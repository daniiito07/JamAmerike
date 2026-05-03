using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private string nombreMenu, levelName;

    // Esta función aparecerá en el menú desplegable del botón (On Click)
    public void Retry()
    {
        // Muy importante: devolver el tiempo a la normalidad antes de cargar
        Time.timeScale = 1f;

        // Recarga la escena en la que estamos actualmente
        SceneManager.LoadScene(levelName);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreMenu);
    }
}