using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

public class OptionsController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private UIDocument optionsMenuUIDocument;
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject pauseMenuObject;
    
    [Header("Settings")]
    [SerializeField] private string mouseSensitivitySaveKey = "MouseSensitivity";
    
    private VisualElement optionsRoot;
    private Slider mouseSensitivitySlider;
    private Label mouseSensitivityValue;
    private Slider masterVolumeSlider;
    private Label masterVolumeValue;
    private Slider sfxVolumeSlider;
    private Label sfxVolumeValue;
    private Button saveButton;
    private Button backButton;
    
    private float currentMouseSensitivity = 100f;
    private float currentMasterVolume = 80f;
    private float currentSFXVolume = 80f;
    private bool cameFromPauseMenu = false;
    
    private void OnEnable()
    {
        if (optionsMenuUIDocument == null)
        {
            Debug.LogError("Options Menu UI Document not assigned!");
            return;
        }
        
        optionsRoot = optionsMenuUIDocument.rootVisualElement;
        
        // DETERMINE WHERE WE CAME FROM
        cameFromPauseMenu = false;
        
        if (pauseMenuObject != null && pauseMenuObject.activeInHierarchy)
        {
            cameFromPauseMenu = true;
            Debug.Log("Options opened from Pause Menu (Pause Menu is active)");
        }
        else if (Time.timeScale == 0f)
        {
            cameFromPauseMenu = true;
            Debug.Log("Options opened from Pause Menu (Game is paused)");
        }
        else if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            cameFromPauseMenu = true;
            Debug.Log("Options opened from Gameplay scene (assuming pause context)");
        }
        else
        {
            Debug.Log("Options opened from Main Menu");
        }
        
        // Get UI elements
        mouseSensitivitySlider = optionsRoot.Q<Slider>("MouseSensitivitySlider");
        mouseSensitivityValue = optionsRoot.Q<Label>("MouseSensitivityValue");
        masterVolumeSlider = optionsRoot.Q<Slider>("MasterVolumeSlider");
        masterVolumeValue = optionsRoot.Q<Label>("MasterVolumeValue");
        sfxVolumeSlider = optionsRoot.Q<Slider>("SFXVolumeSlider");
        sfxVolumeValue = optionsRoot.Q<Label>("SFXVolumeValue");
        backButton = optionsRoot.Q<Button>("BackButton");
        
        // Load saved values
        LoadMouseSensitivity();
        LoadVolumeSettings();
        
        // Set up events
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.RegisterValueChangedCallback(OnMouseSensitivityChanged);
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);
        
        if (backButton != null)
            backButton.clicked += OnBackClicked;
        
        // Add hover effects
        var allButtons = optionsRoot.Query<Button>().ToList();
        foreach (var button in allButtons)
        {
            button.RegisterCallback<MouseEnterEvent>(OnHorrorHover);
            button.RegisterCallback<MouseLeaveEvent>(OnHorrorLeave);
        }
        
        // Show cursor for options menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log($"Options menu opened. Came from pause menu: {cameFromPauseMenu}");
    }
    
    private void LoadMouseSensitivity()
    {
        currentMouseSensitivity = PlayerPrefs.GetFloat(mouseSensitivitySaveKey, 100f);
        
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = currentMouseSensitivity;
        
        if (mouseSensitivityValue != null)
            mouseSensitivityValue.text = Mathf.RoundToInt(currentMouseSensitivity) + "%";
        
        // Apply to any existing mouse look script
        ApplyMouseSensitivity(currentMouseSensitivity);
    }
    
    private void LoadVolumeSettings()
    {
        if (VolumeManager.Instance != null)
        {
            currentMasterVolume = VolumeManager.Instance.CurrentMasterVolume;
            currentSFXVolume = VolumeManager.Instance.CurrentSFXVolume;
        }
        else
        {
            currentMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 80f);
            currentSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 80f);
        }
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = currentMasterVolume;
        
        if (masterVolumeValue != null)
            masterVolumeValue.text = Mathf.RoundToInt(currentMasterVolume) + "%";
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = currentSFXVolume;
        
        if (sfxVolumeValue != null)
            sfxVolumeValue.text = Mathf.RoundToInt(currentSFXVolume) + "%";
    }
    
    private void OnMouseSensitivityChanged(ChangeEvent<float> evt)
    {
        currentMouseSensitivity = evt.newValue;
        
        if (mouseSensitivityValue != null)
            mouseSensitivityValue.text = Mathf.RoundToInt(currentMouseSensitivity) + "%";
        
        ApplyMouseSensitivity(currentMouseSensitivity);
    }
    
    private void ApplyMouseSensitivity(float sensitivity)
    {
        // Find any MouseLook script and apply sensitivity
        PlayerLook mouseLook = FindObjectOfType<PlayerLook>();
        if (mouseLook != null)
        {
            float normalizedSensitivity = sensitivity / 100f;
            mouseLook.SetSensitivity(normalizedSensitivity);
        }
        
        // Also find on player camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            PlayerLook cameraMouseLook = mainCamera.GetComponentInParent<PlayerLook>();
            if (cameraMouseLook != null)
            {
                float normalizedSensitivity = sensitivity / 100f;
                cameraMouseLook.SetSensitivity(normalizedSensitivity);
            }
        }
    }
    
    private void OnMasterVolumeChanged(ChangeEvent<float> evt)
    {
        currentMasterVolume = evt.newValue;
        
        if (masterVolumeValue != null)
            masterVolumeValue.text = Mathf.RoundToInt(currentMasterVolume) + "%";
        
        if (VolumeManager.Instance != null)
        {
            VolumeManager.Instance.SetMasterVolume(currentMasterVolume);
        }
    }
    
    private void OnSFXVolumeChanged(ChangeEvent<float> evt)
    {
        currentSFXVolume = evt.newValue;
        
        if (sfxVolumeValue != null)
            sfxVolumeValue.text = Mathf.RoundToInt(currentSFXVolume) + "%";
        
        if (VolumeManager.Instance != null)
        {
            VolumeManager.Instance.SetSFXVolume(currentSFXVolume);
        }
    }
    
    private void OnBackClicked()
    {
        Debug.Log($"Closing options menu. Returning to {(cameFromPauseMenu ? "Pause Menu" : "Main Menu")}");
    
        // Hide options menu
        gameObject.SetActive(false);
    
        // Return to previous menu
        if (cameFromPauseMenu)
        {
            // Return to pause menu
            if (pauseMenuObject != null)
            {
                // Enable the pause menu GameObject
                pauseMenuObject.SetActive(true);
            
                // Get the PauseMenuController and manually call PauseGame to restore state
                PauseMenuController pauseController = pauseMenuObject.GetComponent<PauseMenuController>();
                if (pauseController != null)
                {
                    // Make sure game is still paused and menu is visible
                    pauseController.PauseGame();
                }
            
                // Keep game paused
                Time.timeScale = 0f;
            
                // Show cursor for pause menu
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            
                Debug.Log("Returned to Pause Menu - Game remains paused");
            }
            else
            {
                Debug.LogError("Cannot return to Pause Menu - pauseMenuObject is null!");
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            // Return to main menu
            if (mainMenuObject != null)
            {
                mainMenuObject.SetActive(true);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            
                Debug.Log("Returned to Main Menu");
            }
            else
            {
                Debug.LogError("Cannot return to Main Menu - mainMenuObject is null!");
            }
        }
    }
    
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
        }
    }
    
    private void OnDisable()
    {
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.UnregisterValueChangedCallback(OnMouseSensitivityChanged);
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.UnregisterValueChangedCallback(OnMasterVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.UnregisterValueChangedCallback(OnSFXVolumeChanged);
        
        if (backButton != null)
            backButton.clicked -= OnBackClicked;
    }
}