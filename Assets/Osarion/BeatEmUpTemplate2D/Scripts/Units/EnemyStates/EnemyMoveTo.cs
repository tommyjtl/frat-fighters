using UnityEngine;

namespace BeatEmUpTemplate2D {

    //enemy moves towards the target
    public class EnemyMoveTo : State {

        private string animationName = "Run";
        private Vector2 destination;

        public EnemyMoveTo(Vector2 pos){
            destination = pos;
        }

        public override void FixedUpdate(){
            Vector2 unitPos = unit.GetComponent<UnitActions>().currentPosition;
            Vector2 moveDir = (destination - unitPos).normalized; //get vector to destination

            //if there is a wall in front of us, go to Idle
            Vector2 wallDistanceCheck = unit.col2D? (unit.col2D.size/1.8f) * 1.1f : Vector2.one * .3f; //dividing by 1.8f because the distance check needs to be a bit larger than the collider (otherwise we never encounter a wall)
            if(unit.WallDetected(moveDir * wallDistanceCheck)){
                unit.stateMachine.SetState(new EnemyIdle());
                return;
            }

            //move and play 'Run' anim
            unit.MoveToVector(moveDir, unit.settings.moveSpeed);
            unit.animator.Play(animationName);
            
            //if we've reached our destination, go to Idle
            if(Vector2.Distance(unitPos, destination) < .1f) unit.stateMachine.SetState(new EnemyIdle());
        }
    }
}