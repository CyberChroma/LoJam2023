using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target (player) that the camera will follow
    public float smoothSpeed = 0.125f; // Smoothing speed
    public Vector3 offset; // Offset from the target (player)

    void Start() {
        Snap();
    }

    void FixedUpdate() {
        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }

    public void Snap() {
        transform.position = target.position + offset;
    }
}
