using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class BuffSystem : MonoBehaviour
    {
        private Coroutine zynBuffRoutine;
        private bool isZynBuffActive = false;

        [Tooltip("Speed multiplier during Zyn buff")]
        public float zynSpeedMultiplier = 5.5f;

        public void ApplyZynBuff(float duration)
        {
            if (isZynBuffActive)
            {
                StopCoroutine(zynBuffRoutine);
            }
            zynBuffRoutine = StartCoroutine(ZynBuffRoutine(duration));
        }

        private IEnumerator ZynBuffRoutine(float duration)
        {
            Debug.Log("[BuffSystem] Zyn buff activated for " + duration + " seconds.");

            isZynBuffActive = true;

            // Apply speed buff
            UnitActions unit = GetComponent<UnitActions>();
            float originalSpeed = unit.settings.moveSpeed;
            unit.settings.moveSpeed *= zynSpeedMultiplier;

            // TODO: Send UI signal to show timer here

            float timeRemaining = duration;
            while (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                // TODO: Send UI update to reflect remaining time
                yield return null;
            }

            // Revert speed buff
            unit.settings.moveSpeed = originalSpeed;
            isZynBuffActive = false;

            // TODO: Send UI signal to hide timer here
            Debug.Log("[BuffSystem] Zyn buff ended.");
        }
    }
}

