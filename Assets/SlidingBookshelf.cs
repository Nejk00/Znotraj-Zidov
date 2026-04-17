using UnityEngine;
using System.Collections;

public class SlidingBookshelf : MonoBehaviour
{
    [Header("Slide Settings")]
    public float slideDistance = 2f;
    public float slideSpeed = 2f;
    public Vector3 slideDirection = Vector3.right;
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;
    
    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + (slideDirection.normalized * slideDistance);
    }
    
    public void ToggleBookshelf()
    {
        if (isMoving) return;
        
        if (isOpen)
            StartCoroutine(SlideTo(closedPosition));
        else
            StartCoroutine(SlideTo(openPosition));
            
        isOpen = !isOpen;
    }
    
    private IEnumerator SlideTo(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        
        while (elapsedTime < slideSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / slideSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
        isMoving = false;
    }
}