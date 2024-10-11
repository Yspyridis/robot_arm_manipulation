using UnityEngine;
using System.Collections.Generic;

public class TouchTest : MonoBehaviour
{
    private Mesh planeMesh;
    private Vector3[] vertices; // Store the vertices of the plane
    private Vector3[] velocities; // Store velocities for each vertex
    private Dictionary<int, List<int>> vertexNeighbors; // Store neighboring vertices for each vertex
    private int pinnedVertexIndex = -1; // To store the index of the pinned vertex
    private Transform pinnedTransform; // The transform of the object to follow (gripper)

    public float gravity = -9.81f; // Gravity value
    public float damping = 0.95f; // Damping to slow down motion over time
    public float springK = 0.5f; // Spring constant for spring behavior between neighbors
    public float maxVelocity = 0.5f; // Cap the maximum velocity

    public Collider boxCollider; // Reference to the box collider

    void Start()
    {
        // Get the Mesh from the plane
        planeMesh = GetComponent<MeshFilter>().mesh;
        vertices = planeMesh.vertices; // Get the vertices once at the start
        velocities = new Vector3[vertices.Length]; // Initialize velocities for each vertex
        vertexNeighbors = FindVertexNeighbors(); // Build the neighbor map
    }

    void Update()
    {
        // Apply gravity and spring forces to vertices
        ApplyGravityAndSpringForces();

        // If a vertex is pinned, continuously update its position to follow the gripper
        if (pinnedVertexIndex >= 0 && pinnedTransform != null)
        {
            // Convert gripper's world position to local space of the plane
            Vector3 localPoint = transform.InverseTransformPoint(pinnedTransform.position);
            vertices[pinnedVertexIndex] = localPoint;
        }

        // Update the mesh after applying forces
        planeMesh.vertices = vertices;
        planeMesh.RecalculateBounds();
        planeMesh.RecalculateNormals();
    }

    void ApplyGravityAndSpringForces()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i != pinnedVertexIndex)
            {
                Vector3 worldPosVertex = transform.TransformPoint(vertices[i]); // Convert vertex to world space

                // Only apply forces to vertices not above the box collider
                if (!IsVertexAboveBox(worldPosVertex))
                {
                    // Apply gravity to all non-pinned, non-box vertices
                    velocities[i] += new Vector3(0, gravity * Time.deltaTime, 0); // Add gravity to velocity
                    velocities[i] *= damping; // Apply damping to slow down motion

                    // Apply spring force between the vertex and its neighbors
                    foreach (int neighborIndex in vertexNeighbors[i])
                    {
                        ApplySpringForce(i, neighborIndex);
                    }

                    // Cap the velocity to prevent extreme forces
                    velocities[i] = Vector3.ClampMagnitude(velocities[i], maxVelocity);

                    // Update the vertex position
                    vertices[i] += velocities[i] * Time.deltaTime;
                }
            }
        }
    }

    // Apply Hooke's law spring force between two neighboring vertices
    void ApplySpringForce(int vertexIndex, int neighborIndex)
    {
        Vector3 vertexPos = vertices[vertexIndex];
        Vector3 neighborPos = vertices[neighborIndex];

        // Calculate the displacement between the vertices
        Vector3 displacement = vertexPos - neighborPos;

        // Calculate the spring force: F = -k * x
        Vector3 springForce = -springK * displacement;

        // Apply the spring force to the velocity
        velocities[vertexIndex] += springForce * Time.deltaTime;
    }

    // Find the neighboring vertices in the mesh
    Dictionary<int, List<int>> FindVertexNeighbors()
    {
        Dictionary<int, List<int>> neighbors = new Dictionary<int, List<int>>();

        int[] triangles = planeMesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            AddNeighbor(neighbors, v0, v1);
            AddNeighbor(neighbors, v0, v2);
            AddNeighbor(neighbors, v1, v0);
            AddNeighbor(neighbors, v1, v2);
            AddNeighbor(neighbors, v2, v0);
            AddNeighbor(neighbors, v2, v1);
        }

        return neighbors;
    }

    // Helper function to add a neighbor to a vertex
    void AddNeighbor(Dictionary<int, List<int>> neighbors, int vertex, int neighbor)
    {
        if (!neighbors.ContainsKey(vertex))
        {
            neighbors[vertex] = new List<int>();
        }

        if (!neighbors[vertex].Contains(neighbor))
        {
            neighbors[vertex].Add(neighbor);
        }
    }

    // Check if a vertex is directly above the box collider (within the X and Z bounds)
    private bool IsVertexAboveBox(Vector3 vertexWorldPos)
    {
        Bounds boxBounds = boxCollider.bounds;

        bool isWithinXBounds = vertexWorldPos.x >= boxBounds.min.x && vertexWorldPos.x <= boxBounds.max.x;
        bool isWithinZBounds = vertexWorldPos.z >= boxBounds.min.z && vertexWorldPos.z <= boxBounds.max.z;

        return isWithinXBounds && isWithinZBounds;
    }

    // Trigger function for detecting when the robot gripper's trigger touches the plane
    void OnTriggerEnter(Collider other)
    {
        // Print when the object first enters the trigger zone
        Debug.Log("Object entered trigger: " + other.gameObject.name);

        // Get the contact point from the collider
        Vector3 contactPoint = other.ClosestPoint(transform.position);

        // Find and pin the closest vertex
        PinClosestVertex(contactPoint, other.transform);
    }

    void PinClosestVertex(Vector3 contactPoint, Transform gripperTransform)
    {
        int closestVertexIndex = 0;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosVertex = transform.TransformPoint(vertices[i]);
            float distance = Vector3.Distance(contactPoint, worldPosVertex);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestVertexIndex = i;
            }
        }

        // Pin the closest vertex and make it follow the gripper's movement
        pinnedVertexIndex = closestVertexIndex;
        pinnedTransform = gripperTransform;
    }

    // Trigger function for detecting when the robot gripper's trigger leaves the plane
    void OnTriggerExit(Collider other)
    {
        // Reset the pinned vertex when the gripper leaves
        pinnedVertexIndex = -1;
        pinnedTransform = null;
    }
}
