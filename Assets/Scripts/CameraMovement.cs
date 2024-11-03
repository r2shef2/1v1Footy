using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Smoothly move the camera towards that target position
        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
    }
}
