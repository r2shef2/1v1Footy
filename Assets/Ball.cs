using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float forceMultiplier = 10f; // Control the strength of the movement
    public float upwardForce = 5f; // Control the upward force for horizontal hits
    public LayerMask player;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that collided with this one is on the Player layer
        if ((player.value & (1 << collision.gameObject.layer)) != 0)
        {
            // Get the collision direction and apply it as a force
            Vector3 collisionDirection = collision.contacts[0].normal * -1; // Reverse to get the direction from the hit
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(collisionDirection * forceMultiplier, ForceMode.Impulse);
            }
        }
    }
}
