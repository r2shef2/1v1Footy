using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public Rigidbody2D rb;
    public float horizontal;
    public float speed = 8f;
    public float jumpingPower = 8f;
    public bool isFacingRight = true;
    private bool grounded = false;
    public ParticleSystem hitGrassParticle;
    private bool allowMovement = true;
    public Collider2D headCollider;
    public Collider2D bodyCollider;

    [Header("Stun")]
    public float stunDuration = 1.5f;
    public float stunCooldown = 2.0f;  // Cooldown to prevent multiple stuns
    private bool isStunned = false;    // Tracks if the player is already stunned

    [Header("Powerup")]
    public float grappleSpeed = 15f;
    private bool isGrappling = false;
    public LineRenderer lineRenderer;
    public Material lineMaterial;

    // Powerup manager reference and cooldown variables
    public PowerupManager powerupManager;  // Reference to PowerupManager
    [SerializeField] private float powerupCooldownDuration = 5f;
    private float powerupCooldownTimer;
    public Image cooldownImage;

    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure line renderer
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        lineRenderer.sortingLayerName = "Effects";
        lineRenderer.sortingOrder = 1;

        // Initialize cooldown timer to start in cooldown
        powerupCooldownTimer = powerupCooldownDuration;

        // Request an initial power-up assignment from the manager
        if (powerupManager != null)
        {
            powerupManager.AssignRandomPowerup(this);
        }
    }

    private void Update()
    {
        if (!isGrappling && allowMovement)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        if (Mathf.Abs(horizontal) > 0.1f)
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

        // Update line position if grappling
        if (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, GameController.Instance.ball.transform.position);
        }
    }

    public void UpdateTimers()
    {
        // Update cooldown timer and assign a new power-up when timer resets
        if (powerupCooldownTimer > 0)
        {
            powerupCooldownTimer -= Time.deltaTime;
            UpdateCooldownUI();
        }
    }

    private void UpdateCooldownUI()
    {
        if (cooldownImage != null)
        {
            // Invert fill: Start fully filled and gradually empty as cooldown completes
            cooldownImage.fillAmount = 1 - (powerupCooldownTimer / powerupCooldownDuration);
        }
    }

    public void SetCollisions(bool argCollisionsOn)
    {
        headCollider.enabled = argCollisionsOn;
        bodyCollider.enabled = argCollisionsOn;
    }

    public void SetAllowedMovement(bool argAllow)
    {
        if (!argAllow)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            horizontal = 0;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        allowMovement = argAllow;
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
        if (context.performed && powerupCooldownTimer <= 0 && powerupManager != null)
        {
            // Use assigned power-up and reset cooldown
            powerupManager.UseAssignedPowerup(this);

            // assign a new powerup after use
            if (powerupCooldownTimer <= 0 && powerupManager != null)
            {
                powerupManager.AssignRandomPowerup(this);
            }

            powerupCooldownTimer = powerupCooldownDuration;
        }
    }

    public IEnumerator GrappleToBall()
    {
        if (allowMovement)
        {
            isGrappling = true;
            lineRenderer.enabled = true;

            while (isGrappling)
            {
                Vector2 ballPosition = GameController.Instance.ball.transform.position;
                Vector2 playerPosition = transform.position;
                Vector2 directionToBall = (ballPosition - playerPosition).normalized;

                rb.velocity = directionToBall * grappleSpeed;

                yield return null;
            }

            rb.velocity = Vector2.zero;
        }
        lineRenderer.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameController.GROUND_TAG) || collision.gameObject.CompareTag(GameController.PLAYER_TAG))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            grounded = true;
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            isGrappling = false;
            rb.velocity = Vector2.zero;
            lineRenderer.enabled = false;
        }
    }

    public void ApplyStun(float duration, Vector3 hitPosition)
    {
        if (!isStunned)  // Only apply if not currently stunned
        {
            StartCoroutine(StunEffect(duration, hitPosition));
        }
    }

    private IEnumerator StunEffect(float duration, Vector3 hitPosition)
    {
        isStunned = true;  // Set stunned state

        SetAllowedMovement(false);

        // Store the original scale of the head sprite for reset
        Vector3 originalPosition = headCollider.transform.localPosition;

        // Scale down the head sprite from the bottom (y-axis shrink)
        headCollider.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y - .3f, originalPosition.z);

        yield return new WaitForSeconds(duration);

        // Reset the head to its original scale
        headCollider.transform.localPosition = originalPosition;

        SetAllowedMovement(true);

        yield return new WaitForSeconds(stunCooldown);  // Wait for cooldown before allowing another stun
        isStunned = false;  // Reset stunned state
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (allowMovement)
        {
            if (context.performed && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (context.canceled && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
    }

    private bool IsGrounded()
    {
        return grounded;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GameController.GROUND_TAG || collision.gameObject.tag == GameController.PLAYER_TAG)
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
        if (collision.gameObject.tag == GameController.GROUND_TAG || collision.gameObject.tag == GameController.PLAYER_TAG)
        {
            grounded = false;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (allowMovement && !isGrappling)
        {
            horizontal = context.ReadValue<Vector2>().x;
        }
    }
}
