using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class PlayerTryGrab : State {

        private string animationName => unit.settings.grabAnimation;
        private float animDuration => unit.GetAnimDuration(animationName);
        private AttackData attackData => new AttackData(animationName.ToString(), 0, unit.gameObject, ATTACKTYPE.GRAB, false);
        public override bool canGrab => false; //cannot be grabbed in this state
        private bool grabInProgress;

        public override void Enter() {

            //if there is a pickup nearby... pick it up
            GameObject pickup = unit.GetClosestPickup(Vector2.one * .4f); // .4f is the pickup range
            if(pickup != null){ unit.stateMachine.SetState(new PlayerGrabItem(pickup)); return; }

            //otherwise, just play grab animation
            unit.animator.Play(animationName);
            unit.StopMoving();
        }

        public override void Update() {

            //during the grab animation we check (based on the hitbox) if we've hit an enemy
            if(unit.HitBoxActive() && !grabInProgress){
                grabInProgress = true;

                //if we encounter an enemy, grab it.
                foreach(GameObject obj in unit.GetObjectsHit(attackData)){
                    if(obj.CompareTag("Enemy")){

                        //check if this enemy can be grabbed
                        if(obj.GetComponent<UnitSettings>()?.canBeGrabbed == true){
                            unit.stateMachine.SetState(new PlayerGrabEnemy(obj));
                            return; //return the first enemy we encounter, because only one enemy can be grabbed at once
                        }
                    }
                }
            }

            //if we've grabbed nothing, return to idle when the animation is finished
            if(Time.time - stateStartTime > animDuration) unit.stateMachine.SetState(new PlayerIdle());
        }
    }
}