using System;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private PlayerLook LookScript;
    [SerializeField] private PlayerMove MoveScript;
    [SerializeField] private PlayerRaycast RaycastScript;

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (LookScript != null)
        {
            LookScript.enabled = true;
        }

        if (MoveScript != null)
        {
            MoveScript.enabled = true;
        }

        if (RaycastScript != null)
        {
            RaycastScript.enabled = true;
        }
        
        
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (LookScript != null)
        {
            LookScript.enabled = false;
        }

        if (MoveScript != null)
        {
            MoveScript.enabled = false;
        }

        if (RaycastScript != null)
        {
            RaycastScript.enabled = false;
        }
    }

    void Update()
    {
        
    }
}
