using UnityEngine;

public class PlayerStompDetector : MonoBehaviour
{
    public float stunDuration = 1.5f;          // Duration of the stun effect
    public ParticleSystem stunParticleEffect;   // Particle effect for the stun

    private PlayerController playerController;  // Reference to the parent player’s controller

    private void Awake()
    {
        // Get the PlayerController component from the parent object
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("StompDetector: No PlayerController found in parent.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to another player and is not the parent player
        if (other.CompareTag("Player") && other.gameObject != playerController.gameObject)
        {
            // Get the PlayerController of the other player
            PlayerController otherPlayerController = other.GetComponentInParent<PlayerController>();
            if (otherPlayerController != null)
            {
                // Apply stun to the other player and play the stun effect
                otherPlayerController.ApplyStun(stunDuration, transform.position);
                SoundManager.Instance.PlayStunSound();
                PlayStunEffect();
            }
        }
    }

    private void PlayStunEffect()
    {
        // Play the stun particle effect at the stomp position if assigned
        if (stunParticleEffect != null)
        {
            stunParticleEffect.transform.position = transform.position;
            stunParticleEffect.Play();
        }
    }
}
