using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 50f;
    public Transform playerBody;

    private PlayerInputActions inputActions;
    private float xRotation = 0f;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private Vector3 euler;

    void Update() {
        Vector2 look = inputActions.Player.Look.ReadValue<Vector2>();

        euler += new Vector3(-look.y, look.x) * mouseSensitivity *  Time.deltaTime;
        euler.x = Mathf.Clamp(euler.x, -90.0f, 90.0f);

        transform.eulerAngles = euler;
        
    }
}