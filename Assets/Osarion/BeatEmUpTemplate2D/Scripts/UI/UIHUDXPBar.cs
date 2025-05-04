using UnityEngine;
using UnityEngine.UI;
using System;

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
        public float BarDivisions = 15;

        void OnEnable()
        {
            GlobalVariables.OnSPChanged += UpdateCurrentSPText;

            XPSystem.onXPChange += UpdateXP; //subscribe to xp update events
            InitializeXpBar(); //initialize xp bar with global values
        }

        void OnDisable()
        {
            GlobalVariables.OnSPChanged -= UpdateCurrentSPText;

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

                xpBar.fillAmount = xpSystem.stageXpPercentage; // (float)GlobalVariables.Instance.globalStageXP / (float)xpSystem.maxStageXP;
                spValue.text = xpSystem.currentSP.ToString();
                xpValue.text = xpSystem.currentStageXP.ToString();
                maxXpValue.text = xpSystem.maxStageXP.ToString();

                GlobalVariables.Instance.globalSP = xpSystem.currentSP;
                GlobalVariables.Instance.globalStageXP = xpSystem.currentStageXP;
                GlobalVariables.Instance.globalXP = xpSystem.currentOverallXP;
            }

            // Set the initialized flag to true
            initialized = true;
        }

        void UpdateXP(XPSystem xs)
        {
            if (xpBar == null) return;
            if (!initialized) InitializeXpBar(); //this is only done once at the start of the level

            double step = 1.0/BarDivisions;
            float fill = (float)(Math.Floor(xs.stageXpPercentage / step) * step);
            xpBar.fillAmount = fill;
            spValue.text = xs.currentSP.ToString();
            xpValue.text = xs.currentStageXP.ToString();
            maxXpValue.text = xs.maxStageXP.ToString();

            GlobalVariables.Instance.globalSP = xs.currentSP;
            GlobalVariables.Instance.globalStageXP = xs.currentStageXP;
            GlobalVariables.Instance.globalXP = xs.currentOverallXP;

            // Debug.Log($"[UIHUDXPBar.cs] Update XP Bar with global values");
            // Debug.Log($"[UIHUDXPBar.cs] SP: {GlobalVariables.Instance.globalSP}, sXP: {GlobalVariables.Instance.globalStageXP}, XP: {GlobalVariables.Instance.globalXP}");
        }

        void UpdateCurrentSPText(int sp)
        {
            // currentSP.text = sp.ToString();
            spValue.text = sp.ToString(); // update SP value in the UI
            xpSystem.currentSP = sp; // update local SP value
        }

    }
}
