using UnityEngine;

namespace BeatEmUpTemplate2D {

    //ctate for actions when the player is holding an enemy in a grab.
    public class PlayerGrabEnemy : State {

        private GameObject enemy;

        public PlayerGrabEnemy(GameObject enemy){
            this.enemy = enemy;
        }

        public override void Enter(){

            //calculate grab position
            Vector2 grabPos = new Vector2(unit.settings.grabPosition.x * (int)unit.dir, unit.settings.grabPosition.y); //calculate grab position offset based on current direction
            Vector2 enemyGrabPos = (Vector2)unit.transform.position + grabPos; //the enemy position during the grab

            //put enemy in grab state
            enemy.GetComponent<StateMachine>()?.SetState(new EnemyGrabbed(unit.gameObject, enemyGrabPos));
        }

        public override void Update(){
             unit.groundPos = unit.transform.position.y;

            //punch button was pressed during grab
            if(InputManager.PunchKeyDown()){
                //unit.stateMachine.SetState(new PlayerGrabAttack(unit.settings.grabPunch));
                
                // Only throw while grabbing.
                unit.stateMachine.SetState(new PlayerThrowEnemy(enemy));
                enemy = null;
                return;
            }

            //kick button was pressed during grab
            if(InputManager.KickKeyDown()){
                //unit.stateMachine.SetState(new PlayerGrabAttack(unit.settings.grabKick));
                
                // Only throw while grabbing.
                unit.stateMachine.SetState(new PlayerThrowEnemy(enemy));
                enemy = null;
                return;
            }

            //throw button was pressed during grab
            if(InputManager.GrabKeyDown()){
                unit.stateMachine.SetState(new PlayerThrowEnemy(enemy));
                enemy = null;
                return;
            }

            //release grab when time expires
            if(Time.time - stateStartTime > unit.settings.grabDuration){
                unit.stateMachine.SetState(new PlayerIdle()); //player return to Idle
                enemy = null;
            }
        }

        public override void Exit(){
            //release enemy from grab if something else happens
            if(enemy != null) enemy.GetComponent<StateMachine>()?.SetState(new EnemyIdle());
        }
    }
}