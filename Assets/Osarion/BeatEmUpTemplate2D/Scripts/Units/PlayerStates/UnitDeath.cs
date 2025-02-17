using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class UnitDeath : State {

        private string animationName = "Death";
        private bool showDeathAnimation;

        public UnitDeath(bool showDeathAnim){
            this.showDeathAnimation = showDeathAnim;
        }

        public override void Enter(){

            //play death animation
            if(showDeathAnimation) unit.animator.Play(animationName);

            //set this unit on the floor
            unit.GetComponent<Collider2D>().offset = Vector2.zero;
            unit.transform.position = unit.currentPosition;
            unit.isGrounded = true;
            
            //stop moving
            unit.StopMoving(true);

            //disable all enemy AI if a player has died
            if(unit.isPlayer) EnemyManager.DisableAllEnemyAI();

            //flicker and remove enemy units from the field
            if(unit.isEnemy){
                SpriteFlickerAndDestroy flicker = unit.gameObject.AddComponent<SpriteFlickerAndDestroy>();
                flicker.startDelay = 1f;
            }
        }
    }
}