using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class UnitDefend : State {

        private string animationName = "Defend";
        private string defendHitSFX = "DefendHit";
        private float enemyDefendDuration => unit.settings.defendDuration;
    
        public override void Enter(){
            unit.animator.Play(animationName);
            unit.StopMoving();
        }

        public override void Update(){

            //for players
            if(unit.isPlayer){

                //change direction while defening
                if(unit.isPlayer && unit.settings.canChangeDirWhileDefending){
                    Vector2 inputVector = InputManager.GetInputVector();
                    if(inputVector.x == 1) unit.TurnToDir(DIRECTION.RIGHT);
                    else if(inputVector.x == -1) unit.TurnToDir(DIRECTION.LEFT);
                }

                //return to idle when defend button is released
                if(!InputManager.DefendKeyDown()) unit.stateMachine.SetState(new PlayerIdle());
            }

            //for enemies
            if(unit.isEnemy && (Time.time - stateStartTime > enemyDefendDuration)) unit.stateMachine.SetState(new EnemyIdle());
        }

        //we are hit while defending
        public void Hit(){
           
            //show defend effect
            unit.ShowEffect("DefendEffect");

            //play sfx
            BeatEmUpTemplate2D.AudioController.PlaySFX(defendHitSFX, unit.transform.position);
        }
    }
}
