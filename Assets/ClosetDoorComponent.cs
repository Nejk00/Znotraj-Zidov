using System.Collections;
using UnityEngine;

public class ClosetDoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    private bool isOpen = false;
    public bool isLocked;
    public GameObject requiredKey;
    
    [Header("Animation Settings")]
    public float openSpeed = 2f;
    public float openAngle = 90f;
    public bool opensInward = true;
    public bool isLeftHinged = true;
    
    [Header("Double Door Settings")]
    public bool isDoubleDoor = false;
    public ClosetDoorInteraction otherDoor;
    
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine currentAnimation;
    
    void Start()
    {
        closedRotation = transform.localRotation;
        
        // Calculate open rotation based on hinge side and direction
        float actualOpenAngle = openAngle;
        
        if (isLeftHinged)
            actualOpenAngle = opensInward ? openAngle : -openAngle;
        else
            actualOpenAngle = opensInward ? -openAngle : openAngle;
            
        openRotation = Quaternion.Euler(0, actualOpenAngle, 0);
    }
    
    public void OpenClose()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        
        isOpen = !isOpen;
        
        // Handle double doors
        if (isDoubleDoor && otherDoor != null && !otherDoor.IsAnimating())
        {
            otherDoor.OpenClose();
        }
        
        currentAnimation = StartCoroutine(AnimateDoor(isOpen ? openRotation : closedRotation));
    }
    
    IEnumerator AnimateDoor(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.localRotation;
        float elapsed = 0f;
        
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * openSpeed;
            
            transform.localRotation = Quaternion.Lerp(
                startRotation,
                targetRotation,
                elapsed
            );
            
            yield return null;
        }
        
        transform.localRotation = targetRotation;
        currentAnimation = null;
    }
    
    public void TryOpenClose(GameObject heldObject)
    {
        if (isLocked)
        {
            if (heldObject != null && heldObject == requiredKey)
            {
                isLocked = false;
                OpenClose();
                if (requiredKey != null)
                    Destroy(requiredKey);
                Debug.Log("Door unlocked with key!");
            }
            else
            {
                Debug.Log("Door is locked. You need a key!");
                StartCoroutine(PlayLockedJiggle());
            }
        }
        else
        {
            OpenClose();
        }
    }
    
    IEnumerator PlayLockedJiggle()
    {
        Quaternion originalRot = transform.localRotation;
        float elapsed = 0f;
        float jiggleTime = 0.3f;
        
        while (elapsed < jiggleTime)
        {
            float angle = Mathf.Sin(elapsed * 30f) * 3f;
            transform.localRotation = originalRot * Quaternion.Euler(0, angle, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localRotation = originalRot;
    }
    
    public bool IsAnimating()
    {
        return currentAnimation != null;
    }
}