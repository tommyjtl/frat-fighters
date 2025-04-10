using UnityEngine;

namespace BeatEmUpTemplate2D
{
    // Enemy moves immediately to the player and attacks with double power, then exits revenge mode.
    public class EnemyRevengeMoveToTargetAndAttack : State
    {
        private string animationName = "Run";
        private Vector2 maxAttackRange = new Vector2(1.2f, .1f); // Max attack range from target
        private float attackDistance = 1f; // Ideal x distance to stand from target
        private AttackData attack; // The attack this unit will execute
        private float pauseBeforeAttack;
        private UnitSettings settings;

        public EnemyRevengeMoveToTargetAndAttack(AttackData attack)
        {
            this.attack = attack;
        }

        public override void Enter()
        {
            if (!unit.target)
            {
                unit.stateMachine.SetState(new EnemyIdle());
                return;
            }

            pauseBeforeAttack = unit.settings.enemyPauseBeforeAttack;
            unit.TurnToTarget();
        }

        public override void Update()
        {
            if (targetInRange())
            {
                unit.StopMoving();
                unit.animator.Play("Idle");

                if (pauseBeforeAttack > 0)
                {
                    pauseBeforeAttack -= Time.deltaTime;
                    return;
                }

                unit.stateMachine.SetState(new EnemyAttack(attack));
            }
        }

        public override void FixedUpdate()
        {
            if (!unit.target) return;

            bool targetIsGrounded = unit.target.GetComponent<UnitActions>().isGrounded;
            if ((unit.distanceToTarget().y > maxAttackRange.y && targetIsGrounded) || unit.distanceToTarget().x > maxAttackRange.x)
            {
                Vector2 idealPos = getIdealAttackPos();
                Vector2 dirToPos = (idealPos - (Vector2)unit.transform.position).normalized;

                Vector2 wallDistanceCheck = unit.col2D ? (unit.col2D.size / 1.6f) * 1.1f : Vector2.one * .3f;
                if (unit.WallDetected(dirToPos * wallDistanceCheck))
                {
                    unit.stateMachine.SetState(new EnemyIdle());
                    return;
                }

                unit.MoveToVector(dirToPos, unit.settings.moveSpeed * 1.5f);
                unit.animator.Play(animationName);
            }
        }

        private Vector2 getIdealAttackPos()
        {
            Vector2 XDirToTarget = (unit.target.transform.position.x > unit.transform.position.x) ? Vector2.right : Vector2.left;
            return unit.target.GetComponent<UnitActions>().currentPosition - XDirToTarget * attackDistance;
        }

        private bool targetInRange()
        {
            return (unit.distanceToTarget().x < maxAttackRange.x && unit.distanceToTarget().y < maxAttackRange.y);
        }
    }
}
