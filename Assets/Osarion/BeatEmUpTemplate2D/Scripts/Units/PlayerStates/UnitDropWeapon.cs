using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class UnitDropWeapon : State {

        private string animationName = "DropWeapon";
        private float animDuration => unit.GetAnimDuration(animationName);
        private float droptime = .5f; //drop at half the time of this animation

        public override void Enter(){
            unit.StopMoving();
            unit.animator.Play(animationName);
        }

        public override void Update(){

            //drop weapon at half time
            if(unit.weapon && (Time.time - stateStartTime) > animDuration * droptime){
                unit.GetComponentInChildren<WeaponAttachment>().DropCurrentWeapon();
            }

            //return to idle when animation is finished
            if((Time.time - stateStartTime) > animDuration){
                unit.stateMachine.SetState(new PlayerIdle()); 
            }
        }
    }
}
