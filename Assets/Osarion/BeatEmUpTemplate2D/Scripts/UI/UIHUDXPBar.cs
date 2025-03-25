using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUpTemplate2D
{

    //class for unit or objects XP bar in the User Interface
    public class UIHUBXPBar : MonoBehaviour
    {

        public Text xpValue;
        public Text spValue;
        public Text maxXpValue;
        public Image xpBar;

        private bool initialized; // is the xpbar initialized?
        private XPSystem xpSystem;

        void OnEnable()
        {
            XPSystem.onXPChange += UpdateXP; //subscribe to xp update events
            InitializeXpBar(); //initialize xp bar with global values
        }

        void OnDisable()
        {
            XPSystem.onXPChange -= UpdateXP; //unsubscribe to xp update events
        }

        void Start()
        {
            // Find the player object and get the XPSystem component
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                xpSystem = player.GetComponent<XPSystem>();
                if (xpSystem != null)
                {
                    InitializeXpBar(); //initialize xp bar with global values
                }
            }
        }

        //load player data on initialize
        void InitializeXpBar()
        {
            if (GlobalVariables.Instance != null && xpSystem != null)
            {
                // xpSystem.currentSP = GlobalVariables.Instance.globalSP;
                // xpSystem.currentStageXP = GlobalVariables.Instance.globalStageXP;
                // xpSystem.currentOverallXP = GlobalVariables.Instance.globalXP;

                // Debug.Log("[UIHUDXPBar.cs] " + "Initialize XP Bar with global values");
                // Debug.Log("[UIHUDXPBar.cs] " + "Global SP: " + xpSystem.currentSP);
                // Debug.Log("[UIHUDXPBar.cs] " + "Global Stage XP: " + xpSystem.currentStageXP);
                // Debug.Log("[UIHUDXPBar.cs] " + "Global XP: " + xpSystem.currentOverallXP);

                xpBar.fillAmount = xpSystem.stageXpPercentage; // (float)GlobalVariables.Instance.globalStageXP / (float)xpSystem.maxStageXP;
                spValue.text = xpSystem.currentSP.ToString();
                xpValue.text = xpSystem.currentStageXP.ToString();
                maxXpValue.text = xpSystem.maxStageXP.ToString();
            }

            // Set the initialized flag to true
            initialized = true;
        }

        void UpdateXP(XPSystem xs)
        {
            if (xpBar == null) return;
            if (!initialized) InitializeXpBar(); //this is only done once at the start of the level

            xpBar.fillAmount = xs.stageXpPercentage;
            spValue.text = xs.currentSP.ToString();
            xpValue.text = xs.currentStageXP.ToString();
            maxXpValue.text = xs.maxStageXP.ToString();

            // GlobalVariables.Instance.globalSP = xs.currentSP;
            // GlobalVariables.Instance.globalStageXP = xs.currentStageXP;
            // GlobalVariables.Instance.globalXP = xs.currentOverallXP;

            // Debug.Log("[UIHUDXPBar.cs] " + "Update XP Bar with global values");
            // Debug.Log("[UIHUDXPBar.cs] " + "Global SP: " + GlobalVariables.Instance.globalSP);
            // Debug.Log("[UIHUDXPBar.cs] " + "Global Stage XP: " + GlobalVariables.Instance.globalStageXP);
            // Debug.Log("[UIHUDXPBar.cs] " + "Global XP: " + GlobalVariables.Instance.globalXP);
        }

    }
}
