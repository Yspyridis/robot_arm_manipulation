using UnityEngine;

public class GripController : MonoBehaviour
{

    public Transform grip;

    public float rotationAmount = -5f;

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    // Update is called once per frame
    void Update()
    {

        if (isRotatingLeft)
        {
            RotateGripLeft();
        }

        if (isRotatingRight)
        {
            RotateGripRight();
        }        
        
    }

    public void RotateGripLeft()
    {
        grip.Rotate(Vector3.right, rotationAmount * Time.deltaTime);
    }

    public void RotateGripRight()
    {
        grip.Rotate(Vector3.right, -rotationAmount * Time.deltaTime);
    }

    public void OnPressGripLeft()
    {
        isRotatingLeft = true;
    }

    public void OnReleaseGripLeft()
    {
        isRotatingLeft = false;
    }

    public void OnPressGripRight()
    {
        isRotatingRight = true;
    }

    public void OnReleaseGripRight()
    {
        isRotatingRight = false;
    }
}
