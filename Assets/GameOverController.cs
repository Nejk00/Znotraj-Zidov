using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;
using Cursor = UnityEngine.Cursor;

public class GameOverController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    [Header("Audio")]
    [SerializeField] private AudioSource hoverSound;
    [SerializeField] private AudioSource clickSound;
    
    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement gameOverContainer;
    private Label gameOverTitle;
    
    private bool isGameOverActive = false;
    
    // Colors for win/lose states
    private readonly Color loseColor = new Color(0.59f, 0.39f, 0.39f);
    private readonly Color winColor = new Color(0.49f, 0.69f, 0.59f);
    
    // ========== HOVER FLICKER EFFECT (same as main menu) ==========
    
    private void OnHorrorHover(MouseEnterEvent evt)
    {
        var button = evt.target as Button;
        if (button != null)
        {
            if (hoverSound != null)
                hoverSound.Play();
            
            StartCoroutine(FlickerButton(button));
        }
    }
    
    private IEnumerator FlickerButton(Button button)
    {
        float elapsed = 0;
        Color originalColor = button.style.color.value;
    
        // Store original color values
        float originalR = originalColor.r;
        float originalG = originalColor.g;
        float originalB = originalColor.b;
    
        while (elapsed < 0.3f)
        {
            // Intensity range: 0.7 to 1.0 (never goes below 0.7)
            float intensity = Random.Range(0.75f, 1f);
        
            // Make it flicker BRIGHTER or to a RED tint, never darker
            button.style.color = new Color(
                Mathf.Max(originalR, originalR * intensity),      // Red stays strong
                originalG * intensity * 0.7f,                     // Green dims slightly
                originalB * intensity * 0.5f                      // Blue dims more (reddish tint)
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
    
    // ========== TITLE FLICKER (when screen appears) ==========
    
    private IEnumerator FlickerTitle(bool isLose)
    {
        if (gameOverTitle == null) yield break;
        
        float duration = 1.5f;
        float elapsed = 0;
        Color originalColor = gameOverTitle.style.color.value;
        float originalSpacing = 20f;
        
        while (elapsed < duration)
        {
            if (Random.value < 0.4f)
            {
                float intensity = Random.Range(0.3f, 0.9f);
                gameOverTitle.style.color = new Color(
                    originalColor.r * intensity,
                    originalColor.g * intensity * 0.6f,
                    originalColor.b * intensity * 0.4f
                );
                gameOverTitle.style.letterSpacing = originalSpacing + Random.Range(-5f, 10f);
                gameOverTitle.style.opacity = Random.Range(0.5f, 0.95f);
                
                yield return new WaitForSecondsRealtime(0.05f);
                
                gameOverTitle.style.color = originalColor;
                gameOverTitle.style.letterSpacing = originalSpacing;
                gameOverTitle.style.opacity = 0.9f;
            }
            
            elapsed += 0.1f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        
        gameOverTitle.style.color = originalColor;
        gameOverTitle.style.letterSpacing = originalSpacing;
        gameOverTitle.style.opacity = 0.9f;
    }
    
    // ========== TRIGGER FUNCTIONS ==========
    
    public void TriggerLose()
    {
        if (isGameOverActive) return;
        isGameOverActive = true;
        
        if (gameOverTitle != null)
        {
            gameOverTitle.text = "GAME OVER";
            gameOverTitle.style.color = loseColor;
            StartCoroutine(FlickerTitle(true));
        }
        
        ShowGameOver();
    }
    
    public void TriggerWin()
    {
        if (isGameOverActive) return;
        isGameOverActive = true;
        
        if (gameOverTitle != null)
        {
            gameOverTitle.text = "YOU ESCAPED";
            gameOverTitle.style.color = winColor;
            StartCoroutine(FlickerTitle(false));
        }
        
        ShowGameOver();
    }
    
    // ========== UI SETUP ==========
    
    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("No UIDocument component found!");
            return;
        }
        
        root = uiDocument.rootVisualElement;
        gameOverContainer = root.Q<VisualElement>("GameOverContainer");
        gameOverTitle = gameOverContainer?.Q<Label>("GameOverTitle");
        
        // Get buttons
        Button playAgainButton = root.Q<Button>("PlayAgainButton");
        Button mainMenuButton = root.Q<Button>("MainMenuButton");
        
        // Register click events
        if (playAgainButton != null)
            playAgainButton.clicked += OnPlayAgainPressed;
        
        if (mainMenuButton != null)
            mainMenuButton.clicked += OnMainMenuPressed;
        
        // Register hover events for ALL buttons (same as main menu)
        var allButtons = root.Query<Button>().ToList();
        foreach (var button in allButtons)
        {
            button.RegisterCallback<MouseEnterEvent>(OnHorrorHover);
            button.RegisterCallback<MouseLeaveEvent>(OnHorrorLeave);
        }
        
        // Hide at start
        if (gameOverContainer != null)
            gameOverContainer.style.display = DisplayStyle.None;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Game Over Controller initialized");
    }
    
    private void OnDisable()
    {
        if (uiDocument == null) return;
        
        if (root == null)
            root = uiDocument.rootVisualElement;
        
        Button playAgainButton = root.Q<Button>("PlayAgainButton");
        Button mainMenuButton = root.Q<Button>("MainMenuButton");
        
        if (playAgainButton != null)
            playAgainButton.clicked -= OnPlayAgainPressed;
        
        if (mainMenuButton != null)
            mainMenuButton.clicked -= OnMainMenuPressed;
    }
    
    // ========== BUTTON ACTIONS ==========
    
    private void OnPlayAgainPressed()
    {
        Debug.Log("Restarting game...");
        
        if (clickSound != null)
            clickSound.Play();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void OnMainMenuPressed()
    {
        Debug.Log("Going to main menu...");
        
        if (clickSound != null)
            clickSound.Play();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    // ========== UI CONTROL ==========
    
    private void ShowGameOver()
    {
        if (gameOverContainer == null) return;
        
        gameOverContainer.style.display = DisplayStyle.Flex;
        
        if (uiDocument != null)
            uiDocument.sortingOrder = 100;
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log($"Game Over: {gameOverTitle?.text}");
    }
    
    public void HideGameOver()
    {
        if (gameOverContainer == null) return;
        
        isGameOverActive = false;
        gameOverContainer.style.display = DisplayStyle.None;
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}