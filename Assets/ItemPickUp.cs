using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemPickUp : MonoBehaviour
{
    public GameObject player;
    public Transform rightHoldPos;  // For normal items
    public Transform leftHoldPos;   // For flashlight
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    public GameObject heldObj;      // Right hand item
    public GameObject heldFlashlight; // Left hand flashlight

    public GameObject InteractionInfo;
    
    private Rigidbody heldObjRb;
    private Rigidbody flashlightRb;
    private bool canDrop = true;
    private int LayerNumber;
    
    public float holdSmoothTime = 0.1f;
    private Vector3 holdVelocity = Vector3.zero;
    private Vector3 flashlightHoldVelocity = Vector3.zero;
    
    public float clipCheckRadius = 0.2f;
    public LayerMask clipCheckMask = ~0;
    public float autoAdjustSpeed = 5f;
    
    public PlayerInputActions inputActions;
    PlayerLook playerLookScript;
    
    private ItemRotationValues itemRotationValues;
    private bool hasFlashlight = false;
    
    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        playerLookScript = player.GetComponent<PlayerLook>();
    }
    
    void Update()
    {
        // Pick up items
        if (inputActions.Player.PickUp.WasPressedThisFrame())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
            {
                // Check for flashlight
                if (hit.transform.gameObject.GetComponent<FlashLight>() != null && !hasFlashlight)
                {
                    PickUpFlashlight(hit.transform.gameObject);
                }
                // Check for regular pickup
                else if (hit.transform.gameObject.tag == "canPickUp" && heldObj == null)
                {
                    PickUpObject(hit.transform.gameObject);
                }
            }
            // Drop right hand item if holding something
            else if (heldObj != null && canDrop == true)
            {
                StopClipping();
                DropObject();
            }
        }
        
        // Throw right hand item
        if (heldObj != null)
        {
            InteractionInfo.SetActive(true);
            MoveObject();
            
            if (inputActions.Player.Throw.WasPressedThisFrame() && canDrop == true)
            {
                StopClipping();
                ThrowObject();
            }
        }
        else
        {
            InteractionInfo.SetActive(false);
        }
        
        // Update flashlight position if we have one
        if (heldFlashlight != null)
        {
            MoveFlashlight();
        }
    }
    
    void PickUpObject(GameObject pickUpObj)
    {
        print("picked up " + pickUpObj.name);
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            
            heldObj.transform.parent = rightHoldPos;
            heldObj.transform.localPosition = Vector3.zero;
            
            itemRotationValues = heldObj.GetComponent<ItemRotationValues>();
            heldObj.transform.localRotation = Quaternion.identity;
            
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    
    void PickUpFlashlight(GameObject flashlight)
    {
        print("Picked up flashlight - permanently held in left hand");
        if (flashlight.GetComponent<Rigidbody>())
        {
            heldFlashlight = flashlight;
            flashlightRb = flashlight.GetComponent<Rigidbody>();
            flashlightRb.isKinematic = true;
            flashlightRb.useGravity = false;
        
            heldFlashlight.transform.parent = leftHoldPos;
            heldFlashlight.transform.localPosition = Vector3.zero;
            heldFlashlight.transform.localRotation = Quaternion.identity;
        
            // Set layer for parent AND all children
            SetLayerRecursively(heldFlashlight, LayerNumber);
        
            // Ignore collisions with player for all parts
            Collider[] allColliders = heldFlashlight.GetComponentsInChildren<Collider>();
            foreach (Collider col in allColliders)
            {
                Physics.IgnoreCollision(col, player.GetComponent<Collider>(), true);
            }
        
            // Also ignore collisions with right-hand item if exists
            if (heldObj != null)
            {
                Collider[] heldObjColliders = heldObj.GetComponentsInChildren<Collider>();
                foreach (Collider flashlightCol in allColliders)
                {
                    foreach (Collider heldObjCol in heldObjColliders)
                    {
                        Physics.IgnoreCollision(flashlightCol, heldObjCol, true);
                    }
                }
            }
        
            // Enable flashlight component
            FlashLight flashlightScript = heldFlashlight.GetComponent<FlashLight>();
            if (flashlightScript != null)
            {
                flashlightScript.enabled = true;
            }
        
            hasFlashlight = true;
        }
    }

// Helper method to set layer for object and all its children
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    
    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = LayerMask.NameToLayer("Interactable");
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
        itemRotationValues = null;
    }
    
    void MoveObject()
    {
        if (IsPositionClipping(rightHoldPos.position, heldObj))
        {
            Vector3 safePos = FindSafePosition(rightHoldPos.position, heldObj);
            heldObj.transform.position = Vector3.SmoothDamp(
                heldObj.transform.position, 
                safePos, 
                ref holdVelocity, 
                holdSmoothTime
            );
            return;
        }
        
        heldObj.transform.position = Vector3.SmoothDamp(
            heldObj.transform.position, 
            rightHoldPos.position, 
            ref holdVelocity,
            holdSmoothTime
        );
        
        // Apply custom rotation relative to the hold position
        if (itemRotationValues != null)
        {
            heldObj.transform.rotation = rightHoldPos.rotation * Quaternion.Euler(itemRotationValues.xAngle, itemRotationValues.yAngle, itemRotationValues.zAngle);
        }
        else
        {
            heldObj.transform.rotation = rightHoldPos.rotation;
        }
    }
    
    void MoveFlashlight()
    {
        // Check for clipping
        if (IsFlashlightPositionClipping(leftHoldPos.position))
        {
            Vector3 safePos = FindSafeFlashlightPosition(leftHoldPos.position);
            heldFlashlight.transform.position = Vector3.SmoothDamp(
                heldFlashlight.transform.position, 
                safePos, 
                ref flashlightHoldVelocity,
                holdSmoothTime
            );
            return;
        }
    
        // Smooth movement for flashlight
        heldFlashlight.transform.position = Vector3.SmoothDamp(
            heldFlashlight.transform.position, 
            leftHoldPos.position, 
            ref flashlightHoldVelocity,
            holdSmoothTime
        );
    
        heldFlashlight.transform.rotation = leftHoldPos.rotation;
    }

    bool IsFlashlightPositionClipping(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, clipCheckRadius, clipCheckMask);
    
        foreach (Collider col in colliders)
        {
            if (col.gameObject != heldFlashlight && col.gameObject != player)
            {
                return true;
            }
        }
        return false;
    }

    Vector3 FindSafeFlashlightPosition(Vector3 targetPos)
    {
        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            transform.forward * 0.3f,
            transform.forward * -0.2f,
            Vector3.up * 0.2f,
            Vector3.down * 0.2f,
            Vector3.right * 0.2f,
            Vector3.left * 0.2f
        };
    
        foreach (Vector3 offset in offsets)
        {
            Vector3 testPos = targetPos + offset;
            if (!IsFlashlightPositionClipping(testPos))
            {
                return testPos;
            }
        }
    
        return targetPos + (transform.forward * 0.3f);
    }
    
    bool IsPositionClipping(Vector3 position, GameObject obj)
    {
        Collider[] colliders = Physics.OverlapSphere(position, clipCheckRadius, clipCheckMask);
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject != obj && col.gameObject != player)
            {
                return true;
            }
        }
        return false;
    }
    
    Vector3 FindSafePosition(Vector3 targetPos, GameObject obj)
    {
        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            transform.forward * 0.3f,
            transform.forward * -0.2f,
            Vector3.up * 0.2f,
            Vector3.down * 0.2f
        };
        
        foreach (Vector3 offset in offsets)
        {
            Vector3 testPos = targetPos + offset;
            if (!IsPositionClipping(testPos, obj))
            {
                return testPos;
            }
        }
        
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
        itemRotationValues = null;
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