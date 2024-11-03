using UnityEngine;

public class Ball : MonoBehaviour
{
    public float forceMultiplier = 10f; // Control the strength of the movement
    public float upwardForce = 5f; // Control the upward force for horizontal hits
    public float torqueMultiplier = 5f;  // Control the amount of torque to apply for forward rotation
    public LayerMask player; // LayerMask reference for the player
    public Rigidbody2D rb; // Reference to the Rigidbody2D on this object
    public ParticleSystem hitGrass;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that collided with this one is on the Player layer
        if (collision.gameObject.tag == GameController.PLAYER_TAG)
        {
            HitPlayer(collision);
        }

        if (collision.gameObject.tag == GameController.GROUND_TAG)
        {
            HitGround(collision);
        }
    }


    private void HitGround(Collision2D collision)
    {
        hitGrass.transform.position = collision.GetContact(0).point;
        hitGrass.Play();
    }

    private void HitPlayer(Collision2D collision)
    {
        // Get the collision direction and reverse it to get the direction away from the hit
        Vector2 collisionDirection = collision.contacts[0].normal * -1;

        // Check if the collision direction is mostly horizontal
        if (Mathf.Abs(collisionDirection.y) < 0.5f) // Adjust this threshold if necessary
        {
            // Add upward force to make the object move upward when hit from the side
            collisionDirection.y += upwardForce;
        }

        // Normalize the direction to keep it consistent, then apply the force
        if (rb != null)
        {
            rb.AddForce(collisionDirection.normalized * forceMultiplier, ForceMode2D.Impulse);

            // Determine torque direction based on the horizontal impact
            float torqueDirection = collisionDirection.x > 0 ? 1f : -1f; // Rotate clockwise or counterclockwise based on impact
            rb.AddTorque(torqueDirection * torqueMultiplier, ForceMode2D.Impulse);
        }
    }
}
