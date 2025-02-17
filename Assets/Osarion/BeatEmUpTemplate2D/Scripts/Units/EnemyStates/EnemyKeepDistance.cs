using UnityEngine;

namespace BeatEmUpTemplate2D {

    //enemy keeps a bit of distance from the target
    public class EnemyKeepDistance : State {

        private Vector2 distance; //amount of distance

        public EnemyKeepDistance(float xMin, float xMax, float yMin, float yMax){
            this.distance = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
        }

        public override void Enter(){
            if(!unit.target){ unit.stateMachine.SetState(new EnemyIdle()); return; } //go back to idle if there is no target
            unit.TurnToTarget();

            //check if we are on the left or right side of the target
            int intDir = (unit.target.transform.position.x < unit.transform.position.x)? 1 : -1;

            //calculate destination point
            float yPos = unit.target.GetComponent<UnitActions>().groundPos + distance.y;
            float xPos = unit.target.transform.position.x + (distance.x * -(int)unit.dir); //unit.dir is used to keep the enemy on the same side of the target
            Vector2 point = new Vector2(xPos, yPos);

            //move to position
            unit.stateMachine.SetState(new EnemyMoveTo(point));
        }
    }
}