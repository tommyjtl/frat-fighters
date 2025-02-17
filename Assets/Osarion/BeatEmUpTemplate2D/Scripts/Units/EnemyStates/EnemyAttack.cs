using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class EnemyAttack : State {

        private string animationName => attack.animationState;
        private float animDuration => unit.GetAnimDuration(animationName);
        private AttackData attack;
        private bool damageDealt;

        public EnemyAttack(AttackData attack){
            this.attack = attack;
        }

        public override void Enter(){
            unit.StopMoving();
            unit.TurnToTarget();

            //don't attack when target is dead
            if(unit.target && unit.target.GetComponent<HealthSystem>().isDead) unit.stateMachine.SetState(new EnemyIdle());
            
            //play attack anim
            unit.animator.Play(animationName);
        }

        public override void Update(){

            //check for hit until damage was dealt
            if(!damageDealt) damageDealt = unit.CheckForHit(attack); //check for hit until damage was dealt

            //return to idle when animation is finished
            if(Time.time - stateStartTime > animDuration) unit.stateMachine.SetState(new EnemyIdle()); 
        }
    }
}