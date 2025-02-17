using UnityEngine;

namespace BeatEmUpTemplate2D {

    //state for landing after a jump
    public class PlayerLand : State {

        private string animationName = "Land";
        private float animDuration => unit.GetAnimDuration(animationName);
    
        public override void Enter(){
            unit.animator.Play(animationName);
            unit.StopMoving();
            unit.Footstep(); //footstep sfx
        }

        public override void Update(){
            if(Time.time - stateStartTime > animDuration)  unit.stateMachine.SetState(new PlayerIdle()); //go to idle state
        }
    }
}
