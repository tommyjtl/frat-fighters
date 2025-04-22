using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace BeatEmUpTemplate2D
{

    //Input Manager for the Modern Unity Input System
    public class InputManager : MonoBehaviour
    {

        public static InputManager Instance { get; private set; }
        [Header("MODERN INPUTMANAGER. v1.0")]
        [ReadOnlyProperty] public string controlsScheme;
        public PlayerControls playerInput;
        public static bool playerControlEnabled = true;
        private InputAction move;
        private InputAction punch;
        private InputAction kick;
        private InputAction defend;
        private InputAction grab;
        private InputAction jump;

        private InputAction pause;

        private UIPauseMenu pauseMenu;
        private InputAction perkMenu;
        // private UIPerksAndUltimates perksAndUltimatesMenu;

        void Awake()
        {

            playerInput = new PlayerControls();
            controlsScheme = playerInput.ToString();

            //singleton pattern (only one InputManager allowed in a scene)
            if (Instance == null)
            {
                Instance = this;

            }
            else
            {
                Debug.Log("Multiple InputManagers found in this scene, there can be only one.");
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {

            //subscribe to event
            InputSystem.onDeviceChange += OnDeviceChange;

            move = playerInput.Player.Move;
            punch = playerInput.Player.Punch;
            kick = playerInput.Player.Kick;
            defend = playerInput.Player.Defend;
            grab = playerInput.Player.Grab;
            jump = playerInput.Player.Jump;
            pause = playerInput.Player.Pause;
            perkMenu = playerInput.Player.PerkMenu;
            // perkAndUltimates = playerInput.Player.PerkAndUltimates;

            move.Enable();
            punch.Enable();
            kick.Enable();
            defend.Enable();
            grab.Enable();
            jump.Enable();
            pause.Enable();
            perkMenu.Enable();
            // perkAndUltimates.Enable();
        }

        void OnDisable()
        {

            //unsubscribe from event
            InputSystem.onDeviceChange -= OnDeviceChange;

            move.Disable();
            punch.Disable();
            kick.Disable();
            defend.Disable();
            grab.Disable();
            jump.Disable();
            pause.Disable();
            perkMenu.Disable();
            // perkAndUltimates.Disable();
        }

        //get Punch button state
        public static bool PunchKeyDown()
        {
            return playerControlEnabled && Instance.punch.WasPressedThisFrame();
        }

        //get Kick button state
        public static bool KickKeyDown()
        {
            return playerControlEnabled && Instance.kick.WasPressedThisFrame();
        }

        //get Jump button state
        public static bool DefendKeyDown()
        {
            return playerControlEnabled && Instance.defend.IsPressed();
        }

        //get Grab button state
        public static bool GrabKeyDown()
        {
            return playerControlEnabled && Instance.grab.WasPressedThisFrame();
        }

        //get Jump button state
        public static bool JumpKeyDown()
        {
            return playerControlEnabled && Instance.jump.WasPressedThisFrame();
        }

        //get Pause button state
        public static bool PauseMenuDown()
        {
            return Instance.pause.WasPressedThisFrame();
        }

        //get PerkMenu button state
        public static bool PerkMenuDown()
        {
            return Instance.perkMenu.WasPressedThisFrame();
        }

        //returns the directional input as a vector2
        public static Vector2 GetInputVector()
        {
            return playerControlEnabled ? Instance.move.ReadValue<Vector2>() : Vector2.zero;
        }

        //detect joypad direction input
        public static bool JoypadDirInputDetected()
        {
            return (Instance.move.ReadValue<Vector2>().x != 0 || Instance.move.ReadValue<Vector2>().y != 0);
        }

        //detect device input change
        void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added)
            {
                InputUser.PerformPairingWithDevice(device, InputUser.all[0], InputUserPairingOptions.ForceNoPlatformUserAccountSelection);
            }
            else if (change == InputDeviceChange.Removed)
            {
            }
        }
    }
}