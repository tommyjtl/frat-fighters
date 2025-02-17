using UnityEngine;

namespace BeatEmUpTemplate2D {

    //State that causes the player to remain inactive, effectively disabling all controls.
    public class PlayerInActive : State {

        private string animationName = "Idle";
    
        public override void Enter(){
            unit.animator.Play(animationName);
            unit.StopMoving();
        }
    }
}
