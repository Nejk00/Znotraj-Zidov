using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "SampleScene";
    
    
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
    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("No UIDocument component found!");
            return;
        }
        
        var root = uiDocument.rootVisualElement;
        
        Button playButton = root.Q<Button>("PlayButton");
        Button quitButton = root.Q<Button>("QuitButton");
        
        if (playButton != null)
            playButton.clicked += OnPlayButtonPressed;
            
        if (quitButton != null)
            quitButton.clicked += OnQuitButtonPressed;
        var allButtons = root.Query<Button>().ToList();
        foreach (var button in allButtons)
        {
            button.RegisterCallback<MouseEnterEvent>(OnHorrorHover);
            button.RegisterCallback<MouseLeaveEvent>(OnHorrorLeave);
        }
    }
    
    private void OnDisable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        
        var root = uiDocument.rootVisualElement;
        
        Button playButton = root.Q<Button>("PlayButton");
        Button quitButton = root.Q<Button>("QuitButton");
        
        if (playButton != null)
            playButton.clicked -= OnPlayButtonPressed;
        if (quitButton != null)
            quitButton.clicked -= OnQuitButtonPressed;
    }
    
    private void OnPlayButtonPressed()
    {
        Debug.Log("Loading game scene...");
    
        StartCoroutine(LoadGameScene());
    }

    private System.Collections.IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
    
        asyncLoad.allowSceneActivation = false;
    
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
            yield return null;
        }
    
        asyncLoad.allowSceneActivation = true;
    }
    
    private void OnQuitButtonPressed()
    {
        Debug.Log("Quitting game...");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}