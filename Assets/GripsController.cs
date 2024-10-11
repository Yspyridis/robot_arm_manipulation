using UnityEngine;

public class GripsController : MonoBehaviour
{

    public Transform Grips;

    public float rotationAmount = -2f;

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    public Transform rotationCenter;

    void Update()
    {
        if (isRotatingLeft)
        {
            RotateGripsLeft();
        }

        if (isRotatingRight)
        {
            RotateGripsRight();
        }
        
    }

    public void RotateGripsRight()
    {
        RotateAroundPoint(rotationCenter.position, rotationAmount * Time.deltaTime);
    }


    public void RotateGripsLeft()
    {
        RotateAroundPoint(rotationCenter.position, -rotationAmount * Time.deltaTime);

    }

    public void OnPressGripsLeft()
    {
        isRotatingLeft = true;
    }

    public void OnReleaseGripsLeft()
    {
        isRotatingLeft = false;
    }

    public void OnPressGripsRight()
    {
        isRotatingRight = true;
    }

    public void OnReleaseGripsRight()
    {
        isRotatingRight = false;
    }

    private void RotateAroundPoint(Vector3 point, float angle)
    {
        // Translate the Grips to the pivot point
        Grips.position = Quaternion.AngleAxis(angle, Vector3.up) * (Grips.position - point) + point;

        // Rotate the object itself around its local forward axis if needed (optional)
        Grips.Rotate(Vector3.forward, angle);
    }
}
