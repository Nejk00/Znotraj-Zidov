using UnityEngine;

public class crouchComponent : MonoBehaviour
{
    public float crouchScale = 0.5f;  // Scale when crouched
    public float standScale = 1f;      // Normal scale
    public float crouchSpeed = 5f;
    
    private bool isCrouching = false;
    private PlayerInputActions inputActions;
    
    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }
    
    void Update()
    {
        if (inputActions.Player.Crouch.WasPressedThisFrame())
        {
            isCrouching = !isCrouching;
        }
        
        float targetScale = isCrouching ? crouchScale : standScale;
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            Vector3.one * targetScale,
            Time.deltaTime * crouchSpeed
        );
    }
}
