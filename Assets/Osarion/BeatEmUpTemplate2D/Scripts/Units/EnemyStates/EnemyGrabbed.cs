using UnityEngine;

namespace BeatEmUpTemplate2D {

    //sets enemy in a grabbed state
    public class EnemyGrabbed : State {

        private string animationName = "Grabbed";
        public override bool canGrab => false; //cannot be grabbed in this state
        private float grabDuration;
        private GameObject player;
        private Vector2 grabPos;

        public EnemyGrabbed(GameObject player, Vector2 grabPos){
            this.player = player;
            this.grabPos = grabPos;
        }

        public override void Enter(){

            //return to idle if there is no player
            if(player == null) unit.stateMachine.SetState(new EnemyIdle());

            unit.StopMoving();
            unit.animator.Play(animationName);

            //move into grab position
            unit.transform.position = grabPos;
            unit.groundPos = unit.transform.position.y;

            //turn to the opposite direction as the grabber
            unit.TurnToDir((DIRECTION)(-(int)player.GetComponent<UnitActions>().dir));
            
            //duration before grab expires
            grabDuration = player.GetComponent<UnitActions>().settings.grabDuration;
        }

        public override void Update(){

            //grab expires
            if(Time.time - stateStartTime > grabDuration) unit.stateMachine.SetState(new EnemyIdle());
        }
    }
}