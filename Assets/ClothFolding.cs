using UnityEngine;

public class ClothFolding : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;

    public KeyCode foldKey = KeyCode.F; // Key to trigger the fold
    private bool firstFoldDone = false; // Track the first fold state
    private bool secondFoldDone = false; // Track the second fold state

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
            if (!firstFoldDone)
            {
                FirstFold(); // Perform the first fold (90 degrees along Z-axis)
                firstFoldDone = true;
            }
            else if (!secondFoldDone)
            {
                SecondFold(); // Perform the second fold (90 degrees along Z-axis)
                secondFoldDone = true;
            }
        }
    }

    void FirstFold()
    {
        if (mesh != null)
        {
            Vector3[] vertices = mesh.vertices;

            // First fold: Rotate vertices around the Z-axis
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].x > 0) // Right side fold upwards by 90 degrees
                {
                    vertices[i] = RotateVertexAroundZAxis(vertices[i], 90f);
                }
                else if (vertices[i].x < 0) // Left side fold upwards by 90 degrees
                {
                    vertices[i] = RotateVertexAroundZAxis(vertices[i], -90f);
                }
            }

            // Apply the modified vertices back to the mesh
            mesh.vertices = vertices;
            mesh.RecalculateNormals(); // Ensure proper shading
            meshFilter.mesh = mesh;

            Debug.Log("Performed the first fold.");
        }
    }

    void SecondFold()
    {
        if (mesh != null)
        {
            Vector3[] vertices = mesh.vertices;

            // Second fold: Fold the halves on top of each other
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

            // Apply the modified vertices back to the mesh
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;

            Debug.Log("Performed the second fold.");
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
}
