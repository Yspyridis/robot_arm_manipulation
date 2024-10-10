using System;
using UnityEngine;

public class TouchTest : MonoBehaviour
{
    private Mesh planeMesh;
    private Vector3[] vertices; // Store the vertices of the plane
    private Vector3[] velocities; // Store velocities for each vertex
    private int pinnedVertexIndex = -1; // To store the index of the pinned vertex
    private Transform pinnedTransform; // The transform of the object to follow (gripper)

    public float gravity = -9.81f; // Gravity value
    public float damping = 0.99f; // Damping to slow down motion over time


    public Collider boxCollider; // Reference to the box collider
    
    void Start()
    {
        // Get the Mesh from the plane
        planeMesh = GetComponent<MeshFilter>().mesh;
        vertices = planeMesh.vertices; // Get the vertices once at the start
        velocities = new Vector3[vertices.Length]; // Initialize velocities for each vertex
    }

    void Update()
    {
        // Apply gravity to non-pinned vertices
        ApplyGravityToVertices();

        // If a vertex is pinned, continuously update its position to follow the gripper
        if (pinnedVertexIndex >= 0 && pinnedTransform != null)
        {
            // Convert gripper's world position to local space of the plane
            Vector3 localPoint = transform.InverseTransformPoint(pinnedTransform.position);
            vertices[pinnedVertexIndex] = localPoint;

            // Apply the updated vertex back to the mesh
            planeMesh.vertices = vertices;
            planeMesh.RecalculateBounds();
        }

        // Update the mesh after applying gravity
        planeMesh.vertices = vertices;
        planeMesh.RecalculateBounds();
        planeMesh.RecalculateNormals();
    }

    void ApplyGravityToVertices()
    {
        // Apply gravity only if the vertex is not pinned or touching the box
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i != pinnedVertexIndex)
            {
                Vector3 worldPosVertex = transform.TransformPoint(vertices[i]); // Convert vertex to world space

                // Apply gravity if the vertex is above the box or outside of it
                if (!IsVertexAboveBox(worldPosVertex))
                {
                    velocities[i] += new Vector3(0, gravity * Time.deltaTime, 0); // Add gravity to velocity
                    velocities[i] *= damping; // Apply damping to slow down over time

                    // Update the vertex position
                    vertices[i] += velocities[i] * Time.deltaTime;

                    // Ensure the vertex does not fall below the box collider (bottom of the box)
                    worldPosVertex = transform.TransformPoint(vertices[i]); // Update the world position

                    if (worldPosVertex.y < GetBoxBottomY()) // If the vertex has fallen below the top of the box
                    {
                        // Snap the vertex to the top of the box
                        vertices[i] = transform.InverseTransformPoint(new Vector3(worldPosVertex.x, GetBoxBottomY(), worldPosVertex.z));
                        velocities[i] = Vector3.zero; // Stop further movement
                    }
                }
            }
        }
    }

    // Check if a vertex is directly above the box collider (within the X and Z bounds)
    private bool IsVertexAboveBox(Vector3 vertexWorldPos)
    {
        // Get the bounds of the box collider
        Bounds boxBounds = boxCollider.bounds;

        // Check if the vertex is within the X and Z bounds of the box collider
        bool isWithinXBounds = vertexWorldPos.x >= boxBounds.min.x && vertexWorldPos.x <= boxBounds.max.x;
        bool isWithinZBounds = vertexWorldPos.z >= boxBounds.min.z && vertexWorldPos.z <= boxBounds.max.z;

        // Return true if the vertex is above the box
        return isWithinXBounds && isWithinZBounds;
    }

    // Get the bottom Y position of the box collider (the table's surface height)
    private float GetBoxBottomY()
    {
        return boxCollider.bounds.min.y; // The Y coordinate of the bottom face of the box
    }

    // Trigger function for detecting when the robot gripper's trigger touches the plane
    void OnTriggerEnter(Collider other)
    {
        // Print when the object first enters the trigger zone
        Debug.Log("Object entered trigger: " + other.gameObject.name);

        // Get the contact point from the collider
        Vector3 contactPoint = other.ClosestPoint(transform.position);

        // Find and pin the closest vertex
        PinClosestVertex(contactPoint, other.transform); // Also pass the gripper's transform
    }

    void PinClosestVertex(Vector3 contactPoint, Transform gripperTransform)
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

        // Pin the vertex by saving the index and the gripper's transform to follow
        pinnedVertexIndex = closestVertexIndex;
        pinnedTransform = gripperTransform;
    }

    // Trigger function for detecting when the robot gripper's trigger leaves the plane
    void OnTriggerExit(Collider other)
    {
        // Print when the object leaves the trigger zone
        Debug.Log("Object exited trigger: " + other.gameObject.name);

        // Optionally, reset the pinned vertex when the gripper leaves
        pinnedVertexIndex = -1;
        pinnedTransform = null;
    }
}
