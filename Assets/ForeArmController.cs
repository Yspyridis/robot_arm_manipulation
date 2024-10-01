using UnityEngine;

public class ForeArmController : MonoBehaviour
{
    public Transform ForeArm;
    public float rotationAmount = -2f;

    public void RotateForeArmDown()
    {
        ForeArm.Rotate(Vector3.up, rotationAmount);
    }
    public void RotateForeArmUp()
    {
        ForeArm.Rotate(Vector3.up, rotationAmount*(-1));
    }
}
