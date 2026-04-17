using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;


public class PauseMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";  // Ime tvojega glavnega menija
    
    private UIDocument uiDocument;
    private bool isPaused = false;
    private PlayerInputActions inputActions;
    
    private void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        
        uiDocument = GetComponent<UIDocument>();
        
        // Na začetku skrij pause menu
        uiDocument.enabled = false;
        
        // Odkleni miško (če je bila zaklenjena)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        // Preveri če je pritisnjen ESC
        if (inputActions.Player.Exit.WasPressedThisFrame())
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    private void OnEnable()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();
        
        var root = uiDocument.rootVisualElement;
        
        Button resumeButton = root.Q<Button>("ResumeButton");
        Button optionsButton = root.Q<Button>("OptionsButton");
        Button mainMenuButton = root.Q<Button>("MainMenuButton");
        Button quitButton = root.Q<Button>("QuitButton");
        
        if (resumeButton != null)
            resumeButton.clicked += ResumeGame;
        
        if (optionsButton != null)
            optionsButton.clicked += OpenOptions;
        
        if (mainMenuButton != null)
            mainMenuButton.clicked += GoToMainMenu;
        
        if (quitButton != null)
            quitButton.clicked += QuitGame;
    }
    
    private void OnDisable()
    {
        var root = uiDocument?.rootVisualElement;
        if (root == null) return;
        
        Button resumeButton = root.Q<Button>("ResumeButton");
        Button optionsButton = root.Q<Button>("OptionsButton");
        Button mainMenuButton = root.Q<Button>("MainMenuButton");
        Button quitButton = root.Q<Button>("QuitButton");
        
        if (resumeButton != null)
            resumeButton.clicked -= ResumeGame;
        if (optionsButton != null)
            optionsButton.clicked -= OpenOptions;
        if (mainMenuButton != null)
            mainMenuButton.clicked -= GoToMainMenu;
        if (quitButton != null)
            quitButton.clicked -= QuitGame;
    }
    
    private void PauseGame()
    {
        isPaused = true;
        
        // Zaustavi čas v igri
        Time.timeScale = 0f;
        
        // Pokaži pause menu
        uiDocument.enabled = true;
        
        // Odkleni miško za kazalec
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Game Paused");
    }
    
    private void ResumeGame()
    {
        isPaused = false;
        
        // Nadaljuj čas v igri
        Time.timeScale = 1f;
        
        // Skrij pause menu
        uiDocument.enabled = false;
        
        // Zakleni miško nazaj (če imaš FPS kamero)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Game Resumed");
    }
    
    private void OpenOptions()
    {
        Debug.Log("Opening options...");
        // Tukaj lahko odpreš options menu
        // Lahko narediš še en UI Document za options
    }
    
    private void GoToMainMenu()
    {
        // Najprej nadaljuj čas (pomembno!)
        Time.timeScale = 1f;
        
        // Naloži glavni menu sceno
        SceneManager.LoadScene(mainMenuSceneName);
        
        Debug.Log("Returning to Main Menu...");
    }
    
    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}