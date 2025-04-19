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
        private float buffTimeRemaining = 0f;
        private float speedMultiplier = 0f;
        private bool isZynBuffActive = false;

        void Start()
        {
            playerRenderer = GetComponent<SpriteRenderer>();
            auraRenderer = auraObject.GetComponent<SpriteRenderer>();
            auraObject.SetActive(false);
        }

        void LateUpdate()
        {
            if (auraObject.activeSelf)
            {
                auraRenderer.sprite = playerRenderer.sprite;
                auraRenderer.flipX = playerRenderer.flipX;
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

            while (buffTimeRemaining > 0f)
            {
                buffTimeRemaining -= Time.deltaTime;
                yield return null;
            }

            // Reset to original values
            GlobalVariables.Instance.globalMoveSpeed = originalGlobalSpeed;
            auraObject.SetActive(false);
            isZynBuffActive = false;

            Debug.Log("[BuffSystem] Zyn buff ended.");
        }
    }
}
