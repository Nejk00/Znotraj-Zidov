using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 50f;

    private PlayerInputActions inputActions;
    private float xRotation = 0f;
    private Vector3 euler;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            float savedValue = PlayerPrefs.GetFloat("MouseSensitivity");
            // CHANGED: 0.5 to 5 range instead of 20 to 200
            mouseSensitivity = Mathf.Lerp(5f, 30f, savedValue / 100f);
        }
        else
        {
            mouseSensitivity = 2f; // Default medium sensitivity
        }
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Update() 
    {
        Vector2 look = inputActions.Player.Look.ReadValue<Vector2>();

        euler += new Vector3(-look.y, look.x) * mouseSensitivity * Time.deltaTime;
        euler.x = Mathf.Clamp(euler.x, -90.0f, 90.0f);

        transform.eulerAngles = euler;
    }
    
    public void SetSensitivity(float normalizedSensitivity)
    {
        // normalizedSensitivity is 0-1 (0% to 100%)
        // CHANGED: Maps to 0.5 (slow) to 5 (fast)
        mouseSensitivity = Mathf.Lerp(0.5f, 5f, normalizedSensitivity);
        PlayerPrefs.SetFloat("MouseSensitivity", normalizedSensitivity * 100f);
    }
}