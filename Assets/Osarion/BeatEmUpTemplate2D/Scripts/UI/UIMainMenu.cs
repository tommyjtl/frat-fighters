using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;


namespace BeatEmUpTemplate2D
{
    public class UIMainMenu : MonoBehaviour
    {
        [Header("Credits Panel")]
        public GameObject creditsPanel;  // Drag your CreditsPanel here in the Inspector

        private bool creditsVisible = false;
        private float creditsPopupCooldown = 0f;
        public AudioSource mainMenuMusic;
        public float audioFadeIn = 3f;

        void Start()
        {
            StartCoroutine(FadeInMusic());
        }

        private IEnumerator FadeInMusic()
        {
            float t = 0f;
            while (t < audioFadeIn)
            {
                t += Time.deltaTime;
                mainMenuMusic.volume = Mathf.Lerp(0, .3f, t / audioFadeIn);
                yield return null;
            }
        }

        void Update()
        {
            if (creditsPopupCooldown > 0f)
            {
                creditsPopupCooldown -= Time.unscaledDeltaTime;
                return; // skip input check while cooldown is active
            }
            // Close credits if open and any button is pressed
            if (creditsVisible && (Keyboard.current.anyKey.wasPressedThisFrame ||
                 Mouse.current.leftButton.wasPressedThisFrame ||
                 Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame))
            {
                HideCredits();
            }
        }

        // Call this from UIButton to show credits
        public void ShowCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(true);
                creditsVisible = true;
                creditsPopupCooldown = 0.15f; // Wait ~150ms before allowing close
                Debug.Log("Showing Credits");

                // Clear selection to avoid highlighting stuff behind
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        private void HideCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }
            creditsVisible = false;

            
        }
    }
}
