using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

namespace BeatEmUpTemplate2D
{
    public class UISplashScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup promptGroup;
        [SerializeField] private CanvasGroup fadeOverlay;
        [SerializeField] private AudioSource splashMusic;

        [Header("Settings")]
        [SerializeField] private float fadeInDuration = 1f;     // NEW: fade in time
        [SerializeField] private float promptDelay = 1f;        // Delay before showing prompt (starts after fade-in)
        [SerializeField] private float fadeOutDuration = 1f;    // Duration of fade to black
        [SerializeField] private float flashSpeed = 2f;         // Sine-based flash
        [SerializeField] private string mainMenuSceneName = "00_FF_MainMenu";

        private bool promptVisible = false;
        private bool hasPressed = false;

        void Start()
        {
            // Start fully black
            if (fadeOverlay != null)
                fadeOverlay.alpha = 1f;

            if (promptGroup != null)
                promptGroup.gameObject.SetActive(false);

            if (splashMusic != null)
                splashMusic.Play();

            // Start fade-in, then prompt logic
            StartCoroutine(FadeInThenShowPrompt());
        }

        void Update()
        {
            if (promptVisible && promptGroup != null)
            {
                float alpha = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f;
                promptGroup.alpha = alpha;
            }

            if (!hasPressed &&
                (Keyboard.current.anyKey.wasPressedThisFrame ||
                 Mouse.current.leftButton.wasPressedThisFrame ||
                 Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame))
            {
                hasPressed = true;
                StartCoroutine(FadeOutAndLoad());
            }
        }

        private IEnumerator FadeInThenShowPrompt()
        {
            // Fade in from black
            float t = 0f;
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                if (fadeOverlay != null)
                    fadeOverlay.alpha = Mathf.Lerp(1, 0, t / fadeInDuration);
                yield return null;
            }

            // Wait before showing prompt
            yield return new WaitForSeconds(promptDelay);

            if (promptGroup != null)
                promptGroup.gameObject.SetActive(true);
            promptVisible = true;
        }

        private IEnumerator FadeOutAndLoad()
        {
            promptVisible = false;
            

            float t = 0f;
            while (t < fadeOutDuration)
            {
                t += Time.deltaTime;
                if (fadeOverlay != null)
                    fadeOverlay.alpha = Mathf.Lerp(0, 1, t / fadeOutDuration);
                yield return null;
            }

            if (splashMusic != null)
                splashMusic.Stop();

            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
