using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class PlayerGroundPunch : State {

        private string animationName => unit.settings.groundPunch.animationState;
        private AttackData attackData => unit.settings.groundPunch;
        private float animDuration => unit.GetAnimDuration(animationName);
        private bool damageDealt; //true if the attack hit something
    
        public override void Enter(){
            unit.StopMoving();

            //play animation
            unit.animator.Play(animationName);

            //save data
            unit.lastAttackType = attackData.attackType;
            unit.lastAttackTime = Time.time;
        }

        public override void Update(){
            if(!damageDealt) damageDealt = unit.CheckForHit(attackData); //check hit until damage was dealt
            if(Time.time - stateStartTime > animDuration) unit.stateMachine.SetState(new PlayerIdle());
        }
    }
}