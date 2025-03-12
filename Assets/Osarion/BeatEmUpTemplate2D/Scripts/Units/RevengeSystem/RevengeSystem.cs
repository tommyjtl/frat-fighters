using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class RevengeSystem : MonoBehaviour
    {
        public float revengeMeter = 0;
        public float maxRevengeMeter = 5;
        public bool inRevengeMode = false;
        private SpriteRenderer spriteRenderer;

        public bool IsMaxedOut => revengeMeter >= maxRevengeMeter;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                Debug.LogWarning($"{gameObject.name}: No SpriteRenderer found for RevengeSystem!");
        }

        public void IncreaseRevengeMeter(float amount)
        {
            if (inRevengeMode) return;
            revengeMeter = Mathf.Min(revengeMeter + amount, maxRevengeMeter);
            UpdateColor();
        }

        public void DecreaseRevengeMeter(float amount)
        {
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
            revengeMeter = 0;
            inRevengeMode = false;
            UpdateColor();
        }
    }
}