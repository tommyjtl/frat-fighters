using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BeatEmUpTemplate2D
{

    public class UnitDeath : State
    {

        private string animationName = "Death";
        private bool showDeathAnimation;
        private XPSystem playerXPSystem;
        private HealthSystem playerHPSystem;

        public UnitDeath(bool showDeathAnim)
        {
            this.showDeathAnimation = showDeathAnim;
        }

        // //
        // void OnEnable()
        // {
        //     GlobalVariables.OnSPChanged += UpdateCurrentSPText;
        // }

        // void OnDisable()
        // {
        //     GlobalVariables.OnSPChanged -= UpdateCurrentSPText;
        // }

        // void UpdateCurrentSPText(int sp)
        // {
        //     currentSP.text = sp.ToString();
        //     PerkStatusCheck();
        // }

        public override void Enter()
        {
            // Find the player object in the scene and get its XPSystem component
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerXPSystem = player.GetComponent<XPSystem>();
                playerHPSystem = player.GetComponent<HealthSystem>();
            }

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
            if (unit.isEnemy)
            {

                SpriteFlickerAndDestroy flicker = unit.gameObject.AddComponent<SpriteFlickerAndDestroy>();
                flicker.startDelay = 1f;

                // Add XP points to the player
                if (playerXPSystem != null)
                {
                    if (GlobalVariables.Instance != null)
                    {
                        if (GlobalVariables.Instance.globalStealOnEnemyKill)
                        {
                            var healthSystem = unit.GetComponent<HealthSystem>();
                            if (healthSystem != null)
                            {
                                int enemyMaxHp = healthSystem.maxHp;
                                // Debug.Log("Enemy max HP: " + enemyMaxHp);

                                // Apply lifesteal to the player
                                playerHPSystem.currentHp = Mathf.Clamp(playerHPSystem.currentHp + enemyMaxHp, 0, playerHPSystem.maxHp);

                                // Recalculate the player's HP bar
                                GlobalVariables.Instance.globalRecalculateHP = true;

                                // Debug.Log("Player current HP after lifesteal: " + playerHPSystem.currentHp);
                            }
                        }
                        else
                        {
                            // Debug.Log("Life steal on enemy kill is disabled.");
                        }
                    }

                    // Debug.Log("Player XP: " + playerXPSystem.currentOverallXP);

                    if (!unit.isBoss)
                    {
                        playerXPSystem.AddXP(120);
                    }
                    else
                    {
                        playerXPSystem.AddXP(220);
                    }
                }
            }

        }
    }
}