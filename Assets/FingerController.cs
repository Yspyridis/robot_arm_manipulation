using UnityEngine;

public class FingerController : MonoBehaviour
{
    public Transform Finger;
    public float rotationAmount = -2f;

    public void RotateFingerLeft()
    {
        Finger.Rotate(Vector2.up, rotationAmount);
    }
    public void RotateFingerRight()
    {
        Finger.Rotate(Vector2.up, rotationAmount*(-1));
    }
}
