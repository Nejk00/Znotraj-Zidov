using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

public class KeycodeComponent : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI displayText;
    [SerializeField]private NoteComponent note;
    [SerializeField]private DoorInteraction Door;
    [SerializeField]public GameObject UI;
    private string code;
    private PlayerInputActions inputActions;

    public void Number(int number)
    {
        if (displayText.text == "INVALID") displayText.text = "";
            displayText.text += number.ToString();
    }

    void Start()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        
        code = note.keycode;
    }
    
    public void Enter()
    {
        if (displayText.text == code)
        {
            displayText.text = "CORRECT";
            Door.isLockedByKeycode = false;
        }
        else{
            displayText.text = "INVALID";
        }
    }

    public void Delete()
    {
        displayText.text = "";
    }
    
    public void Close()
    {
        UI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (UI.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (inputActions.Player.Exit.WasPressedThisFrame())
            {
                Close();
            }
        }
    }
}
