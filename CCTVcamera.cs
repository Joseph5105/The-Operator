using UnityEngine;

public class CCTVcamera : MonoBehaviour
{
    public float rotationSpeed = 50f; // Degrees per second
    public Vector3 rotationAxis = Vector3.up; // Default to rotating around the Y-axis
    public float maxAngle = 45f; // Maximum rotation in degrees

    private Vector3 baseRotation = Vector3.zero; // Store the initial rotation
    private float currentOffset = 0f; // Track offset from base rotation
    private int rotationDirection = 1; // 1 for forward, -1 for backward


    void Start()
    {
        // Store the initial rotation as the base rotation
        baseRotation = transform.localEulerAngles;
        currentOffset = 0f; // Start with no offset
    }


    void Update()
    {
        // Calculate how much to rotate this frame
        float rotationAmount = rotationSpeed * Time.deltaTime * rotationDirection;

        // Apply rotation to the offset (not the absolute rotation)
        float newOffset = currentOffset + rotationAmount;

        // Check if we've hit the max angle and reverse direction
        if (newOffset >= maxAngle || newOffset <= -maxAngle)
        {
            rotationDirection *= -1; // Reverse direction
        }

        // Clamp the offset to stay within [-maxAngle, maxAngle]
        newOffset = Mathf.Clamp(newOffset, -maxAngle, maxAngle);

        // Store the new offset
        currentOffset = newOffset;

        // Apply the base rotation + offset
        Vector3 finalRotation = baseRotation;

        // Add the current offset to the appropriate axis based on rotationAxis
        if (rotationAxis == Vector3.right)
        { 
            finalRotation.x += currentOffset; 
        }
        else if (rotationAxis == Vector3.up)
        { 
            finalRotation.y += currentOffset; 
        }
        else if (rotationAxis == Vector3.forward)
        { 
            finalRotation.z += currentOffset; 
        }

        // Apply the final rotation to the transform
        transform.localRotation = Quaternion.Euler(finalRotation);
    }
}