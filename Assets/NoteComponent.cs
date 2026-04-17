using System;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class NoteComponent : MonoBehaviour
{
    public string keycode;
    [SerializeField]private TextMeshProUGUI displayText;
    public GameObject UI;
    private PlayerInputActions inputActions;
    

    private void Awake()
    {
        int code = UnityEngine.Random.Range (100,999);
        keycode = code.ToString();
        displayText.text = keycode;
        
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }
    
    private void Update()
    {
        if (inputActions.Player.Exit.WasPressedThisFrame())
        {
            UI.SetActive(false);
            
        }
    }
}
