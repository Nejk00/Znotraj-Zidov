using UnityEngine;

public class drawerController : MonoBehaviour
{
    [Header("Drawer Settings")]
    [SerializeField] private float openDistance = 5f; // How far drawer slides out
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private Axis slideAxis = Axis.X; // Which direction to slide
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;
    private float targetPosition;
    
    private enum Axis { X, Y, Z }
    
    void Start()
    {
        // Store the starting (closed) position
        closedPosition = transform.localPosition;
        
        // Calculate open position based on axis
        openPosition = closedPosition;
        switch (slideAxis)
        {
            case Axis.X:
                openPosition.x += openDistance;
                break;
            case Axis.Y:
                openPosition.y += openDistance;
                break;
            case Axis.Z:
                openPosition.z += openDistance;
                break;
        }
    }
    
    void Update()
    {
        if (isMoving)
        {
            // Smoothly move towards target
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, 
                isOpen ? openPosition : closedPosition, 
                Time.deltaTime * openSpeed
            );
            
            // Check if we've arrived
            if (Vector3.Distance(transform.localPosition, isOpen ? openPosition : closedPosition) < 0.001f)
            {
                transform.localPosition = isOpen ? openPosition : closedPosition;
                isMoving = false;
            }
        }
    }
    
    public void ToggleDrawer()
    {
        isOpen = !isOpen;
        isMoving = true;
        
        // Optional: Add sound effect here
        Debug.Log($"Drawer {(isOpen ? "opened" : "closed")}");
    }
    
    // Public method to open specifically
    public void OpenDrawer()
    {
        if (!isOpen)
        {
            isOpen = true;
            isMoving = true;
        }
    }
    
    // Public method to close specifically
    public void CloseDrawer()
    {
        if (isOpen)
        {
            isOpen = false;
            isMoving = true;
        }
    }
}