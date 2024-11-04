using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float horizontal;
    public float speed = 8f;
    public float jumpingPower = 8f;
    public bool isFacingRight = true;
    private bool grounded = false;
    public ParticleSystem hitGrassParticle;

    // Grappling hook variables
    public float grappleSpeed = 15f;  // Speed at which the player is pulled towards the ball
    private bool isGrappling = false;

    // Line renderer variables
    public LineRenderer lineRenderer;
    public Material lineMaterial;  // Assign this material in the inspector

    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure the line renderer
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;  // Initially disable the line

        // Set the sorting layer to "Effects"
        lineRenderer.sortingLayerName = "Effects";
        lineRenderer.sortingOrder = 1;  // Adjust as needed to control render order within the sorting layer
    }

    private void Update()
    {
        if (!isGrappling)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        if (Mathf.Abs(horizontal) > 0.1f)  // Adjust dead zone threshold as needed
        {
            if (!isFacingRight && horizontal > 0f)
            {
                Flip();
            }
            else if (isFacingRight && horizontal < 0f)
            {
                Flip();
            }
        }

        // Update the line position if grappling
        if (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, GameController.Instance.ball.transform.position);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 flippedScale = transform.localScale;
        flippedScale.x *= -1;
        transform.localScale = flippedScale;
    }

    public void UsePowerup(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Start grappling towards the ball
            StartCoroutine(GrappleToBall());
        }
    }

    private IEnumerator GrappleToBall()
    {
        isGrappling = true;
        lineRenderer.enabled = true;  // Enable the line when grappling

        while (isGrappling)
        {
            // Recalculate the direction to the ball in each frame to adjust for ball movement
            Vector2 ballPosition = GameController.Instance.ball.transform.position;
            Vector2 playerPosition = transform.position;
            Vector2 directionToBall = (ballPosition - playerPosition).normalized;

            // Move the player towards the ball at grappleSpeed
            rb.velocity = directionToBall * grappleSpeed;

            yield return null;
        }

        // Stop movement and disable the line once grappling ends
        rb.velocity = Vector2.zero;
        lineRenderer.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            // Stop grappling if we collide with the ball
            isGrappling = false;
            rb.velocity = Vector2.zero;  // Stop the player upon collision with the ball
            lineRenderer.enabled = false;  // Disable the line when grappling ends
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower); // Use rb.velocity.x for horizontal velocity
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); // Preserve horizontal velocity
        }
    }

    private bool IsGrounded()
    {
        return grounded;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Ball")
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Ball")
        {
            grounded = false;
        }
        else
        {
            grounded = true;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!isGrappling)  // Prevent movement while grappling
        {
            horizontal = context.ReadValue<Vector2>().x;
        }
    }
}
