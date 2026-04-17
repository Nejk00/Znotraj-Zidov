using UnityEngine;

public class drawerController : MonoBehaviour
{
    [Header("Drawer Settings")]
    [SerializeField] private float openDistance = 5f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private Axis slideAxis = Axis.X;
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;
    private float targetPosition;
    
    private enum Axis { X, Y, Z }
    
    void Start()
    {
        closedPosition = transform.localPosition;
        
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
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, 
                isOpen ? openPosition : closedPosition, 
                Time.deltaTime * openSpeed
            );
            
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
        
        Debug.Log($"Drawer {(isOpen ? "opened" : "closed")}");
    }
    
    public void OpenDrawer()
    {
        if (!isOpen)
        {
            isOpen = true;
            isMoving = true;
        }
    }
    
    public void CloseDrawer()
    {
        if (isOpen)
        {
            isOpen = false;
            isMoving = true;
        }
    }
}