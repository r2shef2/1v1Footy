using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CharacterSelectionInput : MonoBehaviour
{
    public CharacterSelectionManager selectionManager;

    public PlayerInput player1Input;
    public PlayerInput player2Input;

    public TextMeshProUGUI P1Text;
    public TextMeshProUGUI P2Text;

    public TextMeshProUGUI bugText;

    private const string wasd = "WASD and Space Bar";
    private const string arrowkeys = "Arrow Keys and Right Shift";
    private const string controller = "Controller";


    private void Start()
    {
        // Initial setup of control schemes
        AssignControllers();

        // Set up listener for device changes
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the device change event to avoid memory leaks
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added)
        {
            Debug.Log($"Device added: {device.displayName}");
            AssignControllers();
        }
    }

    private void AssignControllers()
    {
        Debug.Log("Assigning control schemes...");
        foreach (var scheme in player1Input.actions.controlSchemes)
        {
            Debug.Log(scheme.name);
        }

        var gamepads = Gamepad.all;

        if (gamepads.Count >= 2)
        {
            // Assign Gamepad 0 to player 1
            player1Input.SwitchCurrentControlScheme("Gameplay", new InputDevice[] { gamepads[0] });
            Debug.Log($"Assigned {gamepads[0].displayName} to Player 1");
            P1Text.text = controller + " " + 1;

            // Assign Gamepad 1 to player 2
            player2Input.SwitchCurrentControlScheme("Gameplay", new InputDevice[] { gamepads[1] });
            Debug.Log($"Assigned {gamepads[1].displayName} to Player 2");
            P2Text.text = controller + " " + 2;

            bugText.gameObject.SetActive(true);
        }
        else if (gamepads.Count == 1)
        {
            // Only one gamepad is connected, assign it to player 1 and keep keyboard for both
            player1Input.SwitchCurrentControlScheme("Gameplay", new InputDevice[] { gamepads[0] });
            Debug.Log($"Assigned {gamepads[0].displayName} to Player 1");
            P1Text.text = controller;

            // Player 2 will default to keyboard
            Debug.Log("Player 2 using keyboard as no second gamepad is connected.");
            player2Input.SwitchCurrentControlScheme("Keyboard2", new InputDevice[] { Keyboard.current });
            P2Text.text = arrowkeys;

            bugText.gameObject.SetActive(true);

        }
        else
        {
            // No gamepads connected, both players will use keyboard by default
            Debug.Log("No gamepads connected, both players will use keyboard.");

            player1Input.SwitchCurrentControlScheme("Keyboard1", new InputDevice[] { Keyboard.current });
            P1Text.text = wasd;
            player2Input.SwitchCurrentControlScheme("Keyboard2", new InputDevice[] { Keyboard.current });
            P2Text.text = arrowkeys;

            bugText.gameObject.SetActive(false);

        }
    }
    
    
    public void OnPlayer1Next(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectionManager.OnPlayer1Next();
        }
    }

    public void OnPlayer1Previous(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectionManager.OnPlayer1Previous();
        }
    }

    public void OnPlayer2Next(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectionManager.OnPlayer2Next();
        }
    }

    public void OnPlayer2Previous(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectionManager.OnPlayer2Previous();
        }
    }
}
