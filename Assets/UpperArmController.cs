using UnityEngine;

public class UpperArmController : MonoBehaviour
{
    public Transform UpperArm;
    public float rotationAmount = -2f;

    public void RotateUpperArmDown()
    {
        UpperArm.Rotate(Vector3.up, rotationAmount);
    }
    public void RotateUpperArmUp()
    {
        UpperArm.Rotate(Vector3.up, rotationAmount*(-1));
    }
}
