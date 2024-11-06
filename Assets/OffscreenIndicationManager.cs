using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicationManager : MonoBehaviour
{
    public RectTransform arrowUI;         // Reference to the arrow UI element in the scene
    public RectTransform canvasRect;      // Reference to the Canvas RectTransform (for screen boundaries)
    public Camera mainCamera;
    public PlayerController playerController;
    public float offScreenOffset = 100; // additional value to be concerded offscreen

    [Tooltip("Adjust this to fine-tune the arrow pointing accuracy.")]
    public float angleOffset = -90f;      // Initial offset, can be fine-tuned in the Inspector

    private void Update()
    {
        Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(playerController.transform.position);

        // Check if the player is off-screen
        bool isOffScreen = playerScreenPosition.x - offScreenOffset < 0 || playerScreenPosition.x + offScreenOffset > Screen.width ||
                           playerScreenPosition.y - offScreenOffset < 0 || playerScreenPosition.y + offScreenOffset > Screen.height;

        // Show and rotate the arrow if the player is off-screen, otherwise hide it
        if (isOffScreen)
        {
            arrowUI.gameObject.SetActive(true);
            RotateArrowTowardsPlayer(playerScreenPosition);
            Physics2D.IgnoreCollision(GameController.Instance.playerOne.headCollider, GameController.Instance.playerTwo.headCollider, true);
            Physics2D.IgnoreCollision(GameController.Instance.playerOne.bodyCollider, GameController.Instance.playerTwo.headCollider, true);
            Physics2D.IgnoreCollision(GameController.Instance.playerOne.bodyCollider, GameController.Instance.playerTwo.bodyCollider, true);
        }
        else
        {
            arrowUI.gameObject.SetActive(false);
            Physics2D.IgnoreCollision(GameController.Instance.playerOne.headCollider, GameController.Instance.playerTwo.headCollider, false);
            Physics2D.IgnoreCollision(GameController.Instance.playerOne.bodyCollider, GameController.Instance.playerTwo.headCollider, false);
            Physics2D.IgnoreCollision(GameController.Instance.playerOne.bodyCollider, GameController.Instance.playerTwo.bodyCollider, false);
        }
    }

    private void RotateArrowTowardsPlayer(Vector3 playerScreenPosition)
    {
        // Calculate the direction from the center of the screen to the player
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 direction = (playerScreenPosition - (Vector3)screenCenter).normalized;

        // Calculate the angle to rotate the arrow
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation with adjustable angle offset
        arrowUI.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
    }
}
