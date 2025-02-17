using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BeatEmUpTemplate2D {

    //class for prompting players to continue through a level by showing a hand pointer
    public class HandPointer : MonoBehaviour{

        public string sfx = "HandPointer";
        public float flickerInterval = .4f;
        public float timeUntilNextNotification = 2.5f;
        private bool RoutineInProgress;

         void OnEnable() {
		    HealthSystem.onUnitDeath += HandPointerCheck; //subscribe to event
            GetComponent<Image>().enabled = false; //disable by default
	    }

	    void OnDisable() {
		    HealthSystem.onUnitDeath -= HandPointerCheck; //unsubscribe to event
	    }

        //check if the hand pointer needs to be shown
        void HandPointerCheck(GameObject unit){
            if(unit.CompareTag("Enemy") && !RoutineInProgress) StartCoroutine(FlickerHand()); //start hand pointer routine when the unit killed was an enemy
        }

        //show the hand pointer and flicker
        IEnumerator FlickerHand(){
            RoutineInProgress = true;

            //wait a moment before showing the hand
            yield return new WaitForSeconds(timeUntilNextNotification);

            Image hand = GetComponent<Image>();
            for(int i=0; i<8; i++){

                //exit if enemies have spotted the player
                if((EnemyManager.GetTotalEnemyCount() == 0 || EnemiesDetectedPlayer()) && !hand.enabled){
                    RoutineInProgress = false;
                    yield break;
                }

                //enable/disable hand
                hand.enabled = !hand.enabled;

                //play sfx when hand is visible
                if(hand.enabled) BeatEmUpTemplate2D.AudioController.PlaySFX(sfx, Camera.main.transform.position);

                //wait a moment...
                yield return new WaitForSeconds(flickerInterval);
            }

            hand.enabled = false;
            StartCoroutine(FlickerHand());
        }

        //true if any of the current wave of enemies have spotted the player
        bool EnemiesDetectedPlayer(){
            foreach(GameObject enemy in EnemyManager.enemyList){
                if(enemy && enemy.GetComponent<UnitActions>().targetSpotted) return true;
            }
            return false;
        }
    }
}
