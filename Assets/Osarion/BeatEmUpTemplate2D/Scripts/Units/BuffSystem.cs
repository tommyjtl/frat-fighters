using System.Collections;
using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class BuffSystem : MonoBehaviour
    {

        public GameObject auraObject; // Assign the AuraLayer GameObject in Inspector
        private SpriteRenderer auraRenderer;
        private SpriteRenderer playerRenderer;

        private float originalGlobalSpeed;
        private Coroutine zynBuffRoutine;
        private Coroutine auraFlashRoutine;
        private float buffTimeRemaining = 0f;
        private float speedMultiplier = 0f;
        private bool isZynBuffActive = false;

        void Start()
        {
            if (auraObject == null)
            {
                Debug.Log("[BuffSystem] Aura object is not assigned.");
                return;
            }

            playerRenderer = GetComponent<SpriteRenderer>();
            auraRenderer = auraObject.GetComponent<SpriteRenderer>();
            auraObject.SetActive(false);
        }

        void LateUpdate()
        {
            if (auraObject == null || playerRenderer == null)
                return;

            if (auraObject.activeSelf)
            {
                auraRenderer.sprite = playerRenderer.sprite;
                auraRenderer.flipX = playerRenderer.flipX;

                auraRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
            }
        }

        public void ApplyZynBuff(float duration, float multiplier)
        {
            // Extend or restart duration
            buffTimeRemaining = duration;
            speedMultiplier = multiplier;

            if (!isZynBuffActive)
            {
                zynBuffRoutine = StartCoroutine(ZynBuffRoutine());
            }
            else
            {
                Debug.Log("[BuffSystem] Zyn buff refreshed with new duration.");
            }
        }

        private IEnumerator ZynBuffRoutine()
        {
            Debug.Log("[BuffSystem] Zyn buff activated.");
            isZynBuffActive = true;

            // Cache and apply multiplier once
            originalGlobalSpeed = GlobalVariables.Instance.globalMoveSpeed;
            GlobalVariables.Instance.globalMoveSpeed = originalGlobalSpeed * speedMultiplier;
            auraObject.SetActive(true);

            bool flashStarted = false;

            while (buffTimeRemaining > 0f)
            {
                buffTimeRemaining -= Time.deltaTime;

                // Start flashing when 2 seconds are left
                if (!flashStarted && buffTimeRemaining <= 3f)
                {
                    flashStarted = true;
                    auraFlashRoutine = StartCoroutine(FlashAura());
                }

                yield return null;
            }

            // Stop flashing
            if (auraFlashRoutine != null)
                StopCoroutine(auraFlashRoutine);

            // Reset to original values
            GlobalVariables.Instance.globalMoveSpeed = originalGlobalSpeed;
            auraObject.SetActive(false);
            isZynBuffActive = false;

            Debug.Log("[BuffSystem] Zyn buff ended.");
        }

        private IEnumerator FlashAura()
        {
            SpriteRenderer aura = auraObject.GetComponent<SpriteRenderer>();
            float flashInterval = 0.2f;

            while (true) // we'll stop it externally
            {
                aura.enabled = !aura.enabled;
                yield return new WaitForSeconds(flashInterval);
            }
        }
    }
}
