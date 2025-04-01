using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{
    public class PlayerPowerUp : State
    {
        //private UnitActions unit;
        private Animator animator;
        private float originalFixedDeltaTime;
        private bool animationFinished = false;
        private bool slowMoInProgress = false;

        // You can override this with actual buff duration logic later
        public float powerUpSlowMoTimeScale = 0.1f;
        public float animationEndBuffer = 0.1f; // fallback in case animation event fails

        private float originalCamSize;
        private float zoomedCamSize = 2.0f;
        private float zoomDuration = 0.3f;

        public override void Enter()
        {
            Debug.Log("[PowerUpState] Entering power-up state.");

            //unit = GetComponent<UnitActions>();
            animator = unit.animator;

            // Save current fixedDeltaTime
            originalFixedDeltaTime = Time.fixedDeltaTime;

            // Stop player movement
            unit.StopMoving();

            // Slow down time for everything
            Time.timeScale = powerUpSlowMoTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Play animation in unscaled time
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.Play("Defend");


            SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float playerHeight = sr.bounds.size.y;
                float paddingFactor = 0.1f; // higher = more space above/below player
                zoomedCamSize = playerHeight * paddingFactor * 0.5f;
            }


            Camera camera = Camera.main;
            if (camera != null)
            {
                Debug.Log("[PlayerPowerUp] Calling CameraZoom routine");
                originalCamSize = camera.orthographicSize;
                camera.GetComponent<CameraZoom>()?.ZoomTo(zoomedCamSize, zoomDuration);
            }


            // VFX + SFX
            //unit.ShowEffect("ZynsChargeAura");
            unit.ShowEffect("FireHydrantWaterSplash");
            //unit.PlaySFX("ZynsPowerUp");
            unit.StartCoroutine(PlayPowerUpSoundSequence());
            unit.CamShake();

            // Fallback: Start coroutine in case animation event doesn't fire
            float fallbackDuration = unit.GetAnimDuration("PowerUp") + animationEndBuffer;
            unit.StartCoroutine(FallbackExitRoutine(fallbackDuration));
        }

        private IEnumerator PlayPowerUpSoundSequence()
        {
            float delay = 0.25f;
            for (int i = 0; i < 5; i++)
            {
                unit.ShowEffect("FireHydrantWaterSplash");
                unit.PlaySFX("DefendHit");
                unit.CamShake();
                yield return new WaitForSecondsRealtime(delay);
            }
        }

        public void OnPowerUpAnimationFinished()
        {
            if (animationFinished) return;
            animationFinished = true;
            Exit();
            ApplyBuff();
        }

        private void ApplyBuff()
        {
            // This is where you apply the actual buff
            unit.GetComponent<BuffSystem>()?.ApplyZynBuff(5f); // replace 5f with duration if needed
            unit.stateMachine.SetState(new PlayerIdle());
        }

        public override void Exit()
        {
            if (slowMoInProgress) return;
            slowMoInProgress = true;

            // Restore time scale instantly
            Time.timeScale = 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime;

            // Reset animator update mode
            animator.updateMode = AnimatorUpdateMode.Normal;

            Camera camera = Camera.main;
            if (camera != null)
            {
                camera.GetComponent<CameraZoom>()?.ZoomTo(originalCamSize, zoomDuration);
            }
        }

        private IEnumerator FallbackExitRoutine(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            if (!animationFinished)
            {
                OnPowerUpAnimationFinished();
            }
        }
    }
}
