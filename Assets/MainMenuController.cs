using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "SampleScene";
    
    [Header("UI References")]
    [SerializeField] private GameObject optionsMenuObject;  // Drag Options menu GameObject here
    [SerializeField] private GameObject creditsMenuObject;  // Drag Credits menu GameObject here
    
    [Header("Audio")]
    [SerializeField] private AudioSource hoverSound;       // Optional: creepy sound on hover
    [SerializeField] private AudioSource clickSound;       // Optional: click sound
    
    private UIDocument uiDocument;
    private VisualElement root;
    
    private void OnHorrorHover(MouseEnterEvent evt)
    {
        var button = evt.target as Button;
        if (button != null)
        {
            // Play hover sound if assigned
            if (hoverSound != null)
                hoverSound.Play();
            
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
    
    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("No UIDocument component found!");
            return;
        }
        
        root = uiDocument.rootVisualElement;
        
        // Get all buttons
        Button playButton = root.Q<Button>("PlayButton");
        Button optionsButton = root.Q<Button>("OptionsButton");
        Button creditsButton = root.Q<Button>("CreditsButton");
        Button quitButton = root.Q<Button>("QuitButton");
        
        // Register click events
        if (playButton != null)
            playButton.clicked += OnPlayButtonPressed;
        
        if (optionsButton != null)
            optionsButton.clicked += OnOptionsButtonPressed;
            
        if (creditsButton != null)
            creditsButton.clicked += OnCreditsButtonPressed;
            
        if (quitButton != null)
            quitButton.clicked += OnQuitButtonPressed;
        
        // Register hover events for ALL buttons
        var allButtons = root.Query<Button>().ToList();
        foreach (var button in allButtons)
        {
            button.RegisterCallback<MouseEnterEvent>(OnHorrorHover);
            button.RegisterCallback<MouseLeaveEvent>(OnHorrorLeave);
        }
        
        Debug.Log("Main Menu initialized with all buttons working");
    }
    
    private void OnDisable()
    {
        if (uiDocument == null) return;
        
        if (root == null)
            root = uiDocument.rootVisualElement;
        
        Button playButton = root.Q<Button>("PlayButton");
        Button optionsButton = root.Q<Button>("OptionsButton");
        Button creditsButton = root.Q<Button>("CreditsButton");
        Button quitButton = root.Q<Button>("QuitButton");
        
        if (playButton != null)
            playButton.clicked -= OnPlayButtonPressed;
        
        if (optionsButton != null)
            optionsButton.clicked -= OnOptionsButtonPressed;
            
        if (creditsButton != null)
            creditsButton.clicked -= OnCreditsButtonPressed;
            
        if (quitButton != null)
            quitButton.clicked -= OnQuitButtonPressed;
    }
    
    // ========== BUTTON PRESS HANDLERS ==========
    
    private void OnPlayButtonPressed()
    {
        Debug.Log("Loading game scene...");
        
        // Play click sound if assigned
        if (clickSound != null)
            clickSound.Play();
        
        StartCoroutine(LoadGameScene());
    }
    
    private void OnOptionsButtonPressed()
    {
        gameObject.SetActive(false);
        optionsMenuObject.SetActive(true);
    }
    
    private void OnCreditsButtonPressed()
    {
        Debug.Log("Opening credits...");
        
        // Play click sound if assigned
        if (clickSound != null)
            clickSound.Play();
        
        // Show credits menu if assigned
        if (creditsMenuObject != null)
        {
            // Hide main menu
            gameObject.SetActive(false);
            // Show credits menu
            creditsMenuObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Credits menu object not assigned in MainMenuController!");
            // Alternative: Show simple credits text
            ShowSimpleCredits();
        }
    }
    
    private void OnQuitButtonPressed()
    {
        Debug.Log("Quitting game...");
        
        // Play click sound if assigned
        if (clickSound != null)
            clickSound.Play();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    // ========== SCENE LOADING ==========
    
    private System.Collections.IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = false;
    
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
            yield return null;
        }
        
        // Optional: Add loading screen here
        asyncLoad.allowSceneActivation = true;
    }
    
    // ========== SIMPLE CREDITS (Fallback) ==========
    
    private void ShowSimpleCredits()
    {
        // Create a simple popup credits
        var creditsLabel = new Label();
        creditsLabel.text = "ZNOTRAJ ZIDOV\n\nCreated by: [Your Name]\nSound Design: [Name]\n\nClick anywhere to close";
        creditsLabel.style.position = Position.Absolute;
        creditsLabel.style.top = 0;
        creditsLabel.style.left = 0;
        creditsLabel.style.right = 0;
        creditsLabel.style.bottom = 0;
        creditsLabel.style.backgroundColor = new Color(0, 0, 0, 0.95f);
        creditsLabel.style.color = Color.white;
        creditsLabel.style.alignItems = Align.Center;
        creditsLabel.style.justifyContent = Justify.Center;
        creditsLabel.style.fontSize = 20;
        creditsLabel.style.whiteSpace = WhiteSpace.Normal;
        
        root.Add(creditsLabel);
        
        // Close on click
        creditsLabel.RegisterCallback<ClickEvent>(evt =>
        {
            root.Remove(creditsLabel);
        });
    }
}