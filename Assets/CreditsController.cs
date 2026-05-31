using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CreditsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument creditsUIDocument;
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject pauseMenuObject;
    
    private Button backButton;
    private bool cameFromPauseMenu = false;
    
    private void OnEnable()
    {
        if (creditsUIDocument == null) return;
        
        var root = creditsUIDocument.rootVisualElement;
        backButton = root.Q<Button>("BackButton");
        
        // Check where we came from
        cameFromPauseMenu = (pauseMenuObject != null && pauseMenuObject.activeSelf);
        
        if (backButton != null)
            backButton.clicked += OnBackClicked;
        
        // Add hover effect
        if (backButton != null)
        {
            backButton.RegisterCallback<MouseEnterEvent>(OnHorrorHover);
            backButton.RegisterCallback<MouseLeaveEvent>(OnHorrorLeave);
        }
        
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void OnHorrorHover(MouseEnterEvent evt)
    {
        var button = evt.target as Button;
        if (button != null)
            StartCoroutine(FlickerButton(button));
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
            button.style.color = StyleKeyword.Null;
    }
    
    private void OnBackClicked()
    {
        gameObject.SetActive(false);
        
        if (cameFromPauseMenu && pauseMenuObject != null)
        {
            pauseMenuObject.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (mainMenuObject != null)
        {
            mainMenuObject.SetActive(true);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    private void OnDisable()
    {
        if (backButton != null)
            backButton.clicked -= OnBackClicked;
    }
}