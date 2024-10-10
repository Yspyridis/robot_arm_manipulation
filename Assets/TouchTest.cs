using UnityEngine;

public class TouchTest : MonoBehaviour
{
    private Mesh planeMesh;
    private Vector3[] vertices; // To store the vertices

    void Start()
    {
        // Get the Mesh from the plane
        planeMesh = GetComponent<MeshFilter>().mesh;
        vertices = planeMesh.vertices; // Get the vertices once at the start
    }

    // Trigger function for detecting when the robot gripper's trigger touches the plane
    void OnTriggerEnter(Collider other)
    {
        // Print when the object first enters the trigger zone
        Debug.Log("Object entered trigger: " + other.gameObject.name);

        // Get the contact point from the collider
        Vector3 contactPoint = other.ClosestPoint(transform.position);

        // Find and pin the closest vertex
        PinClosestVertex(contactPoint);
    }

    void PinClosestVertex(Vector3 contactPoint)
    {
        int closestVertexIndex = 0;
        float minDistance = Mathf.Infinity;

        // Find the closest vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosVertex = transform.TransformPoint(vertices[i]); // Transform vertex from local to world space
            float distance = Vector3.Distance(contactPoint, worldPosVertex);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestVertexIndex = i;
            }
        }

        // Log the closest vertex that was "pinned"
        Debug.Log("Pinned vertex index: " + closestVertexIndex + " at position: " + vertices[closestVertexIndex]);

        // Move the vertex upwards for visual confirmation
        vertices[closestVertexIndex] += new Vector3(0, 0.5f, 0); // Move the vertex up by 0.5 units

        // Apply the change back to the mesh
        planeMesh.vertices = vertices;
        planeMesh.RecalculateBounds(); // Recalculate bounds so the plane updates its appearance
    }

    // Trigger function for detecting when the robot gripper's trigger leaves the plane
    void OnTriggerExit(Collider other)
    {
        // Print when the object leaves the trigger zone
        Debug.Log("Object exited trigger: " + other.gameObject.name);
    }
}
