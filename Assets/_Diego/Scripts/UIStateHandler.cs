using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIState
{
    MAIN_MENU,
    LEVEL_MENU,
    CREDITS,
    IN_GAME,
    PAUSED,
    VICTORY,
    LOSE
}

public class UIStateHandler : MonoBehaviour
{
    private static UIStateHandler instance;
    public static UIStateHandler Instance => instance;

    private UIState currentState;
    private bool isGamePaused = false;

    private static int lastLevelBuildIndex = -1;

    [Header("Paneles — Escena Menu")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelLevelMenu;
    [SerializeField] private GameObject panelCredits;

    [Header("Paneles — Escenas Nivel")]
    [SerializeField] private GameObject panelGameUI;
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelVictory;
    [SerializeField] private GameObject panelLose;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Solo hay dos tipos de escena: Menu y nivel
        bool isMenuScene = SceneManager.GetActiveScene().name == "Menu";

        if (isMenuScene)
        {
            currentState = UIState.MAIN_MENU;
        }
        else
        {
            currentState = UIState.IN_GAME;
            SaveLastLevel(SceneManager.GetActiveScene().buildIndex);
        }

        isGamePaused = false;
        UpdateUI();
    }

    // ─── Núcleo ──────────────────────────────────────────────────────────────

    private void UpdateUI()
    {
        bool isMenuState = currentState is UIState.MAIN_MENU
                                        or UIState.LEVEL_MENU
                                        or UIState.CREDITS;

        // Paneles del Menu — solo uno activo a la vez
        SetActive(panelMenu,      currentState == UIState.MAIN_MENU);
        SetActive(panelLevelMenu, currentState == UIState.LEVEL_MENU);
        SetActive(panelCredits,   currentState == UIState.CREDITS);

        // Paneles del Nivel — GameUI siempre visible, los demás se superponen
        SetActive(panelGameUI,  !isMenuState);
        SetActive(panelPause,   currentState == UIState.PAUSED);
        SetActive(panelVictory, currentState == UIState.VICTORY);
        SetActive(panelLose,    currentState == UIState.LOSE);

        Time.timeScale = isGamePaused ? 0f : 1f;
    }

    public void SetState(UIState newState)
    {
        if (currentState is UIState.VICTORY or UIState.LOSE) return;

        isGamePaused = newState == UIState.PAUSED;
        currentState = newState;
        UpdateUI();
    }

    public void SetState(int newState) => SetState((UIState)newState);

    // ─── Guardar / recuperar último nivel ────────────────────────────────────

    private static void SaveLastLevel(int buildIndex)
    {
        lastLevelBuildIndex = buildIndex;
        PlayerPrefs.SetInt("LastLevelBuildIndex", buildIndex);
        PlayerPrefs.Save();
    }

    private static int LoadLastLevel()
    {
        if (lastLevelBuildIndex >= 0) return lastLevelBuildIndex;
        return PlayerPrefs.GetInt("LastLevelBuildIndex", -1);
    }

    // ─── Navegación ──────────────────────────────────────────────────────────

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void LoadLevel(int buildIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    public void RetryLevel()
    {
        int index = LoadLastLevel();
        if (index < 0) index = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
    }

    public void ResumeGame()   => SetState(UIState.IN_GAME);
    public void TriggerVictory() => SetState(UIState.VICTORY);
    public void TriggerLose()    => SetState(UIState.LOSE);

    public void TogglePause()
    {
        if (currentState == UIState.IN_GAME)  SetState(UIState.PAUSED);
        else if (currentState == UIState.PAUSED) SetState(UIState.IN_GAME);
    }

    // ─── Utilidad ─────────────────────────────────────────────────────────────

    private static void SetActive(GameObject go, bool active)
    {
        if (go != null && go.activeSelf != active)
            go.SetActive(active);
    }
}