using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BeatEmUpTemplate2D {

    //class for handling player combo attacks
    public class PlayerAttack : State {

        private bool damageDealt; //true if the attack has hit something
        private bool animFinished => (Time.time - stateStartTime > animDuration); //true if the end of the animation is reached
        private float animDuration => unit.GetAnimDuration(attackData.animationState); //the duration of the animation
        private AttackData attackData; //information about the current attack
        private ATTACKTYPE attackKeyPressed; //attack key was pressed again during this attack
        private ATTACKTYPE currentAttackType; //the type of the current attack
        private Combo currentCombo; //the combo that matches the current unit.attackList
        private int comboProgress => unit.attackList.Count-1; //the progress in the combo attack sequence (1st attack, 2nd attack etc)

        public PlayerAttack(ATTACKTYPE attackType){
            currentAttackType = attackType;
        }

        public override void Enter(){
            unit.StopMoving();

            //add this attack to the list of attacks
            unit.attackList.Add(currentAttackType);
            
            //try to find a matching combo
            currentCombo = FindComBoMatch(unit.attackList);

            //calculate if we are still in follow Up Time
            bool followUpTimeExpired = (Time.time - unit.lastAttackTime > unit.settings.comboResetTime);
            unit.lastAttackTime = Time.time;
    
            //start from the beginning: if there is no combo match, or if the followTime has expired
            if(currentCombo == null || followUpTimeExpired){
                unit.attackList.Clear();
                unit.attackList.Add(currentAttackType);
                currentCombo = FindComBoMatch(unit.attackList);
                if(currentCombo == null)  unit.stateMachine.SetState(new PlayerIdle()); //still no match? go to idle
            }

            //turn towards input dir
            unit.TurnToFloatDir(InputManager.GetInputVector().x);

            //get attack Data
            attackData = currentCombo?.attackSequence[comboProgress];
            
            //play animation
            if(attackData?.animationState.Length == 0) Debug.Log("Please enter animation state for combo: " + currentCombo.comboName + " - Attack: "+ (comboProgress+1));
            else unit.animator.Play(attackData?.animationState, 0, 0);
           }

        public override void Update(){
            if(attackData == null) return;

            //detect wether the punch key was pressed again during this attack
            if(InputManager.PunchKeyDown()) attackKeyPressed = ATTACKTYPE.PUNCH;
            else if(InputManager.KickKeyDown()) attackKeyPressed = ATTACKTYPE.KICK;

            //check for hit until damage was dealt
            if(!damageDealt) damageDealt = unit.CheckForHit(attackData);

            //stop doing a follow up attack if this is the last attack in the combo
            bool isLastAttack = (currentCombo.attackSequence.Count-1 == comboProgress);
            if(isLastAttack) attackKeyPressed = ATTACKTYPE.NONE;

            //when the animation is finished
            if(animFinished){

                //if the player is carrying a weapon and the punch key was pressed, continue with a weapon attack
                if(unit.weapon && attackKeyPressed == ATTACKTYPE.PUNCH){
                    unit.stateMachine.SetState(new PlayerWeaponAttack()); 
                    return; 
                }

                //continue combo if the an attack key was pressed during this attack
                if(attackKeyPressed != ATTACKTYPE.NONE){
                    unit.stateMachine.SetState(new PlayerAttack(attackKeyPressed)); 
                    return; 
                }

                //otherwise go back to idle
                unit.stateMachine.SetState(new PlayerIdle());
            }            
        }

        public override void Exit() {

            //reset combo if nothing was hit
            if(!damageDealt && unit.settings.continueComboOnHit) unit.attackList.Clear();
        }

        //search through the combos to check if there is a match
        private Combo FindComBoMatch(List<ATTACKTYPE> attackList){

            //go through each combo
            foreach(Combo combo in unit.settings.comboData){

                //continue if the current list of attacks is larger than this combo
                if(combo.attackSequence.Count < attackList.Count) continue;

                //create an attackType sequence of this combo
                List<ATTACKTYPE> comboAttackSequence = new List<ATTACKTYPE>();
                foreach(AttackData attackData in combo.attackSequence) comboAttackSequence.Add(attackData.attackType);

                //compare the combo attackType sequence with the current attack sequence
                bool comboEqual = attackList.SequenceEqual(comboAttackSequence.Take(attackList.Count));

                //this combo is a match
                if(comboEqual) return combo;
            }
            return null;
        }
    }
}
