using UnityEngine;

public class ClothFolding : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;

    public KeyCode foldKey = KeyCode.F; // Key to trigger the fold
    private int foldStage = 0; // Track the current fold stage

    void Start()
    {
        // Get the MeshFilter component and the mesh
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        if (mesh == null)
        {
            Debug.LogError("Mesh is not attached to the object!");
        }
    }

    void Update()
    {
        // Check for the fold key press (in this case, F key)
        if (Input.GetKeyDown(foldKey))
        {
            PerformFold();
        }
    }

    void PerformFold()
    {
        if (mesh != null)
        {
            Vector3[] vertices = mesh.vertices;

            if (foldStage == 0)
            {
                // First fold: Fold the two sides upwards by 90 degrees
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].x > 0) // Right side fold upwards
                    {
                        vertices[i] = RotateVertexAroundZAxis(vertices[i], 90f);
                    }
                    else if (vertices[i].x < 0) // Left side fold upwards
                    {
                        vertices[i] = RotateVertexAroundZAxis(vertices[i], -90f);
                    }
                }

                Debug.Log("Performed the first fold.");
            }
            else if (foldStage == 1)
            {
                // Second fold: Fold the two halves on top of each other
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].y > 0) // Fold the right side down
                    {
                        vertices[i] = RotateVertexAroundZAxis(vertices[i], 90f);
                    }
                    else if (vertices[i].y < 0) // Fold the left side down
                    {
                        vertices[i] = RotateVertexAroundZAxis(vertices[i], -90f);
                    }
                }

                Debug.Log("Performed the second fold.");
            }
            else if (foldStage == 2)
            {
                // Third fold: Fold both sides symmetrically upwards along the Z-axis (create U-shape above grey)
                float midZ = (mesh.bounds.min.z + mesh.bounds.max.z) / 2; // Find the middle Z position for symmetry
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].z > midZ) // Fold the far edge upwards
                    {
                        vertices[i] = RotateVertexAroundXAxis(vertices[i], 270f); // Rotate far edge up (above grey area)
                    }
                    else if (vertices[i].z < midZ) // Fold the near edge upwards
                    {
                        vertices[i] = RotateVertexAroundXAxis(vertices[i], -270f); // Rotate near edge up (above grey area)
                    }
                }

                Debug.Log("Performed the third fold (U-shape above grey).");
            }
            else if (foldStage == 3)
            {
                // Fourth fold: Fold the U-shaped ends on top of each other to make a cube, centered on grey
                float midX = (mesh.bounds.min.x + mesh.bounds.max.x) / 2; // Adjust fold to ensure the final cube is centered
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].x > midX) // Fold one part down
                    {
                        vertices[i] = RotateVertexAroundXAxis(vertices[i], 90f); // Rotate down to center
                    }
                    else if (vertices[i].x < midX) // Fold the other part down
                    {
                        vertices[i] = RotateVertexAroundXAxis(vertices[i], -90f); // Rotate down to center
                    }
                }

                Debug.Log("Performed the fourth fold (centered cube).");
            }

            // Apply the modified vertices back to the mesh
            mesh.vertices = vertices;
            mesh.RecalculateNormals(); // Ensure proper shading
            meshFilter.mesh = mesh;

            foldStage++; // Move to the next fold stage
        }
    }

    // Rotate the vertex around the Z-axis to simulate folding
    Vector3 RotateVertexAroundZAxis(Vector3 vertex, float angle)
    {
        float rad = angle * Mathf.Deg2Rad; // Convert angle to radians
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        // Apply rotation around the Z-axis
        float newX = vertex.x * cos - vertex.y * sin;
        float newY = vertex.x * sin + vertex.y * cos;

        return new Vector3(newX, newY, vertex.z); // Return the new vertex position
    }

    // Rotate the vertex around the X-axis to simulate folding
    Vector3 RotateVertexAroundXAxis(Vector3 vertex, float angle)
    {
        float rad = angle * Mathf.Deg2Rad; // Convert angle to radians
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        // Apply rotation around the X-axis
        float newY = vertex.y * cos - vertex.z * sin;
        float newZ = vertex.y * sin + vertex.z * cos;

        return new Vector3(vertex.x, newY, newZ); // Return the new vertex position
    }
}
