using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    public GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;
    
    public float holdSmoothTime = 0.1f;
    private Vector3 holdVelocity = Vector3.zero;
    
    public float clipCheckRadius = 0.2f;
    public LayerMask clipCheckMask = ~0;
    public float autoAdjustSpeed = 5f;
    
    public PlayerInputActions inputActions;
    PlayerLook playerLookScript;
    
    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        playerLookScript = player.GetComponent<PlayerLook>();
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
            MoveObject();
            
            if (inputActions.Player.Throw.WasPressedThisFrame() && canDrop == true)
            {
                StopClipping();
                ThrowObject();
            }
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
            
            heldObj.transform.parent = holdPos;
            
            heldObj.transform.localPosition = Vector3.zero;
            heldObj.transform.localRotation = Quaternion.identity;
            
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
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
        if (IsPositionClipping(holdPos.position))
        {
            Vector3 safePos = FindSafePosition(holdPos.position);
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
            holdPos.position, 
            ref holdVelocity, 
            holdSmoothTime
        );
        
        heldObj.transform.rotation = holdPos.rotation;
    }
    
    bool IsPositionClipping(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, clipCheckRadius, clipCheckMask);
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject != heldObj && col.gameObject != player)
            {
                return true;
            }
        }
        return false;
    }
    
    Vector3 FindSafePosition(Vector3 targetPos)
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
            if (!IsPositionClipping(testPos))
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