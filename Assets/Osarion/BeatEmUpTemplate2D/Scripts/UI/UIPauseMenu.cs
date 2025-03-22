using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class UIPauseMenu : MonoBehaviour
    {
        public bool CanBePaused = true;
        private bool isPaused = false;
        private UIManager uiManager;

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
            AudioListener.pause = true; // Pause audio
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
                        menu.menuGameObject.SetActive(false);
                        break; // Exit loop once found
                    }
                }
            }
        }
    }
}
