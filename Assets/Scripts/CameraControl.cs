using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Drag your 'Player' here in the Inspector
    public Vector3 offset = new Vector3(0f, 5f, -7f); // Distance behind/above player
    public float smoothSpeed = 0.125f; // How smoothly the camera follows

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        
        // Use Lerp for smooth movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        transform.position = smoothedPosition;

        // Keep the camera looking at the target
        transform.LookAt(target);
    }
}