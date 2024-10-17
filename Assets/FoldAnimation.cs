using System.Collections;
using UnityEngine;

public class FoldAnimation : MonoBehaviour
{
    public Transform redDot;  // Assign the red dot GameObject here
    public Transform point1;  // Initial position (point 1)
    public Transform point2;  // Target position (point 2)
    public float foldDuration = 2f;  // Time for the folding motion
    private bool isFolding = false;  // To track if folding is happening

    void Update()
    {
        // Start the fold on key press (space key in this case)
        if (Input.GetKeyDown(KeyCode.Space) && !isFolding)
        {
            StartCoroutine(FoldToPosition(point2.position, foldDuration));
        }
    }

    IEnumerator FoldToPosition(Vector3 targetPosition, float duration)
    {
        isFolding = true;
        Vector3 startPosition = redDot.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpolates between the start and target positions over time
            redDot.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Ensure it ends exactly at the target position
        redDot.position = targetPosition;
        isFolding = false;
    }
}
