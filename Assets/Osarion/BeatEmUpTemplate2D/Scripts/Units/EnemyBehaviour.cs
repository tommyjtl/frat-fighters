using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class EnemyBehaviour : MonoBehaviour
    {
        public bool AI_Active;
        public float delayBeforeStart = 1f;
        public float decisionInterval = 1f;
        public float randomizeAmount = 1f; // Value to randomize (add to) decision time
        private float lastDecisionTime = 0;
        private float startTime = 0;
        [ReadOnlyProperty] public bool targetSpotted;
        private StateMachine statemachine;
        private UnitSettings settings;
        private RevengeSystem revengeSystem;
        private bool inRevengeState = false;
        private float revengeDuration = 5f;

        void Start()
        {
            startTime = Time.time;
            statemachine = GetComponent<StateMachine>();
            settings = GetComponent<UnitSettings>();
            revengeSystem = GetComponent<RevengeSystem>();
        }

        void Update()
        {
            // Delay before start
            if (Time.time - startTime < delayBeforeStart) return;

            // The target has been spotted
            if (!targetSpotted)
            {
                if (statemachine.targetInSight()) targetSpotted = true;
            }

            // If revenge is active, decrement timer and reset when done
            if (inRevengeState)
            {
                decisionInterval = 0.5f * decisionInterval; // Halve the attack interval
                revengeDuration -= Time.deltaTime;

                if (revengeDuration <= 0)
                {
                    revengeSystem.ResetRevengeMeter();
                    inRevengeState = false;
                    decisionInterval = settings.enemyPauseBeforeAttack; // Reset to normal after revenge ends
                }

                return;
            }

            // Decision time
            if (targetSpotted && Time.time - lastDecisionTime > decisionInterval) DoSomething();
        }

        void DoSomething()
        {
            if (!AI_Active) return; // Do nothing
            else lastDecisionTime = Time.time + Random.Range(0f, randomizeAmount);

            // Get a random attack from Unit Settings
            AttackData attack = GetRandomAttack();

            // Check if this enemy is currently in EnemyIdle state
            bool isIdle = (statemachine.GetCurrentState() is EnemyIdle);
            if (!isIdle) return; // Do nothing

            // If revenge system exists and revenge meter is maxed out, enter revenge mode
            if (revengeSystem != null && revengeSystem.IsMaxedOut && !inRevengeState)
            {
                inRevengeState = true;
                revengeDuration = 5f;
                attack.damage *= 2;
                statemachine.SetState(new EnemyRevengeMoveToTargetAndAttack(attack));
                return;
            }

            // 100% chance to attack if revenge mode is active
            if (revengeSystem != null && revengeSystem.inRevengeMode)
            {
                statemachine?.SetState(new EnemyRevengeMoveToTargetAndAttack(attack));
                return;
            }

            // 75% chance to attack if nobody is attacking the player
            if (EnemyManager.GetEnemyAttackerCount() == 0 && Random.Range(0, 100) < 75)
            {
                statemachine?.SetState(new EnemyMoveToTargetAndAttack(attack));
                return;
            }

            // 25% chance that this enemy attacks when 2 or fewer enemies are attacking the player
            else if (EnemyManager.GetEnemyAttackerCount() <= 2 && Random.Range(0, 100) < 25)
            {
                statemachine?.SetState(new EnemyMoveToTargetAndAttack(attack));
                return;
            }

            // Otherwise, random action 1 or 2
            int i = Random.Range(1, 3);

            // Move closer to player
            if (i == 1) statemachine?.SetState(new EnemyKeepDistance(1f, 1f, -.5f, .5f));

            // Move away from player
            if (i == 2) statemachine?.SetState(new EnemyKeepDistance(4f, 4f, -1f, 1f));
        }

        private AttackData GetRandomAttack()
        {
            // Check available data
            if (settings == null || settings.enemyAttackList.Count == 0)
            {
                Debug.Log("No enemy attacks available to choose from");
                return null;
            }

            // Select a random attack
            int rand = Random.Range(0, settings.enemyAttackList.Count);
            return settings.enemyAttackList[rand];
        }
    }
}
