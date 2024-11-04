using UnityEngine;
using UnityEngine.InputSystem;

public class AssignController : MonoBehaviour
{
    public PlayerInput playerInput;
    public int deviceIndex = 0;

    void Start()
    {

            Debug.Log("Available Control Schemes:");
            foreach (var scheme in playerInput.actions.controlSchemes)
            {
                Debug.Log(scheme.name);
            }

            // Existing device assignment code here
        var devices = Gamepad.all;

        if (devices.Count > deviceIndex)
        {
            // Use the name of your control scheme, such as "Gamepad", and pass the device as an array
            playerInput.SwitchCurrentControlScheme("Gameplay", new InputDevice[] { devices[deviceIndex] });
            Debug.Log($"Assigned {devices[deviceIndex].displayName} to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Not enough devices connected to assign to this player.");
        }
    }
}
