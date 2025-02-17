using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace BeatEmUpTemplate2D {

    //class for navigating UI buttons via the InputManager (joypad and keyboard)
    public class UIButton : MonoBehaviour, ISelectHandler, IPointerDownHandler, ISubmitHandler {

        public bool SelectOnStart;

        [Header("Change button text when selected")]
        public Text buttonText;
        private Color buttonTextDefaultColor = Color.white; //default button text color
        public Color buttonTextSelectedColor = Color.black; //color of the button text when the button is selected

        [Header("Show/hide and image when selected (optional)")]
        public Image imageTarget;
    
        [Header("Button Audio")]
        [SerializeField] private string sfxOnClick = "UIButtonClick"; //play this sfx when this button was pressed / clicked
        [SerializeField] private string sfxOnSelect = "UIButtonSelect"; //play this sfx when this button was selected
        [HideInInspector] public bool waitForButtonRelease;

        public static GameObject lastSelectedButton;
        private bool LoadSceneInProgress;
        private EventSystem eventSystem => EventSystem.current;
        private InputManager input;
   
        private Button thisButton;
        private float timeAlive;

        void OnEnable(){
             timeAlive = Time.time;
        }

        void Start() {

            //find the InputManager
            input = GetInputManager(); 

            //if true, select this button on start by default
            if(SelectOnStart && GetComponent<Button>() != null) GetComponent<Button>().Select();

            //get component
            thisButton = GetComponent<Button>();
 
            //save default button text color
            if(buttonText != null) buttonTextDefaultColor = buttonText.color;
        }

        //---
        // Keyboard / Joypad Navigation via InputManager
        //---

        void Update(){

            //set text button color
            bool selected = (eventSystem.currentSelectedGameObject == gameObject && thisButton.interactable);
            if(buttonText != null) buttonText.color = selected? buttonTextSelectedColor : buttonTextDefaultColor;

            //show / hide image
            if(imageTarget != null) imageTarget.enabled = selected;

            //conditions for button navigation
            if(InputManager.Instance == null) return; //do nothing when there is no InputManager
            if(InputManager.JoypadDirInputDetected()) return; //ignore joypad input (this is handled by Unity's built in event manager)
            if(eventSystem.currentSelectedGameObject == null && UIButton.lastSelectedButton != null) eventSystem.SetSelectedGameObject(UIButton.lastSelectedButton); //fixes an issue with Unity where clicking with a mouse outside of the interactable area deselects all buttons
            if(EventSystem.current.currentSelectedGameObject != gameObject) return; //only listen to selected buttons
            if(InputManager.GetInputVector() == Vector2.zero) waitForButtonRelease = false;//no input buttons are currently pressed, reset waitForButtonRelease
            if(waitForButtonRelease) return; //wait for user to release a button, before continuing

            //get keyboard / joypad input direction and navigate accordingly
            Vector2 dir = InputManager.GetInputVector();
            if(dir != Vector2.zero) NavigateToNextSelectable(dir);
        }

        //find next button in a navigation direction (vector2) and select it
        void NavigateToNextSelectable(Vector2 dir){

            Selectable current = eventSystem.currentSelectedGameObject?.GetComponent<Selectable>();
            if (current != null){
                Selectable next = current.FindSelectable(dir);
                if(next != null) next.Select();
            }
        }

        //find input manager
        InputManager GetInputManager(){
            InputManager im = GameObject.FindObjectOfType<InputManager>();
            if(im == null) Debug.LogError("No Inputmanager found in this scene");
            return im;
        }

        //this button is selected
        public void OnSelect(BaseEventData eventData){
            UIButton.lastSelectedButton = gameObject; //set this button as selected
            if(sfxOnSelect.Length > 0 && Time.time - timeAlive > Time.deltaTime) BeatEmUpTemplate2D.AudioController.PlaySFX(sfxOnSelect, Camera.main.transform.position); //play sfx. (Time.time - timeAlive > 0) skips playing a sfx when the buttons first appears
            waitForButtonRelease = true; //wait for user to release button before continueing
        }

        //this button is clicked with the mouse
        public void OnPointerDown(PointerEventData eventData){
             if(sfxOnClick.Length > 0) BeatEmUpTemplate2D.AudioController.PlaySFX(sfxOnClick, Camera.main.transform.position); //play sfx
        }

        //this button is pressed via keyboard
        public void OnSubmit(BaseEventData eventData){
            if(sfxOnClick.Length > 0) BeatEmUpTemplate2D.AudioController.PlaySFX(sfxOnClick, Camera.main.transform.position); //play sfx
        }

        //disables all button interaction
        void DisableAllButtons(){
            foreach(Button button in FindObjectsOfType<Button>()) button.interactable = false;
        }

        //----
        //Common Button Actions
        //----
    
        public void QuitApplication(){
		    #if UNITY_EDITOR
		    UnityEditor.EditorApplication.isPlaying = false;
		    #else
		    Application.Quit();
		    #endif
	    }

        public void LoadScene(string sceneName){
            float sfxDuration = BeatEmUpTemplate2D.AudioController.GetSFXDuration(sfxOnClick);
            StartCoroutine(LoadSceneRoutine(sceneName, sfxDuration));
        }

        public void ReloadCurrentScene(){
            float sfxDuration = BeatEmUpTemplate2D.AudioController.GetSFXDuration(sfxOnClick);
            StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().name, sfxDuration));
        }

        //loads the next scene with a small delay
        IEnumerator LoadSceneRoutine(string sceneName, float delay){
            if(LoadSceneInProgress) yield break;
            DisableAllButtons();
            LoadSceneInProgress = true;
            yield return new WaitForSeconds(delay); //wait a moment
            SceneManager.LoadScene(sceneName); //load scene
            LoadSceneInProgress = false;
        }
    }
}
