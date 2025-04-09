using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{
    // xpsystem class for player, enemy and objects
    public class XPSystem : MonoBehaviour
    {
        // XP
        public int maxStageXP = 200;
        public int currentStageXP = 0; //current stage XP
        public int currentOverallXP = 0; // current overall XP

        // SP
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
        }

        //subtract xp
        public void SubtractXP(int amount)
        {
            //broadcast Event
            SendXPEvent();
        }

        public float stageXpPercentage
        {
            // check if current is equal to 1, 
            // if yes, return 0
            // else, return the currentStageXP divided by maxStageXP
            get
            {
                if (currentStageXP == 1) return 0;
                else return (float)currentStageXP / (float)maxStageXP;
            }
        }

        //add xp
        public void AddXP(int amount)
        {
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

            SendXPEvent();
        }

        //xp update event
        private void SendXPEvent()
        {
            // float CurrentXpPercentage = 1f / maxStageXP * currentStageXP;
            if (onXPChange != null) onXPChange(this);
        }
    }
}
