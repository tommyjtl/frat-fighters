using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for units during KnockDown
    public class UnitKnockDown : State {

        private string animationNameUp = "KnockDown Up";
        private string animationNameDown = "KnockDown Down";
        public override bool canGrab => false; //unit cannot be grabbed in this state
        private AttackData attackData;
        private Collider2D col2D;
        private int bounceNum = 1;    
        private float xForce;
        private float yForce;
        private bool fallDamageApplied; //set to true when this unit takes damage upon hitting the floor
        private bool hasHitFloor; //set to true when this unit has hit the the floor once

        public UnitKnockDown(AttackData _attackData, float xForce, float yForce){
            attackData = _attackData;
            this.xForce = xForce;
            this.yForce = yForce;
        }
   
        public override void Enter(){
            col2D = unit.GetComponent<Collider2D>();

            unit.StopMoving();
            unit.isGrounded = false;
            
            //turn towards direction of the player attack
            if(attackData.inflictor != null && attackData.inflictor.CompareTag("Player")) unit.TurnToDir((DIRECTION)((int)attackData.inflictor.GetComponent<UnitActions>().dir * -1)); 
        
            //lose equipped weapon
            if(unit.settings.loseWeaponWhenKnockedDown && unit.weapon != null){
                unit.GetComponentInChildren<WeaponAttachment>()?.LoseCurrentWeapon();
            }
        }

        public override void Update(){
            Vector2 moveVector = unit.transform.position;

            //x force
            moveVector.x = unit.transform.position.x + -(int)unit.dir * xForce * Time.deltaTime;
            xForce = xForce>0? xForce -= Time.deltaTime : 0; //decrease over time

            //hit other units when falling down (optional)
            bool goingDown = yForce < 0;
            bool hurtOtherEnemiesWhenThrown = unit.settings.hitOtherEnemiesWhenThrown && attackData.attackType == ATTACKTYPE.GRABTHROW;
            bool hurtOtherEnemiesWhenFalling = unit.settings.hitOtherEnemiesWhenFalling;
            if(!hasHitFloor && goingDown && hurtOtherEnemiesWhenThrown || hurtOtherEnemiesWhenFalling) unit.CheckForHit(attackData);

            //y force (bounce up until there are no more bounces left)
            if(unit.transform.position.y < unit.groundPos){
                hasHitFloor = true;

                //if this is a GRABTHROW attack, apply fall damage when this unit hits the floor
                if(!fallDamageApplied && attackData.attackType == ATTACKTYPE.GRABTHROW) {
                    unit.GetComponent<HealthSystem>()?.SubstractHealth(attackData.damage);
                    fallDamageApplied = true;
                }

                //keep bouncing until there are no more bounces left
                if(bounceNum>0){
                    unit.transform.position = new Vector3(unit.transform.position.x, unit.groundPos); //position this unit on the floor 
                    yForce = unit.settings.knockDownHeight/2f; //add force but make the next bounce less high
                    unit.CamShake();
                    bounceNum --;
                    return;

                } else {

                    //unit has landed on the floor
                    if(col2D) col2D.offset = Vector2.zero;
                    unit.stateMachine.SetState(new UnitKnockDownGrounded());
                    return;
                }
            }

            //vertical movement
            moveVector.y += yForce * Time.deltaTime * unit.settings.knockDownSpeed;
            yForce -= unit.settings.jumpGravity * Time.deltaTime * unit.settings.knockDownSpeed;

            //position collider on the floor by applying offset
            if(col2D) col2D.offset = new Vector2(col2D.offset.x, -(moveVector.y - unit.groundPos));

            //move unit
            unit.transform.position = moveVector;

            //play up/down animation
            if(yForce>0) unit.animator.Play(animationNameUp);
            else unit.animator.Play(animationNameDown);
        }

        public override void Exit(){

            //make sure this unit is on the floor again when exiting this state
            if(col2D) col2D.offset = Vector2.zero;
            unit.transform.position = unit.currentPosition;
            unit.isGrounded = true;
        }
    }
}
