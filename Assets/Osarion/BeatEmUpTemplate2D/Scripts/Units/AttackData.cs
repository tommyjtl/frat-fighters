using UnityEngine;
using System.Collections.Generic;

namespace BeatEmUpTemplate2D {

    public enum ATTACKTYPE { NONE, PUNCH, KICK, GROUNDPOUND, GRAB, GRABPUNCH, GRABKICK, GRABTHROW, WEAPON };

    [System.Serializable]
    public class AttackData {
        public string name; //optional
        public int damage; //the amount of hp damage
        public string animationState = ""; //the animation state, as defined in the Animator component
        public string sfx = ""; //the name of the sfx to be played on hit
        public ATTACKTYPE attackType = ATTACKTYPE.PUNCH;
        public bool knockdown; //if this attack causes a knockDown or not
        [HideInInspector] public bool foldout;
        [HideInInspector] public GameObject inflictor; //the gameobject inflicting the damage
    
        public AttackData(string name, int damage, GameObject inflictor, ATTACKTYPE attackType, bool knockdown, string sfx = ""){
            this.name = name;
            this.damage = damage;
            this.inflictor = inflictor;
            this.attackType = attackType;
            this.knockdown = knockdown;
            this.sfx = sfx;
        }
    }

    [System.Serializable]
    public class Combo {
        public string comboName = "[New Combo]";
        public List<AttackData> attackSequence = new List<AttackData>();
        [HideInInspector] public bool foldout;
    }
}