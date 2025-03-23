using UnityEngine;

namespace BeatEmUpTemplate2D
{

    public class UnitDeath : State
    {

        private string animationName = "Death";
        private bool showDeathAnimation;
        private XPSystem playerXPSystem;

        public UnitDeath(bool showDeathAnim)
        {
            this.showDeathAnimation = showDeathAnim;
        }

        public override void Enter()
        {
            // Find the player object in the scene and get its XPSystem component
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerXPSystem = player.GetComponent<XPSystem>();

            //play death animation
            if (showDeathAnimation) unit.animator.Play(animationName);

            //set this unit on the floor
            unit.GetComponent<Collider2D>().offset = Vector2.zero;
            unit.transform.position = unit.currentPosition;
            unit.isGrounded = true;

            //stop moving
            unit.StopMoving(true);

            //disable all enemy AI if a player has died
            if (unit.isPlayer) EnemyManager.DisableAllEnemyAI();

            //flicker and remove enemy units from the field
            if (unit.isEnemy && !unit.isBoss)
            {
                SpriteFlickerAndDestroy flicker = unit.gameObject.AddComponent<SpriteFlickerAndDestroy>();
                flicker.startDelay = 1f;

                Debug.Log("Enemy " + unit.gameObject.name + " has been defeated!");

                // Add XP points to the player
                if (playerXPSystem != null)
                {
                    playerXPSystem.AddXP(80);
                    // @TODO: depending on which enemy is defeated, add different XP points
                    Debug.Log("Player XP: " + playerXPSystem.currentOverallXP);
                }
            }

            if (unit.isEnemy && unit.isBoss)
            {
                SpriteFlickerAndDestroy flicker = unit.gameObject.AddComponent<SpriteFlickerAndDestroy>();
                flicker.startDelay = 1f;

                Debug.Log("Boss " + unit.gameObject.name + " has been defeated!");

                // Add XP points to the player
                if (playerXPSystem != null)
                {
                    playerXPSystem.AddXP(160);
                    Debug.Log("Player XP: " + playerXPSystem.currentOverallXP);
                }
            }

        }
    }
}