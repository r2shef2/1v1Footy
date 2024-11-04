using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerAssigner : MonoBehaviour
{
    public PlayerInput player1Input;
    public PlayerInput player2Input;

    void Start()
    {
        Debug.Log("Available Control Schemes:");
        foreach (var scheme in player1Input.actions.controlSchemes)
        {
            Debug.Log(scheme.name);
        }

        // Get the list of connected gamepads
        var devices = Gamepad.all;

        if (devices.Count >= 2)
        {
            // Assign Gamepad 0 to player 1
            player1Input.SwitchCurrentControlScheme("Gameplay", new InputDevice[] { devices[0] });
            Debug.Log($"Assigned {devices[0].displayName} to Player 1");

            // Assign Gamepad 1 to player 2
            player2Input.SwitchCurrentControlScheme("Gameplay", new InputDevice[] { devices[1] });
            Debug.Log($"Assigned {devices[1].displayName} to Player 2");
        }
        else if (devices.Count == 1)
        {
            // Only one gamepad is connected, assign it to player 1 and keep keyboard for both
            player1Input.SwitchCurrentControlScheme("Gameplay", new InputDevice[] { devices[0] });
            Debug.Log($"Assigned {devices[0].displayName} to Player 1");

            // Player 2 will default to keyboard
            Debug.Log("Player 2 using keyboard as no second gamepad is connected.");
        }
        else
        {
            // No gamepads connected, both players will use keyboard by default
            Debug.Log("No gamepads connected, both players will use keyboard.");
        }
    }
}