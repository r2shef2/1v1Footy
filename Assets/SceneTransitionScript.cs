using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionScript : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;           // Name of the scene to load
    [SerializeField] private float fadeDuration = 1f;      // Duration of the fade-out effect
    [SerializeField] private CanvasGroup fadeCanvasGroup;  // CanvasGroup for fading

    private InputAction transitionAction;

    private void OnEnable()
    {
        // Initialize the InputAction for detecting the Space key or Gamepad Start button
        transitionAction = new InputAction();
        transitionAction.AddBinding("<Keyboard>/space");
        transitionAction.AddBinding("<Gamepad>/start");  // Adds Start button on gamepad
        transitionAction.performed += OnTransitionInput;
        transitionAction.Enable();
    }

    private void OnDisable()
    {
        // Disable and clean up the InputAction when the script is disabled
        transitionAction.performed -= OnTransitionInput;
        transitionAction.Disable();
    }

    private void OnTransitionInput(InputAction.CallbackContext context)
    {
        // Start the fade-out and scene load coroutine
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        // Fade-out effect
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        // Load the specified scene after fading out
        SceneManager.LoadScene(sceneToLoad);
    }
}
