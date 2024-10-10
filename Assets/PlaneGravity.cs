using UnityEngine;

public class PlaneGravity : MonoBehaviour
{
    private Mesh planeMesh;
    private Vector3[] vertices; // Store the vertices of the plane
    private Vector3[] velocities; // Store velocities for each vertex
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
        ApplyGravityToVertices();
        UpdateMesh();
    }

    // Apply gravity to each vertex unless it's in contact with the box collider
    void ApplyGravityToVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosVertex = transform.TransformPoint(vertices[i]); // Convert vertex to world space

            // Check if the vertex is within the X and Z bounds of the box
            if (IsVertexAboveBox(worldPosVertex))
            {
                // Apply gravity if the vertex is above the box, but don't let it fall below the top of the box
                velocities[i] += new Vector3(0, gravity * Time.deltaTime, 0); // Add gravity to velocity
                velocities[i] *= damping; // Apply damping to slow down over time

                // Update the vertex position
                vertices[i] += velocities[i] * Time.deltaTime;

                // Ensure vertex doesn't fall below the box's top surface
                worldPosVertex = transform.TransformPoint(vertices[i]);
                if (worldPosVertex.y < GetBoxTopY())
                {
                    // Correct the vertex to rest on top of the box
                    vertices[i] = transform.InverseTransformPoint(new Vector3(worldPosVertex.x, GetBoxTopY(), worldPosVertex.z));
                    velocities[i] = Vector3.zero; // Stop the vertex from moving further
                }
            }
            else
            {
                // Apply gravity to the vertices not directly above the box
                velocities[i] += new Vector3(0, gravity * Time.deltaTime, 0); // Gravity
                velocities[i] *= damping; // Damping

                // Update vertex position
                vertices[i] += velocities[i] * Time.deltaTime;
            }
        }
    }

    // Check if a vertex is directly above the box collider (within the X and Z bounds)
    bool IsVertexAboveBox(Vector3 vertexPosition)
    {
        Bounds boxBounds = boxCollider.bounds;

        // Check if the vertex is within the X and Z range of the box
        return (vertexPosition.x >= boxBounds.min.x && vertexPosition.x <= boxBounds.max.x &&
                vertexPosition.z >= boxBounds.min.z && vertexPosition.z <= boxBounds.max.z);
    }

    // Get the top Y position of the box collider
    float GetBoxTopY()
    {
        return boxCollider.bounds.max.y; // The Y coordinate of the top face of the box
    }

    // Update the plane mesh with the new vertex positions
    void UpdateMesh()
    {
        // Apply the updated vertex back to the mesh
        planeMesh.vertices = vertices;
        planeMesh.RecalculateBounds();
        planeMesh.RecalculateNormals();
    }
}
