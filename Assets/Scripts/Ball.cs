using UnityEngine;

public class Ball : MonoBehaviour
{
    public float forceMultiplier = 10f;
    public float upwardForce = 5f;
    public float torqueMultiplier = 5f;
    public float maxAngularVelocity = 10f; // Limit to control maximum rotation speed
    public float torqueCooldown = 0.2f; // Cooldown time for torque application
    public float maxYVelocity = 10f; // Maximum vertical velocity to control the height
    public float maxXVelocity = 15f; // Maximum horizontal velocity to control side speed
    public LayerMask player;
    public Rigidbody2D rb;
    public ParticleSystem hitGrass;
    public TrailRenderer trail;

    private float lastTorqueTime; // Track the last time torque was applied

    private void Update()
    {
        // Limit the vertical velocity to prevent the ball from going too high
        if (Mathf.Abs(rb.velocity.y) > maxYVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * maxYVelocity);
        }

        // Limit the horizontal velocity to prevent the ball from going too fast sideways
        if (Mathf.Abs(rb.velocity.x) > maxXVelocity)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxXVelocity, rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GameController.PLAYER_TAG)
        {
            HitPlayer(collision);
        }

        if (collision.gameObject.tag == GameController.GROUND_TAG)
        {
            HitGround(collision);
        }
    }

    public void SetSimulated(bool argSimulated)
    {
        rb.gravityScale = argSimulated ? 1 : 0;
        trail.enabled = argSimulated;
        rb.freezeRotation = (argSimulated == false);
        rb.velocity = Vector2.zero;
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

        if (Mathf.Abs(collisionDirection.y) < 0.5f)
        {
            collisionDirection.y += upwardForce;
        }

        if (rb != null)
        {
            rb.AddForce(collisionDirection.normalized * forceMultiplier, ForceMode2D.Impulse);

            // Apply torque if the cooldown period has passed
            if (Time.time - lastTorqueTime >= torqueCooldown)
            {
                // Apply torque direction based on horizontal impact
                float torqueDirection = collisionDirection.x > 0 ? 1f : -1f;
                rb.AddTorque(torqueDirection * torqueMultiplier, ForceMode2D.Impulse);

                // Update the last torque application time
                lastTorqueTime = Time.time;
            }

            // Cap the maximum angular velocity to prevent excessive rotation
            if (Mathf.Abs(rb.angularVelocity) > maxAngularVelocity)
            {
                rb.angularVelocity = Mathf.Sign(rb.angularVelocity) * maxAngularVelocity;
            }
        }
    }
}
