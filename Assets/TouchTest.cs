using UnityEngine;

public class TouchTest : MonoBehaviour
{
    // Trigger function for detecting when the robot gripper's trigger touches the plane
    void OnTriggerEnter(Collider other)
    {
        // Print when the object first enters the trigger zone
        Debug.Log("Object entered trigger: " + other.gameObject.name);
    }

    // Trigger function for detecting when the robot gripper's trigger leaves the plane
    void OnTriggerExit(Collider other)
    {
        // Print when the object leaves the trigger zone
        Debug.Log("Object exited trigger: " + other.gameObject.name);
    }
}
