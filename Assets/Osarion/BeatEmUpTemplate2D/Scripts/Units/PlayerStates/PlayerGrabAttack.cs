using UnityEngine;

namespace BeatEmUpTemplate2D {

    //state for handling player attacks while holding/grabbing an enemy
    public class PlayerGrabAttack : State {

        private float animDuration => unit.GetAnimDuration(attackData.animationState);
        private bool damageDealt;
        private AttackData attackData;

        public PlayerGrabAttack(AttackData attackData){
            this.attackData = attackData;
        }

        public override void Enter(){
            unit.animator.Play(attackData.animationState);
        }

        public override void Update(){
            if(!damageDealt) damageDealt = unit.CheckForHit(attackData);
            if(Time.time - stateStartTime > animDuration) unit.stateMachine.SetState(new PlayerIdle());
        }
    }
}