using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{
    //xpsystem class for player, enemy and objects
    public class XPSystem : MonoBehaviour
    {
        // XP

        public int minStageXP = 0;   // experience points
                                     // 200 XP = 1 SP
        public int maxStageXP = 200;
        public int currentStageXP = 0; //current stage XP
        public float stageXpPercentage => (float)currentStageXP / (float)maxStageXP;
        public int currentOverallXP = 0; // current overall XP

        // SP

        public int minSP = 0; // skill points
        public int currentSP = 0; // current skill points

        private GameObject xpBar; // XPbar gameobject to be added

        // Event

        public delegate void OnXPChange(XPSystem xs); // Renamed the delegate
        public static event OnXPChange onXPChange;

        void OnEnable()
        {
            //add enemies to enemyList
            // if(isEnemy) EnemyManager.AddEnemyToList(gameObject);
        }

        void OnDisable()
        {
            //remove enemies from enemyList
            // if(isEnemy) EnemyManager.RemoveEnemyFromList(gameObject);
        }

        void Start()
        {
            //initialize player healthbar
            if (onXPChange != null) onXPChange(this);

            // Load global XP and SP
            if (GlobalVariables.Instance != null)
            {
                Debug.Log("[XPSystem]\t" + "Loading global XP and SP");
                Debug.Log("[XPSystem]\t" + "Current Overall XP: " + GlobalVariables.Instance.globalXP);
                Debug.Log("[XPSystem]\t" + "Current Stage XP: " + GlobalVariables.Instance.globalStageXP);
                Debug.Log("[XPSystem]\t" + "Current SP: " + GlobalVariables.Instance.globalSP);

                currentOverallXP = GlobalVariables.Instance.globalXP;
                currentStageXP = GlobalVariables.Instance.globalStageXP;
                currentSP = GlobalVariables.Instance.globalSP;
            }
        }

        //subtract xp
        public void SubtractXP(int amount)
        {
            //broadcast Event
            SendXPEvent();
        }

        //add xp
        public void AddXP(int amount)
        {
            // print added Xp in the console
            // Debug.Log("[XPSystem]\t" + "Added XP: " + amount);

            // add overall xp
            currentOverallXP += amount;

            // add stage xp, if it is greater than maxStageXP, add 1 SP
            if (currentStageXP + amount >= maxStageXP)
            {
                currentSP += 1;
                currentStageXP = (currentStageXP + amount) - maxStageXP;
            }
            else
            {
                currentStageXP += amount;
            }

            // print the current overall xp in the console
            // Debug.Log("[XPSystem]\t" + "Current Overall XP: " + currentOverallXP);

            // Save global XP and SP
            if (GlobalVariables.Instance != null)
            {
                GlobalVariables.Instance.globalXP = currentOverallXP;
                GlobalVariables.Instance.globalSP = currentSP;
                GlobalVariables.Instance.globalStageXP = currentStageXP;
            }

            SendXPEvent();
        }

        //xp update event
        private void SendXPEvent()
        {
            // float CurrentXpPercentage = 1f / maxStageXP * currentStageXP;

            if (onXPChange != null) onXPChange(this);
        }

        //adjust xpbar position
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
            }
        }
    }
}
