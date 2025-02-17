using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class EnemyWait : State {

        private string animationName = "Idle";
        private float timeToWait;


        public EnemyWait(float timeToWait){
            this.timeToWait = timeToWait;
        }

        public override void Enter(){
            unit.StopMoving(true);
            unit.TurnToTarget();
            unit.animator.Play(animationName);
        }

         public override void Update(){

            //return to idle
            if(Time.time - stateStartTime > timeToWait) unit.stateMachine.SetState(new EnemyIdle()); 
        }
    }
}