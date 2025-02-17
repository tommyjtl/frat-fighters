using System.Collections;
using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class UISetPlayerInactive : MonoBehaviour {

        public float startDelay = 3f;

         void OnEnable() {
                StartCoroutine(SetPlayerInactive(startDelay));
            }

        //Set all player(s) to Inactive state
        IEnumerator SetPlayerInactive(float delay){
            yield return new WaitForSeconds(startDelay);

            foreach(StateMachine unitStateMachine in GameObject.FindObjectsOfType<StateMachine>()){
                if(unitStateMachine.settings.unitType == UNITTYPE.PLAYER){
                    unitStateMachine.SetState(new PlayerInActive());
                }
            }
        }
    }
}