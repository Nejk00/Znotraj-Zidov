using UnityEngine;
using System.Collections;

public class HammerHit : MonoBehaviour
{
    public enum RotationAxis { Right, Up, Forward, NegativeRight, NegativeUp, NegativeForward }

    [Header("Nastavitve udarca")]
    public float hitDuration = 0.3f;
    public float returnDuration = 0.2f;
    public float hitAngle = 90f;
    public RotationAxis localAxis = RotationAxis.Forward; // <-- izberi tukaj
    public AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isHitting = false;

    private Vector3 GetAxisVector()
    {
        switch (localAxis)
        {
            case RotationAxis.Right: return Vector3.right;
            case RotationAxis.Up: return Vector3.up;
            case RotationAxis.Forward: return Vector3.forward;
            case RotationAxis.NegativeRight: return -Vector3.right;
            case RotationAxis.NegativeUp: return -Vector3.up;
            case RotationAxis.NegativeForward: return -Vector3.forward;
            default: return Vector3.right;
        }
    }

    public void HitOnce()
    {
        if (!isHitting) StartCoroutine(DoHit());
    }

    IEnumerator DoHit()
    {
        isHitting = true;

        Quaternion startRot = transform.localRotation;
        Vector3 axis = GetAxisVector(); // uporabi izbrano os
        Quaternion hitRot = startRot * Quaternion.AngleAxis(hitAngle, axis);
        hitRot = hitRot * Quaternion.AngleAxis(180f, axis);

        // Spust
        float elapsed = 0f;
        while (elapsed < hitDuration)
        {
            float t = elapsed / hitDuration;
            float eased = easingCurve.Evaluate(t);
            transform.localRotation = Quaternion.Slerp(startRot, hitRot, eased);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = hitRot;

        // Dvig
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            float eased = easingCurve.Evaluate(t);
            transform.localRotation = Quaternion.Slerp(hitRot, startRot, eased);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = startRot;

        isHitting = false;
    }
}