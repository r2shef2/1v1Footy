using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupManager : MonoBehaviour
{
    [System.Serializable]
    public struct Powerup
    {
        public Sprite icon;
        public PowerupEffect effect;
    }

    public enum PowerupEffect
    {
        SpeedBoost,
        FreezeBall,
        Grapple,
    }

    public List<Powerup> powerups;
    public List<PlayerController> players;
    public List<Image> playerIcons;

    private Dictionary<PlayerController, Powerup> assignedPowerups = new Dictionary<PlayerController, Powerup>();

    public void AssignRandomPowerup(PlayerController player)
    {
        int index = Random.Range(0, powerups.Count);
        Powerup newPowerup = powerups[index];

        // Assign the power-up to the player
        assignedPowerups[player] = newPowerup;

        int playerIndex = players.IndexOf(player);
        if (playerIndex >= 0 && playerIndex < playerIcons.Count)
        {
            playerIcons[playerIndex].sprite = newPowerup.icon;
        }
    }

    public void UseAssignedPowerup(PlayerController player)
    {
        if (assignedPowerups.ContainsKey(player))
        {
            Powerup activePowerup = assignedPowerups[player];
            ActivatePowerupEffect(activePowerup.effect, player);
            assignedPowerups.Remove(player);  // Remove after use
        }
    }

    private void ActivatePowerupEffect(PowerupEffect effect, PlayerController player)
    {
        switch (effect)
        {
            case PowerupEffect.SpeedBoost:
                StartCoroutine(ApplySpeedBoost(player));
                break;
            case PowerupEffect.Grapple:
                StartCoroutine(ApplyGrapple(player));  // Activate grapple as a power-up
                break;
            case PowerupEffect.FreezeBall:
                ApplyFreeze();  // Activate grapple as a power-up
                break;
                // Add additional cases as needed
        }
    }

    private IEnumerator ApplySpeedBoost(PlayerController player)
    {
        SoundManager.Instance.PlayPlayerSpeedUpSound();
        float originalSpeed = player.speed;
        player.speed *= 1.5f;
        yield return new WaitForSeconds(3f);
        SoundManager.Instance.PlayPlayerSlowDownSound();
        player.speed = originalSpeed;
    }

    private void ApplyFreeze()
    {
        GameController.Instance.ball.FreezeBall();
    }

    private IEnumerator ApplyGrapple(PlayerController player)
    {
        Debug.Log("Grapple Activated for " + player.name);
        yield return player.GrappleToBall();  // Start the grapple effect in the player
    }
}
