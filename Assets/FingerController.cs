using UnityEngine;

public class FingerController : MonoBehaviour
{
    public Transform Finger; 
    public float rotationAmount = -5f;

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

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
    }

    public void RotateFingerLeft()
    {
        Finger.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
    }

    public void RotateFingerRight()
    {
        Finger.Rotate(Vector3.up, -rotationAmount * Time.deltaTime);
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
}
