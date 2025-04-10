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
            Time.timeScale = 0f; // Pause game physics & movement
            PlayPauseMenuSFX();
            // float delay = Mathf.Max(0.1f, BeatEmUpTemplate2D.AudioController.GetSFXDuration(pauseMenuSFX));

            StartCoroutine(DelayAudioPause(0f));
            
            isPaused = true;

            if (uiManager != null)
                uiManager.ShowMenu("PauseMenu"); // Show Pause Menu
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f; // Resume game
            AudioListener.pause = false; // Resume audio
            isPaused = false;

            if (uiManager != null)
            {
                // Find the Pause Menu in UIManager and deactivate it
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
