using UnityEngine;

public class BaseController : MonoBehaviour
{

    public Transform Base;

    public float rotationAmount = -5f;

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    private void Update()
    {
        if (isRotatingLeft)
        {
            RotateBaseLeft();
        }

        if (isRotatingRight)
        {
            RotateBaseRight();
        }
    }

    public void RotateBaseLeft()
    {
        Base.Rotate(Vector3.forward, rotationAmount * Time.deltaTime);
    }

    public void RotateBaseRight()
    {
        Base.Rotate(Vector3.forward, -rotationAmount * Time.deltaTime);
    }

    public void OnPressBaseLeft()
    {
        isRotatingLeft = true;
    }

    public void OnReleaseBaseLeft()
    {
        isRotatingLeft = false;
    }

    public void OnPressBaseRight()
    {
        isRotatingRight = true;
    }

    public void OnReleaseBaseRight()
    {
        isRotatingRight = false;
    }
}
