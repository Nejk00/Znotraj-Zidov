using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject onObject;
    public GameObject offObject;
    
    public GameObject lightObject;
    
    public bool lightsAreOn;
    public bool lightsAreOff;
    
    void Start()
    {
        lightsAreOn = false;
        lightsAreOff = true;
        onObject.SetActive(false);
        offObject.SetActive(true);
        lightObject.SetActive(false);
    }

    public void ToggleLight()
    {
        if (lightsAreOn)
        {
            lightsAreOff = true;
            lightsAreOn = false;
            lightObject.SetActive(false);
            onObject.SetActive(false);
            offObject.SetActive(true);
        }
        else if (lightsAreOff)
        {
            lightsAreOff = false;
            lightsAreOn = true;
            lightObject.SetActive(true);
            onObject.SetActive(true);
            offObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
