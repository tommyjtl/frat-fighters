using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for the player attacking with a weapon (pickup)
    public class PlayerWeaponAttack : State {

        private string animationName => unit.weapon.attackData.animationState; //get animation from weapon class
        private AttackData attackData => unit.weapon.attackData; //get attack data from weapon class
        private float animDuration => unit.GetAnimDuration(animationName);
        private bool damageDealt;
        private int timesToUse;
        private bool weaponDestroyed;

        public override void Enter(){

            unit.lastAttackTime = Time.time;
            unit.lastAttackType = attackData.attackType;

            unit.StopMoving();
            unit.animator.Play(animationName);

            timesToUse = unit.weapon.timesToUse;
        }

        public override void Update(){

            //continue to check for hit until damage was dealt
            if(!damageDealt) damageDealt = unit.CheckForHit(attackData);

            //TimesToUse
            if(unit.weapon.timesToUse != -1){ 

                //deplete weapon TimesToUse when something was hit
                if(damageDealt && unit.weapon.depletionType == WeaponPickup.DEPLETIONTYPE.DepleteOnHit) unit.weapon.timesToUse = timesToUse-1;
       
                //deplete weapon TimesToUse on use weapon
                if(unit.weapon.depletionType == WeaponPickup.DEPLETIONTYPE.DepleteOnUse) unit.weapon.timesToUse = timesToUse-1;
            }

            //remove weapon when TimesToUse Reaches 0
            if(unit.weapon.timesToUse == 0 && !weaponDestroyed){
                unit.GetComponentInChildren<WeaponAttachment>().DestroyWeapon();
                weaponDestroyed = true;
            }

            //return to Idle
            if(Time.time - stateStartTime > animDuration) unit.stateMachine.SetState(new PlayerIdle()); 
        }
    }
}