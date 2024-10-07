using UnityEngine;

public class GripController : MonoBehaviour
{

    public Transform grip;

    public float rotationAmount = -5f;

    public float returnSpeed = 5f; // speed of grip returning to original position

    private Quaternion originalRotation; //stores the original rotation of the grip

    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    private bool isreturning = false;

    void Start()
    {
        originalRotation = grip.rotation;
    }

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
        else if (!isRotatingLeft && !isRotatingRight)
        {
            ReturnGripToOriginalPosition();
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

    private void ReturnGripToOriginalPosition()
    {
        grip.rotation = Quaternion.Slerp(grip.rotation, originalRotation, returnSpeed * Time.deltaTime);

        if (Quaternion.Angle(grip.rotation, originalRotation) < 1)
        {
            grip.rotation = originalRotation;
            isreturning = false;
        }
    }

    public void OnPressGripLeft()
    {
        isRotatingLeft = true;
        isreturning = false;
    }

    public void OnReleaseGripLeft()
    {
        isRotatingLeft = false;
        isreturning = true;
    }

    public void OnPressGripRight()
    {
        isRotatingRight = true;
        isreturning = false;
    }

    public void OnReleaseGripRight()
    {
        isRotatingRight = false;
        isreturning = true;
    }
}
