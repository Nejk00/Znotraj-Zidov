using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cameraTransform;

    [Header("Audio")]
    public float footstepInterval = 0.5f;
    public float footstepVolume = 0.3f;
    
    private PlayerInputActions inputActions;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float footstepTimer;

    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        controller = GetComponent<CharacterController>();
        footstepTimer = 0f;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }
    
    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    
        // Get camera directions (YOUR ORIGINAL WORKING CODE)
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
    
        // Flatten the directions to the horizontal plane
        cameraForward.y = 0f;
        cameraRight.y = 0f;
    
        // Normalize to maintain consistent speed
        cameraForward.Normalize();
        cameraRight.Normalize();
    
        // Create movement vector using flattened camera directions
        Vector3 move = cameraForward * moveInput.y + cameraRight * moveInput.x;
        
        velocity.y += gravity * Time.deltaTime;
    
        Vector3 finalMove = move * moveSpeed + velocity;
        controller.Move(finalMove * Time.deltaTime);
        
        // Handle footstep sounds
        bool isMoving = moveInput.magnitude > 0.1f && isGrounded;
        
        if (isMoving)
        {
            if (footstepTimer <= 0f)
            {
                SoundManager.Instance?.PlayPlayerFootstep(transform.position, footstepVolume);
                footstepTimer = footstepInterval;
            }
            else
            {
                footstepTimer -= Time.deltaTime;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
}