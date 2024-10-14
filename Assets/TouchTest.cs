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

    private Vector3[] originalVertices; // Store the original positions of the vertices

    public float gravity = -9.81f; // Gravity value
    public float damping = 0.98f; // Adjusted damping to avoid too much slowdown
    public float springK = 2.0f; // Increased spring constant for stronger coupling between vertices
    public float maxVelocity = 0.75f; // Increased max velocity to allow for more responsive motion

    public Collider boxCollider; // Reference to the box collider

    private float minDistance;

    void Start()
    {
        // Get the Mesh from the plane
        planeMesh = GetComponent<MeshFilter>().mesh;
        vertices = planeMesh.vertices; // Get the vertices once at the start
        originalVertices = planeMesh.vertices.Clone() as Vector3[]; // Store the original vertex positions
        velocities = new Vector3[vertices.Length]; // Initialize velocities for each vertex
        vertexNeighbors = FindVertexNeighbors(); // Build the neighbor map
    }

    void FixedUpdate()  // Use FixedUpdate for consistent physics updates
    {
        // Apply gravity and spring forces to vertices
        ApplyGravityAndSpringForces();

        // If a vertex is pinned, continuously update its position to follow the gripper
        if (pinnedVertexIndex >= 0 && pinnedTransform != null)
        {
            UpdatePinnedVertex();  // Only update the pinned vertex
        }

        // Update the mesh after applying forces
        planeMesh.vertices = vertices;
        planeMesh.RecalculateBounds();
        planeMesh.RecalculateNormals();
    }

    // Update the pinned vertex to follow the gripper's movement
    void UpdatePinnedVertex()
    {
        // Convert gripper's world position to local space of the plane
        Vector3 localPoint = transform.InverseTransformPoint(pinnedTransform.position);

        // Calculate velocity of the pinned vertex based on its movement
        Vector3 velocityOfPinned = (localPoint - vertices[pinnedVertexIndex]) / Time.deltaTime;

        // Set the pinned vertex to follow the gripper's local position
        velocities[pinnedVertexIndex] = velocityOfPinned; // Update velocity
        vertices[pinnedVertexIndex] = localPoint; // Update the position

        // Apply spring forces on neighboring vertices of the pinned vertex
        foreach (int neighborIndex in vertexNeighbors[pinnedVertexIndex])
        {
            ApplySpringForce(pinnedVertexIndex, neighborIndex);
        }
    }

    void ApplyGravityAndSpringForces()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosVertex = transform.TransformPoint(vertices[i]); // Convert vertex to world space

            // Only apply forces to vertices not above the box collider and to non-pinned vertices
            if (!IsVertexAboveBox(worldPosVertex) && i != pinnedVertexIndex)
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
        Vector3 contactPoint = other.ClosestPoint(transform.position);
        PinClosestVertex(contactPoint, other.transform);
    }

    void PinClosestVertex(Vector3 contactPoint, Transform gripperTransform)
    {
        int closestVertexIndex = 0;
        minDistance = Mathf.Infinity;

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

    void OnTriggerExit(Collider other)
    {
        pinnedTransform = null;
        pinnedVertexIndex = -1; // Let the vertex behave naturally after release.
    }
}
