using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUpTemplate2D
{

    //class for unit or objects XP bar in the User Interface
    public class UIHUBXPBar : MonoBehaviour
    {

        public Text xpValue;
        public Text spValue;
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

        void UpdateXP(XPSystem xs)
        {
            if (xpBar == null) return;

            if (!initialized) InitializeXpBar(); //this is only done once at the start of the level

            xpBar.fillAmount = xs.stageXpPercentage;
            spValue.text = xs.currentSP.ToString();
        }

        //load player data on initialize
        void InitializeXpBar()
        {
            if (GlobalVariables.Instance != null && xpSystem != null)
            {
                xpBar.fillAmount = (float)GlobalVariables.Instance.globalStageXP / (float)xpSystem.maxStageXP;
                spValue.text = GlobalVariables.Instance.globalSP.ToString();
            }

            // Set the initialized flag to true
            initialized = true;
        }

        //show or hide this xpbar
        void ShowXpBar(bool state)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = state ? 1f : 0f;
        }
    }
}
