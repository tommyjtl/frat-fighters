using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for throwing an enemy
    public class PlayerThrowEnemy : State {

        private string animationName = "GrabThrow";
        private float animDuration => unit.GetAnimDuration(animationName);
        private GameObject enemy;

        public PlayerThrowEnemy(GameObject enemy){
            this.enemy = enemy;
        }

        public override void Enter(){

            //return to idle if there is no enemy
            if(!enemy) unit.stateMachine.SetState(new PlayerIdle());
            unit.StopMoving();
            unit.animator.Play(animationName);

            //turn player to opposite direction
            DIRECTION currentDir = unit.dir; //current player direction
            DIRECTION oppositeDir = (DIRECTION)(-(int)currentDir); //opposite player direction
            unit.TurnToDir(oppositeDir);

            //move enemy to position of the player
            enemy.transform.position = unit.transform.position;

            //turn enemy to direction of the player
            UnitActions ua = enemy.GetComponent<UnitActions>();
            ua?.TurnToDir(currentDir);

            //use knockdown state for throw
            UnitSettings enemySettings = enemy.GetComponent<UnitSettings>();
            enemy.GetComponent<StateMachine>()?.SetState(new UnitKnockDown(unit.settings.grabThrow, enemySettings.throwDistance, enemySettings.throwHeight));
        }

        public override void Update(){
            if((Time.time - stateStartTime) > animDuration) unit.stateMachine.SetState(new PlayerIdle()); //return to idle when animation is finished
        }
    }
}