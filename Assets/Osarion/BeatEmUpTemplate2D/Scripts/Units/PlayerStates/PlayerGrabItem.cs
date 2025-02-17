using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class PlayerGrabItem : State {

        private string animationName = "Pickup";
        private float animDuration => unit.GetAnimDuration(animationName);
        private GameObject item;

        public PlayerGrabItem(GameObject _item){
            item = _item;
        }

        public override void Enter(){
            unit.StopMoving();
            item.GetComponent<Item>()?.OnPickUpItem(unit.gameObject); //send pickup event
            unit.animator.Play(animationName);
        }

        public override void Update(){
            if((Time.time - stateStartTime) > animDuration) unit.stateMachine.SetState(new PlayerIdle()); //return to idle when animation is finished
        }
    }
}