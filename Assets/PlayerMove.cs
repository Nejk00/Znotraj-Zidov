using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float crouchSpeed = 3f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cameraTransform;
    private PlayerInputActions inputActions;
    
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }
    
    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    
        // Get camera directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
    
        // Flatten the directions to the horizontal plane (remove Y component)
        cameraForward.y = 0f;
        cameraRight.y = 0f;
    
        // Normalize to maintain consistent speed (otherwise diagonal movement is faster)
        cameraForward.Normalize();
        cameraRight.Normalize();
    
        // Create movement vector using flattened camera directions
        Vector3 move = cameraForward * moveInput.y + cameraRight * moveInput.x;
    
        // No need to set move.y = 0f anymore since we already removed it
    
        velocity.y += gravity * Time.deltaTime;
    
        Vector3 finalMove = move * moveSpeed + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }
}