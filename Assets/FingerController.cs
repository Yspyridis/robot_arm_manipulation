using UnityEngine;

public class FingerController : MonoBehaviour
{
    public Transform Finger; 
    public float rotationAmount = -5f;

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    private bool isRotatingUp = false;
    private bool isRotatingDown = false;

    private void Update()
    {
        if (isRotatingLeft)
        {
            RotateFingerLeft();
        }

        if (isRotatingRight)
        {
            RotateFingerRight();
        }

        if (isRotatingUp)
        {
            RotateFingerUp();
        }

        if (isRotatingDown)
        {
            RotateFingerDown();
        }
    }

    public void RotateFingerLeft()
    {
        Finger.Rotate(Vector3.forward, rotationAmount * Time.deltaTime);
    }

    public void RotateFingerRight()
    {
        Finger.Rotate(Vector3.forward, -rotationAmount * Time.deltaTime);
    }

    public void RotateFingerUp()
    {
        Finger.Rotate(Vector3.up, -rotationAmount * Time.deltaTime);
    }

    public void RotateFingerDown()
    {
        Finger.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
    }

    public void OnPressFingerLeft()
    {
        isRotatingLeft = true;
    }

    public void OnReleaseFingerLeft()
    {
        isRotatingLeft = false;
    }

    public void OnPressFingerRight()
    {
        isRotatingRight = true;
    }

    public void OnReleaseFingerRight()
    {
        isRotatingRight = false;
    }

    public void OnPressFingerUp()
    {
        isRotatingUp = true;
    }

    public void OnReleaseFingerUp()
    {
        isRotatingUp = false;
    }

    public void OnPressFingerDown()
    {
        isRotatingDown = true;
    }

    public void OnReleaseFingerDown()
    {
        isRotatingDown = false;
    }
}
