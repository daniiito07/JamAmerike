using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void VolverAlMenu()
    {
        UIStateHandler.Instance.SetState(UIState.MAIN_MENU);  // ← solo cambia el panel
    }

    public void CargarMenu()
    {
        UIStateHandler.Instance.GoToMenu();
    }

    public void ResumeGame()
    {
        UIStateHandler.Instance.ResumeGame();  // ← Resume
    }

    public void CargarLevelMenu()
    {
        Time.timeScale = 1f;
        UIStateHandler.Instance.SetState(UIState.LEVEL_MENU);
    }

    public void CargarCredits()
    {
        UIStateHandler.Instance.SetState(UIState.CREDITS);
    }

    public void CargarLevel(int buildIndex)
    {
        UIStateHandler.Instance.LoadLevel(buildIndex);
    }

    public void Reintentar()
    {
        UIStateHandler.Instance.RetryLevel();
    }

    public void Windows()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}