using UnityEngine;

public class UpperArmController : MonoBehaviour
{
    public Transform UpperArm;
    public float rotationAmount = -2f;

    private bool isRotatingDown = false;
    private bool isRotatingUp = false;

    private void Update()
    {
        if (isRotatingDown)
        {
            RotateUpperArmDown();
        }

        if (isRotatingUp)
        {
            RotateUpperArmUp();
        }
    }

    public void RotateUpperArmDown()
    {
        UpperArm.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
    }
    public void RotateUpperArmUp()
    {
        UpperArm.Rotate(Vector3.up, rotationAmount*(-1) * Time.deltaTime);
    }

    public void OnPressUpperArmDown()
    {
        isRotatingDown = true;
    }

    public void OnReleaseUpperArmDown()
    {
        isRotatingDown = false;
    }

    public void OnPressUpperArmUp()
    {
        isRotatingUp = true;
    }

    public void OnReleaseUpperArmUp()
    {
        isRotatingUp = false;
    }
}
