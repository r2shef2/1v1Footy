using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AnyKeySceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;  // Scene name to load
    [SerializeField] private float fadeDuration = 1f;  // Duration of the fade-out effect
    [SerializeField] private CanvasGroup fadeCanvasGroup;  // CanvasGroup for fading
    [SerializeField] private Animation playAnimation;  // CanvasGroup for fading

    private InputAction anyKeyAction;

    private void OnEnable()
    {
        playAnimation.Play();

        // Initialize and enable the InputAction for any key press
        anyKeyAction = new InputAction(binding: "<Keyboard>/anyKey");
        anyKeyAction.AddBinding("<Gamepad>/buttonSouth"); // A on Xbox, X on PlayStation
        anyKeyAction.AddBinding("<Mouse>/leftButton");
        anyKeyAction.performed += _ => StartCoroutine(FadeAndLoadScene());
        anyKeyAction.Enable();
    }

    private void OnDisable()
    {
        // Disable the InputAction to clean up when the script is disabled
        anyKeyAction.Disable();
        anyKeyAction.performed -= _ => StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);  // Gradually increase alpha
            yield return null;
        }

        LoadScene();  // Load the scene after the fade-out
    }

    // Method to load the specified scene
    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name not set in AnyKeySceneLoader script.");
        }
    }
}
