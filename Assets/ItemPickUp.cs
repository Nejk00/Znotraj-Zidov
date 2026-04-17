using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos; // Keep this for backward compatibility
    public Transform rightHandHold;  // New: position for right-hand hold
    public Transform leftHandHold;   // New: position for left-hand hold
    public Transform backHold;       // New: position for back/carrying
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    public GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;
    
    // New: Which hold style to use
    public enum HoldStyle
    {
        RightHand,
        LeftHand,
        Back,
        Custom // Uses original holdPos
    }
    
    public HoldStyle currentHoldStyle = HoldStyle.RightHand;
    private HoldStyle lastHoldStyle;
    
    // New: Smoothing for hold position transitions
    public float holdSmoothTime = 0.1f;
    private Vector3 holdVelocity = Vector3.zero;
    
    // New: Clipping prevention settings
    public float clipCheckRadius = 0.2f;
    public LayerMask clipCheckMask = ~0; // Everything by default
    public float autoAdjustSpeed = 5f;
    
    public PlayerInputActions inputActions;
    PlayerLook playerLookScript;
    
    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        playerLookScript = player.GetComponent<PlayerLook>();
        
        // If custom holds aren't assigned, fall back to holdPos
        if (rightHandHold == null) rightHandHold = holdPos;
        if (leftHandHold == null) leftHandHold = holdPos;
        if (backHold == null) backHold = holdPos;
    }
    
    void Update()
    {
        if (inputActions.Player.PickUp.WasPressedThisFrame())
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if(canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
        
        if (heldObj != null)
        {
            // Check if we need to change hold style
            if (lastHoldStyle != currentHoldStyle)
            {
                lastHoldStyle = currentHoldStyle;
                // Reparent to new hold position
                heldObj.transform.parent = GetCurrentHoldTransform();
            }
            
            MoveObject();
            
            if (inputActions.Player.Throw.WasPressedThisFrame() && canDrop == true)
            {
                StopClipping();
                ThrowObject();
            }
        }
    }
    
    // New: Get the current hold transform based on style
    Transform GetCurrentHoldTransform()
    {
        switch (currentHoldStyle)
        {
            case HoldStyle.RightHand:
                return rightHandHold;
            case HoldStyle.LeftHand:
                return leftHandHold;
            case HoldStyle.Back:
                return backHold;
            case HoldStyle.Custom:
            default:
                return holdPos;
        }
    }
    
    // New: Cycle through hold styles
    void CycleHoldStyle()
    {
        int nextStyle = ((int)currentHoldStyle + 1) % System.Enum.GetValues(typeof(HoldStyle)).Length;
        currentHoldStyle = (HoldStyle)nextStyle;
    }
    
    void PickUpObject(GameObject pickUpObj)
    {
        print("picked up " + pickUpObj.name);
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            
            // Parent to current hold transform
            Transform targetHold = GetCurrentHoldTransform();
            heldObj.transform.parent = targetHold;
            
            // Reset local position and rotation for clean attachment
            heldObj.transform.localPosition = Vector3.zero;
            heldObj.transform.localRotation = Quaternion.identity;
            
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            
            lastHoldStyle = currentHoldStyle;
        }
    }
    
    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = LayerMask.NameToLayer("Interactable");
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
    }
    
    void MoveObject()
    {
        Transform targetHold = GetCurrentHoldTransform();
        
        // New: Check for clipping at the hold position
        if (IsPositionClipping(targetHold.position))
        {
            // If clipping, either adjust or switch to back hold automatically
            if (currentHoldStyle != HoldStyle.Back)
            {
                // Auto-switch to back carry to prevent clipping
                currentHoldStyle = HoldStyle.Back;
                heldObj.transform.parent = backHold;
            }
            else
            {
                // Even back is clipping? Try to push object forward slightly
                Vector3 safePos = FindSafePosition(targetHold.position);
                heldObj.transform.position = Vector3.SmoothDamp(
                    heldObj.transform.position, 
                    safePos, 
                    ref holdVelocity, 
                    holdSmoothTime
                );
                return;
            }
        }
        
        // Smoothly move to hold position
        heldObj.transform.position = Vector3.SmoothDamp(
            heldObj.transform.position, 
            targetHold.position, 
            ref holdVelocity, 
            holdSmoothTime
        );
        
        // Match rotation of hold transform
        heldObj.transform.rotation = targetHold.rotation;
    }
    
    // New: Check if a position is clipping through geometry
    bool IsPositionClipping(Vector3 position)
    {
        // Check sphere overlap at the position
        Collider[] colliders = Physics.OverlapSphere(position, clipCheckRadius, clipCheckMask);
        
        foreach (Collider col in colliders)
        {
            // Ignore the held object and the player
            if (col.gameObject != heldObj && col.gameObject != player)
            {
                return true; // Clipping detected
            }
        }
        return false;
    }
    
    // New: Find a safe position near the target
    Vector3 FindSafePosition(Vector3 targetPos)
    {
        // Try positions along camera forward direction
        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            transform.forward * 0.3f,  // Push forward
            transform.forward * -0.2f, // Pull back
            Vector3.up * 0.2f,          // Lift up
            Vector3.down * 0.2f          // Lower down
        };
        
        foreach (Vector3 offset in offsets)
        {
            Vector3 testPos = targetPos + offset;
            if (!IsPositionClipping(testPos))
            {
                return testPos;
            }
        }
        
        // If all positions clip, return original + slight forward push
        return targetPos + (transform.forward * 0.3f);
    }
    
    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = LayerMask.NameToLayer("Interactable");
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }
    
    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}