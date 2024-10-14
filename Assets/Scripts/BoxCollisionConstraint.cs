using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollisionConstraint : CollisionConstraint
{
    public Vector3 boxCenter { get; set; }
    [SerializeField]
    public Vector3 boxSize;  // Size of the box
    public Vector3 boxExtents { get; private set; }  // Half-size of the box (extents)
    
    [SerializeField]
    public Vector3 offset;  // Offset from the object position (optional)

    public void Start()
    {
        this.boxExtents = this.boxSize * 0.5f;  // Calculate the extents (half-size)
    }

    public override void UpdatePosition()
    {
        // Update the box center position with any offset applied
        this.boxCenter = this.transform.position + this.offset;
    }

    public override bool ApplyConstraint(ref Vector3 position)
    {
        Vector3 localPosition = position - boxCenter;  // Get position relative to the box center

        // Clamp position within the box bounds
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(localPosition.x, -boxExtents.x, boxExtents.x),
            Mathf.Clamp(localPosition.y, -boxExtents.y, boxExtents.y),
            Mathf.Clamp(localPosition.z, -boxExtents.z, boxExtents.z)
        );

        // If the position was outside the box, update it
        if (localPosition != clampedPosition)
        {
            position = boxCenter + clampedPosition;  // Apply the constraint
            return true;  // Indicate that a constraint was applied
        }

        return false;  // No constraint applied (position inside the box)
    }

    public override void Draw(Color color)
    {
        base.Draw(color);
        // Draw the wireframe box for visualization
        Gizmos.color = color;
        Gizmos.DrawWireCube(boxCenter, boxSize);  // Draw the box with its center and size
    }
}
