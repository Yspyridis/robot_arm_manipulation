using UnityEngine;

public class ForeArmController : MonoBehaviour
{
    public Transform ForeArm;
    public float rotationAmount = -2f;

    private bool isRotatingDown = false;
    private bool isRotatingUp = false;

    private void Update()
    {
        if (isRotatingDown)
        {
            RotateForeArmDown();
        }

        if (isRotatingUp)
        {
            RotateForeArmUp();
        }
    }    

    public void RotateForeArmDown()
    {
        ForeArm.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
    }
    public void RotateForeArmUp()
    {
        ForeArm.Rotate(Vector3.up, rotationAmount*(-1) * Time.deltaTime);
    }

    public void OnPressForeArmDown()
    {
        isRotatingDown = true;
    }

    public void OnReleaseForeArmDown()
    {
        isRotatingDown = false;
    }

    public void OnPressForeArmUp()
    {
        isRotatingUp = true;
    }

    public void OnReleaseForeArmUp()
    {
        isRotatingUp = false;
    }
}
