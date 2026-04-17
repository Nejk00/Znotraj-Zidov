using UnityEngine;

public class makeInteractable : MonoBehaviour
{
    public int unScrewed = 0;
    private bool turn = false;
    void Start()
    {
        
    }

    
    void Update()
    {
        if (unScrewed == 4 && !turn)
        {
            turn = true;
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            gameObject.tag = "canPickUp";
        }
    }
}
