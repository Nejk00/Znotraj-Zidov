using UnityEngine;

public class standAloneLight : MonoBehaviour
{
    public GameObject lightObject;
    
    public bool lightsAreOn;
    public bool lightsAreOff;
    
    void Start()
    {
        lightsAreOn = false;
        lightsAreOff = true;
        lightObject.SetActive(false);
    }

    public void ToggleLight()
    {
        if (lightsAreOn)
        {
            lightsAreOff = true;
            lightsAreOn = false;
            lightObject.SetActive(false);
        }
        else if (lightsAreOff)
        {
            lightsAreOff = false;
            lightsAreOn = true;
            lightObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


