using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

public class PauseMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Header("Options Menu")]
    [SerializeField] private GameObject optionsMenuObject;
    
    
    private PlayerInputActions inputActions;
    
    private VisualElement pauseMenuRoot;
    private bool isPaused = false;
    
    // Store original button colors for flicker effect
    private System.Collections.Generic.Dictionary<Button, Color> originalButtonColors = new System.Collections.Generic.Dictionary<Button, Color>();
    
    private void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }
    
    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("No UIDocument component found on Pause Menu!");
            return;
        }
        
        pauseMenuRoot = uiDocument.rootVisualElement;
        
        // Initially hide pause menu (starts hidden)
        pauseMenuRoot.style.display = DisplayStyle.None;
        
        // Get all buttons
        Button resumeButton = pauseMenuRoot.Q<Button>("ResumeButton");
        Button optionsButton = pauseMenuRoot.Q<Button>("OptionsButton");
        Button mainMenuButton = pauseMenuRoot.Q<Button>("MainMenuButton");
        Button quitButton = pauseMenuRoot.Q<Button>("QuitButton");
        
        // Register click events
        if (resumeButton != null)
            resumeButton.clicked += OnResumeClicked;
            
        if (optionsButton != null)
            optionsButton.clicked += OnOptionsClicked;
            
        if (mainMenuButton != null)
            mainMenuButton.clicked += OnMainMenuClicked;
            
        if (quitButton != null)
            quitButton.clicked += OnQuitClicked;
        
        // Register hover events for ALL buttons
        var allButtons = pauseMenuRoot.Query<Button>().ToList();
        foreach (var button in allButtons)
        {
            originalButtonColors[button] = button.style.color.value;
            button.RegisterCallback<MouseEnterEvent>(OnHorrorHover);
            button.RegisterCallback<MouseLeaveEvent>(OnHorrorLeave);
        }
    }
    
    private void Update()
    {
        if (inputActions.Player.Exit.WasPressedThisFrame())
        {
            TogglePause();
        }
    }
    
    private void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuRoot.style.display = DisplayStyle.Flex;
        
        // Show cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Game Paused");
    }
    
    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuRoot.style.display = DisplayStyle.None;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Game Resumed");
    }
    
    // ========== HORROR FLICKER EFFECTS (IDENTICAL TO MAIN MENU) ==========
    
    private void OnHorrorHover(MouseEnterEvent evt)
    {
        var button = evt.target as Button;
        if (button != null)
        {
            StartCoroutine(FlickerButton(button));
        }
    }
    
    private System.Collections.IEnumerator FlickerButton(Button button)
    {
        float elapsed = 0;
        Color originalColor = button.style.color.value;
        
        while (elapsed < 0.3f)
        {
            float intensity = Random.Range(0.7f, 1f);
            button.style.color = new Color(
                originalColor.r * intensity,
                originalColor.g * intensity * 0.5f,
                originalColor.b * intensity * 0.3f
            );
            yield return new WaitForSecondsRealtime(0.05f);
            elapsed += 0.05f;
        }
        
        button.style.color = originalColor;
    }
    
    private void OnHorrorLeave(MouseLeaveEvent evt)
    {
        var button = evt.target as Button;
        if (button != null)
        {
            button.style.color = StyleKeyword.Null;
            button.style.letterSpacing = StyleKeyword.Null;
        }
    }
    
    // ========== BUTTON CLICK HANDLERS ==========
    
    private void OnResumeClicked()
    {
        Debug.Log("Resuming game...");
        ResumeGame();
    }
    
    private void OnOptionsClicked()
    {
        Debug.Log("Opening Options menu from PAUSE menu...");
    
        // Hide pause menu (but remember we came from here)
        gameObject.SetActive(false);
    
        // Show options menu
        if (optionsMenuObject != null)
        {
            optionsMenuObject.SetActive(true);
        
            // Keep time scale at 0 (paused)
            // Don't change Time.timeScale here
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    private void OnMainMenuClicked()
    {
        Debug.Log("Returning to main menu...");
        
        // Resume time before loading new scene
        Time.timeScale = 1f;
        
        // Load main menu scene
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("Main menu scene name not set in PauseMenuController!");
        }
    }
    
    private void OnQuitClicked()
    {
        Debug.Log("Quitting game...");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void OnDisable()
    {
        // Clean up events to prevent memory leaks
        if (pauseMenuRoot == null) return;
        
        var allButtons = pauseMenuRoot.Query<Button>().ToList();
        foreach (var button in allButtons)
        {
            button.UnregisterCallback<MouseEnterEvent>(OnHorrorHover);
            button.UnregisterCallback<MouseLeaveEvent>(OnHorrorLeave);
        }
        
        Button resumeButton = pauseMenuRoot.Q<Button>("ResumeButton");
        Button optionsButton = pauseMenuRoot.Q<Button>("OptionsButton");
        Button mainMenuButton = pauseMenuRoot.Q<Button>("MainMenuButton");
        Button quitButton = pauseMenuRoot.Q<Button>("QuitButton");
        
        if (resumeButton != null)
            resumeButton.clicked -= OnResumeClicked;
        if (optionsButton != null)
            optionsButton.clicked -= OnOptionsClicked;
        if (mainMenuButton != null)
            mainMenuButton.clicked -= OnMainMenuClicked;
        if (quitButton != null)
            quitButton.clicked -= OnQuitClicked;
    }
}
