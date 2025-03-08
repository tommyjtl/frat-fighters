using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatEmUpTemplate2D
{

    //class for loading other scenes via and Exit Sign
    public class UIExitSign : MonoBehaviour
    {

        public SpriteRenderer signSprite; //sprite
        public float fadeDuration = 0.2f; //duration of the exit sign fade in or out
        public string loadSceneOnExit = "00_MainMenu"; //scene to load on exit
        private bool playerInRange; //true if the player is inside the trigger collider
        private bool exitInProgress; //true if exit is initiated
        private float a; //sign transparancy
        private Vector2 startPos; //sign start pos

        void Start()
        {
            startPos = signSprite.transform.position;
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.CompareTag("Player")) playerInRange = true;
        }

        void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.CompareTag("Player")) playerInRange = false;
        }

        void Update()
        {
            if (exitInProgress) return;

            //increase or decrease sign transparancy
            a += Time.deltaTime / fadeDuration * (playerInRange ? 1 : -1);
            a = Mathf.Clamp01(a);

            //move sign up/down when player is close
            signSprite.transform.position = Vector2.Lerp(startPos, startPos + Vector2.up * 0.2f, a);

            //set sprite color
            signSprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, a);

            //if the player is in range and the punch key is pressed, exit scene
            if (playerInRange && InputManager.PunchKeyDown())
            {
                exitInProgress = true;
                SceneManager.LoadScene(loadSceneOnExit);
            }
        }
    }
}