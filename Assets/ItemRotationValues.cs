using UnityEngine;

public class ItemRotationValues : MonoBehaviour
{
    [Header("Initial Rotation When Picked Up")]
    public float xAngle = 0f;
    public float yAngle = 0f;
    public float zAngle = 0f;

    public void RotateItemToHold(GameObject heldObj)
    {
        // This applies the initial custom rotation only once at pickup
        heldObj.transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle);
        Debug.Log(heldObj.name + " initial rotation set to: " + xAngle + " : " + yAngle + " : " + zAngle);
    }
}