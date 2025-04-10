using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class RevengeSystem : MonoBehaviour
    {
        public float revengeMeter = 0;
        public float maxRevengeMeter = 5;
        public bool inRevengeMode = false;
        private SpriteRenderer spriteRenderer;
        private EnemyBehaviour enemyBehaviour;

        public bool IsMaxedOut => revengeMeter >= maxRevengeMeter;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyBehaviour = GetComponent<EnemyBehaviour>();

            if (spriteRenderer == null)
                Debug.LogWarning($"{gameObject.name}: No SpriteRenderer found for RevengeSystem!");

            if (enemyBehaviour == null)
                Debug.LogWarning($"{gameObject.name}: No EnemyBehaviour found for RevengeSystem!");
        }

        public void IncreaseRevengeMeter(float amount)
        {
            if (enemyBehaviour != null && !enemyBehaviour.EnableRevengeMode) return; // Disable if revenge mode is off
            if (inRevengeMode) return;

            revengeMeter = Mathf.Min(revengeMeter + amount, maxRevengeMeter);
            UpdateColor();
        }

        public void DecreaseRevengeMeter(float amount)
        {
            if (enemyBehaviour != null && !enemyBehaviour.EnableRevengeMode) return; // Disable if revenge mode is off

            revengeMeter = Mathf.Max(revengeMeter - amount, 0);
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (spriteRenderer != null)
                spriteRenderer.color = Color.Lerp(Color.red, Color.blue, revengeMeter / maxRevengeMeter);
        }

        public void ResetRevengeMeter()
        {
            if (enemyBehaviour != null && !enemyBehaviour.EnableRevengeMode) return; // Disable if revenge mode is off

            revengeMeter = 0;
            inRevengeMode = false;
            UpdateColor();
        }
    }
}
