using UnityEngine;

namespace BeatEmUpTemplate2D {

    //enemy moves towards the target
    public class EnemyMoveToTargetAndAttack : State {

        private string animationName = "Run";
        private Vector2 maxAttackRange = new Vector2(1.2f, .1f); //the max distance from where we can attack the target
        private float attackDistance = 1f; //the ideal x distance to stand from the target
        private AttackData attack; //the attack this unit will execute upon arrival
        private float pauseBeforeAttack;

        public EnemyMoveToTargetAndAttack(AttackData attack){
            this.attack = attack;
        }

        public override void Enter(){
            if(!unit.target) unit.stateMachine.SetState(new EnemyIdle());
            pauseBeforeAttack = unit.settings.enemyPauseBeforeAttack;
            unit.TurnToTarget();
        }

        public override void Update(){

            //attack when target is in range
            if(targetInRange()){

                //stop at position
                unit.StopMoving();
                unit.animator.Play("Idle");          

                //pause before attack
                if(pauseBeforeAttack > 0){
                    pauseBeforeAttack -= Time.deltaTime; 
                    return; 
                }

                //attack when close, or go to idle when there is no attack
                unit.stateMachine.SetState(attack != null? new EnemyAttack(attack) : new EnemyIdle());
            }
        }

        public override void FixedUpdate(){
            
            //move to target when out of range
            bool targetIsGrounded  = unit.target.GetComponent<UnitActions>().isGrounded;
            if((unit.distanceToTarget().y > maxAttackRange.y && targetIsGrounded) || unit.distanceToTarget().x > maxAttackRange.x){

                Vector2 idealPos = getIdealAttackPos(); //get ideal attack position
                Vector2 dirToPos = (idealPos - (Vector2)unit.transform.position).normalized; //get vector to attack position

                //if there is a wall in front of us, go to Idle
                Vector2 wallDistanceCheck = unit.col2D? (unit.col2D.size/1.6f) * 1.1f : Vector2.one * .3f; //dividing by 1.8f because the distance check needs to be a bit larger than the collider (otherwise we never encounter a wall)
                if(unit.WallDetected(dirToPos * wallDistanceCheck)){
                    unit.stateMachine.SetState(new EnemyIdle());
                    return;
                }

                //move and play 'Run' anim
                unit.MoveToVector(dirToPos, unit.settings.moveSpeed);
                unit.animator.Play(animationName); 
            }
        }

        //returns the ideal attack position
        Vector2 getIdealAttackPos(){
            Vector2 XDirToTarget = (unit.target.transform.position.x > unit.transform.position.x)? Vector2.right : Vector2.left; //check if the target is to the left or right of us
            return unit.target.GetComponent<UnitActions>().currentPosition - XDirToTarget * attackDistance; //return ideal position to attack the target
        }

        //returns true if the target is currently in attack range
        bool targetInRange(){
            return (unit.distanceToTarget().x < maxAttackRange.x && unit.distanceToTarget().y < maxAttackRange.y);
        }
    }
}