using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{
    public class UIPauseMenu : MonoBehaviour
    {
        public bool CanBePaused = true;
        private bool isPaused = false;
        private UIManager uiManager;
        [SerializeField] private string pauseMenuSFX = "UIButtonClick";

        void Start()
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        void Update()
        {
            // Check if the Pause button was pressed this frame
            if (CanBePaused && InputManager.PauseMenuDown())
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        public void PauseGame()
        {

            if (GlobalVariables.Instance != null && GlobalVariables.Instance.isPauEscButtonPressed)
            {
                GlobalVariables.Instance.isPauEscButtonPressed = false;
                return;
            }

            Time.timeScale = 0f; // Pause game physics & movement
            StartCoroutine(DelayAudioPause(0f));

            PlayPauseMenuSFX();

            if (uiManager != null)
                uiManager.ShowMenu("PauseMenu"); // Show Pause Menu

            isPaused = true;
            if (GlobalVariables.Instance != null)
                GlobalVariables.Instance.isPauseMenuActive = true; // Set pause menu active
        }

        public void ResumeGame()
        {
            if (GlobalVariables.Instance != null)
            {
                if (!GlobalVariables.Instance.isPerkMenuActive)
                {
                    Time.timeScale = 1f;
                    AudioListener.pause = false;
                }
            }

            if (uiManager != null)
            { // Find the Pause Menu in UIManager and deactivate it
                foreach (UIMenu menu in uiManager.menuList)
                {
                    if (menu.menuName == "PauseMenu")
                    {
                        PlayPauseMenuSFX();
                        menu.menuGameObject.SetActive(false);
                        break; // Exit loop once found
                    }
                }
            }

            isPaused = false;
            if (GlobalVariables.Instance != null)
                GlobalVariables.Instance.isPauseMenuActive = false; // Set pause menu inactive
        }

        private void PlayPauseMenuSFX()
        {
            BeatEmUpTemplate2D.AudioController.PlaySFX(pauseMenuSFX, Camera.main.transform.position);
        }

        private IEnumerator DelayAudioPause(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            AudioListener.pause = true;
        }
    }
}
