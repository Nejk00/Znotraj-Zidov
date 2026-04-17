using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    public GameObject flashLight;
    private bool toggle;
    private PlayerInputActions inputActions;

    void Start()
    {
        flashLight.SetActive(false);
        toggle = false;
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    void toggleFlashLight()
    {
        flashLight.SetActive(toggle);
    }

    void Update()
    {
        if (inputActions.Player.FlashLight.WasPressedThisFrame())
        {
            toggle  = !toggle;
            toggleFlashLight();
        }
    }
}
