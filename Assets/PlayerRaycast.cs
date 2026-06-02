using System;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;


public class PlayerRaycast : MonoBehaviour
{
    public GameObject crosshair;
    public GameObject keypad;
    public float interactionRange;
    public LayerMask layers;
    public PlayerInputActions inputActions;
    public makeInteractable face;
    public TextMeshProUGUI InteractionPrompt;
    public GameObject leftClick;
    public FlashLight flashLightScript;
    
    private ItemPickUp itemPickUp;
    private string Text = "Press[E] to interact";
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    private void OnDisable()
    {
        crosshair.SetActive(false);
    }

    private void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        
        itemPickUp = GetComponent<ItemPickUp>();
    }

    void Update()
    {
        Text = "Press [E] to interact";
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionRange, layers))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                crosshair.SetActive(true);
                
                if (hit.collider.gameObject.tag == "canPickUp")
                {
                    Text = "";
                    leftClick.SetActive(true);
                }
                InteractionPrompt.text = Text;
                // Check for DoorInteraction component
                DoorInteraction door = hit.collider.gameObject.GetComponent<DoorInteraction>();
                if (door != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        // Pass the currently held object to the door
                        door.TryOpenClose(transform.rotation.eulerAngles.y, itemPickUp.heldObj);
                    }
                }

                LightSwitch lightSwitch = hit.collider.gameObject.GetComponent<LightSwitch>();
                if (lightSwitch != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        lightSwitch.ToggleLight();
                    }
                }

                standAloneLight standAloneLight = hit.collider.gameObject.GetComponent<standAloneLight>();
                if (standAloneLight != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        standAloneLight.ToggleLight();
                    }
                }

                drawerController Drawer = hit.collider.gameObject.GetComponent<drawerController>();
                if (Drawer != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        Drawer.ToggleDrawer();
                    }
                }

                ClosetDoorInteraction closetDoor = hit.collider.gameObject.GetComponent<ClosetDoorInteraction>();
                if (closetDoor != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        closetDoor.TryOpenClose(itemPickUp.heldObj);
                    }
                }

                SlidingBookshelf Bookshelf = hit.collider.gameObject.GetComponent<SlidingBookshelf>();
                if (Bookshelf != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        Bookshelf.ToggleBookshelf();
                    }
                }

                KeycodeComponent keypad = hit.collider.gameObject.GetComponent<KeycodeComponent>();
                if (keypad != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        keypad.UI.SetActive(true);
                    }
                }

                NoteComponent note = hit.collider.gameObject.GetComponent<NoteComponent>();
                if (note != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame() &&
                        itemPickUp.heldObj.gameObject.name == "magnifying_glass")
                    {
                        note.UI.SetActive(true);
                    }
                }

                if (inputActions.Player.Interact.WasPressedThisFrame() && hit.collider.gameObject.tag == "Plank" &&
                    itemPickUp.heldObj.gameObject.name == "hammer")
                {
                    Destroy(hit.collider.gameObject);
                    AnimateHammer hammer = itemPickUp.heldObj.gameObject.GetComponent<AnimateHammer>();
                    hammer.playAnim();
                    
                }

                if (hit.collider.gameObject.transform.parent.gameObject.name != null)
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame() &&
                        hit.collider.gameObject.transform.parent.gameObject.tag == "Screw" &&
                        itemPickUp.heldObj.gameObject.name == "spanner")
                    {
                        face.unScrewed++;
                        Destroy(hit.collider.gameObject.transform.parent.gameObject);
                    }
                }

                if (itemPickUp.heldObj.gameObject.name == "plunger")
                {
                    plungerComponent plunger = itemPickUp.heldObj.gameObject.GetComponent<plungerComponent>();
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        print("plunger");
                        plunger.ApplyPlungerForce(hit.collider.gameObject);
                    }
                }

                if (hit.collider.gameObject.CompareTag("battery"))
                {
                    if (inputActions.Player.Interact.WasPressedThisFrame())
                    {
                        Destroy(hit.collider.gameObject);
                        flashLightScript.enabled = true;
                    }
                }

                    
            }
            else
            {
                crosshair.SetActive(false);
                Text = "Press[E] to interact";
                leftClick.SetActive(false);
            }
        }
        else
        {
            crosshair.SetActive(false);
            Text = "Press[E] to interact";
            leftClick.SetActive(false);
        }
        
    }
}
