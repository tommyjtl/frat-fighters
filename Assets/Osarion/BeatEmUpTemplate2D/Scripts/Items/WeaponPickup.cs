using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for weapon pickup items
    public class WeaponPickup : Item  {

        [Header("Weapon Settings")]
        [Help("* Use -1 for infinite Times To Use")]
        public int timesToUse = 3; //how many times this weapon can be used
        public enum DEPLETIONTYPE { DepleteOnHit, DepleteOnUse }    
        public DEPLETIONTYPE depletionType = DEPLETIONTYPE.DepleteOnHit;
        public AttackData attackData;

        [Help("* The sprite that gets attached to the player")]
        public Sprite spriteAttachment; //sprite that gets attached to the hand of the unit
        public Vector2 spritePosOffset; //position offset for when the sprite gets parented to the unit

        [Header("On Destroy")]
        public string destroySFX = "";
        public GameObject destroyEffect;

        //weapon has been picked up
        public override void OnPickUpItem(GameObject target){
            if(!target || !spriteAttachment) return;

            //equip weapon
            target.GetComponentInChildren<WeaponAttachment>()?.equipWeapon(this);

            //play sfx
            BeatEmUpTemplate2D.AudioController.PlaySFX(pickupSFX, target.transform.position);
        }
    }
}