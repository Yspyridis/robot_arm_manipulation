using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldMesh : MonoBehaviour
{
    public bool fold = false; // Toggle this to trigger the fold action.
    public float foldSpeed = 1.0f; // Speed of the fold action.
    
    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] foldedVertices;
    private bool isFolding = false;
    private float foldProgress = 0f;
    private float foldLineX;  // Store the center of the mesh for the fold.

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        // Store the original vertices of the mesh.
        originalVertices = mesh.vertices;
        foldedVertices = new Vector3[originalVertices.Length];

        // Calculate the fold line at the center of the mesh.
        foldLineX = (mesh.bounds.min.x + mesh.bounds.max.x) / 2f;
        

        // Prepare the folded vertices by mirroring them relative to the fold line.
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            // Fold vertices to the left if they are on the right side of the fold line.
            if (vertex.x >= foldLineX)
            {
                // Calculate the mirrored position on the left side of the fold line.
                float distanceToFoldLine = vertex.x - foldLineX;
                Debug.Log(distanceToFoldLine);

                // Calculate a smooth y adjustment based on distance to fold line.
                float yadjuster = CalculateYAdjuster(distanceToFoldLine);

                // Apply the yadjuster and calculate folded position.
                foldedVertices[i] = new Vector3(foldLineX - distanceToFoldLine, vertex.y + yadjuster, vertex.z);
            }
            else
            {
                // Vertices on the left side remain the same.
                foldedVertices[i] = vertex;
            }
        }
    }

    // Function to calculate the y-adjustment based on distance from the fold line.
    private float CalculateYAdjuster(float distanceToFoldLine)
    {
        // Lower the height factor to reduce the vertical stretching
        float foldHeightFactor = 1.4f; // Lower value to reduce the height stretching

        // Return a more gradual adjustment based on distance to the fold line
        return Mathf.Pow(distanceToFoldLine, 1.2f) * foldHeightFactor; 
    }


    void Update()
    {
        if (fold && !isFolding && foldProgress == 0f)
        {
            isFolding = true;
        }

        if (isFolding)
        {
            // Perform the fold over time.
            foldProgress += Time.deltaTime * foldSpeed;

            // Ensure that foldProgress stops once it reaches 0.5 (halfway folded)
            foldProgress = Mathf.Clamp(foldProgress, 0f, 0.5f);

            Vector3[] currentVertices = new Vector3[originalVertices.Length];

            // Smooth folding using easing for more natural transition.
            float easedProgress = Mathf.SmoothStep(0f, 1f, foldProgress);

            // Lerp the vertices between the original and folded positions.
            for (int i = 0; i < originalVertices.Length; i++)
            {
                currentVertices[i] = Vector3.Lerp(originalVertices[i], foldedVertices[i], easedProgress);
            }

            // Apply the updated vertices to the mesh.
            mesh.vertices = currentVertices;

            // Recalculate normals, tangents, and bounds to ensure proper rendering and lighting.
            mesh.RecalculateNormals(); 
            mesh.RecalculateTangents();  
            mesh.RecalculateBounds();    

            // Stop the fold once it reaches the halfway point (foldProgress = 0.5)
            if (foldProgress >= 0.5f)
            {
                isFolding = false;  // Stop folding process
            }
        }
    }
}
