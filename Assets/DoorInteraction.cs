using System;
using System.Collections;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")] public bool isOpen = false;
    public bool isLockedByKey;
    public bool isLockedByKeycode;
    public GameObject requiredKey;
    public GameObject enemy = null;
    public GameObject player = null;
    public float enemyOpenDistance;
    public bool AutoClose = false;
    public float AutoCloseDistance;
    
    [Header("Animation Settings")] public float openSpeed = 2f;
    public float openAngle = 90f;

    private Quaternion closedRotation;
    private Coroutine currentAnimation;
    private bool openning = false;

    private void EnemyOpenDoor()
    {
        float distance = Vector3.Distance(enemy.transform.position, transform.position);
        openning = (distance < enemyOpenDistance)? true:false;
        if (openning && !isOpen && !isLockedByKey)
        {
            TryOpenClose(-enemy.transform.rotation.eulerAngles.y, null);
            openning = false;
        } 
    }
    private void Update()
    {
        EnemyOpenDoor();
        AutoCloseDoor();
    }

    void Start()
    {
        closedRotation = transform.parent.localRotation;
    }
    
    public void openClose(float angle)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        
        isOpen = !isOpen;
        Quaternion targetRotation;
        
        if (isOpen)
        {
            targetRotation = Quaternion.Euler(0, angle, 0);
        }
        else
        {
            targetRotation = closedRotation;
        }
        
        currentAnimation = StartCoroutine(AnimateDoor(targetRotation));
    }
    
    IEnumerator AnimateDoor(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.parent.localRotation;
        float elapsed = 0f;
        
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * openSpeed;
            
            transform.parent.localRotation = Quaternion.Lerp(
                startRotation,
                targetRotation,
                elapsed
            );
            
            yield return null;
        }
        
        transform.parent.localRotation = targetRotation;
        currentAnimation = null;
    }
    
    public void TryOpenClose(float playerAngle, GameObject heldObject)
    {
        SoundManager soundManager = FindObjectOfType<SoundManager>();
        float doorAngle = (playerAngle - 180 > 90) ? openAngle : -openAngle;
        
        if (isLockedByKey ||  isLockedByKeycode)
        {
            if (heldObject != null && heldObject == requiredKey)
            {
                isLockedByKey = false;
                if(!isLockedByKeycode)
                    openClose(doorAngle);
                Destroy(requiredKey);
                Debug.Log("Door unlocked with key!");
            }
            else
            {
                Debug.Log("Door is locked. You need a key!");
                StartCoroutine(PlayLockedJiggle());
                soundManager.PlayLockedSound(transform.position);
            }
        }
        else
        {
            openClose(doorAngle);
            soundManager.PlayDoorSound(transform.position);
        }
    }
    
    IEnumerator PlayLockedJiggle()
    {
        Quaternion originalRot = transform.parent.localRotation;
        float elapsed = 0f;
        float jiggleTime = 0.3f;
        
        while (elapsed < jiggleTime)
        {
            float angle = Mathf.Sin(elapsed * 30f) * 5f;
            transform.parent.localRotation = originalRot * Quaternion.Euler(0, angle, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.parent.localRotation = originalRot;
    }

    private void AutoCloseDoor()
    {
        if (AutoClose)
        { 
            float distance = Vector3.Distance(player.transform.position, transform.position);
            bool close = (distance > AutoCloseDistance)? true:false;
            if (close && isOpen)
            {
                TryOpenClose(player.transform.rotation.eulerAngles.y, null);
            }
        }
    }
    
}