using UnityEngine;

namespace BeatEmUpTemplate2D {

    //state for attacking during a jump
    public class PlayerJumpAttack : State {

        private AttackData attackData;
        private bool damageDealt;
        private bool hasLanded;
    
        public PlayerJumpAttack(AttackData attackData){
            this.attackData = attackData;
        }
    
        public override void Enter(){
            unit.isGrounded = false;
            unit.animator.Play(attackData.animationState);
        }

        public override void Update(){

            //go to landed state
            if(hasLanded) unit.stateMachine.SetState(new PlayerLand());

            //check for hit
            if(!damageDealt) damageDealt = unit.CheckForHit(attackData); 
        }

        public override void FixedUpdate(){

            //preform jump
            unit.JumpSequence();

            //check is the jump is finished when the unit has reached the ground position
            bool JumpFinished = (unit.transform.position.y < unit.groundPos);

            //end of jump
            if(JumpFinished){
                unit.GetComponent<Collider2D>().offset = Vector2.zero;
                unit.transform.position = new Vector3(unit.transform.position.x, unit.groundPos, 0);
                unit.isGrounded = true;
                hasLanded = true;
            }
        }
    }
}