using System.Collections;
using UnityEngine;

public class plungerComponent : MonoBehaviour
{
    public void ApplyPlungerForce(GameObject targetObject)
    {
        print(targetObject.name);
        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
    
        if (rb == null)
        {
            // Add rigidbody if it doesn't have one
            rb = targetObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 5f;
        }
    
        // Calculate pull direction (up and slightly toward player)
        Vector3 pullDirection = (Vector3.up * 15f) + (transform.forward * 8f);
    
        // Apply the force
        rb.AddForce(pullDirection, ForceMode.Impulse);
        
        print("plunger force applied");
        
        // Optional: Add a slight delay before gravity fully affects it
        StartCoroutine(TemporaryReducedGravity(rb));
        
    }

    IEnumerator TemporaryReducedGravity(Rigidbody rb)
    {
        float originalGravity = rb.useGravity ? 1f : 0f;
        rb.useGravity = false;
        yield return new WaitForSeconds(0.3f);
        rb.useGravity = true;
    }
}
