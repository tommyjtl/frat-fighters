using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{
    public class PlayerPowerUp : State
    {


        public float buffSpeedMultiplier = 2f;
        public float buffDuration = 5f;

        private Animator animator;
        private float originalFixedDeltaTime;

        public float powerUpSlowMoTimeScale = 0.001f;
        private string animationName = "PowerUp";
        private float animDuration => (unit.GetAnimDuration(animationName) * powerUpSlowMoTimeScale);

        //private float zoomedCamSize = 2.0f;
        //private float zoomDuration = 1.3f;
        //private float zoomHoldTime = 0f; // how long to stay zoomed in

        public override void Enter()
        {
            //Debug.Log("[PowerUpState] Entering power-up state.");

            //// Disable control while powering up
            //InputManager.playerControlEnabled = false;

            // Make player invulnerable + knockdown-immune
            AudioController.PlaySFX("ZYNConsumption");

            HealthSystem health = unit.GetComponent<HealthSystem>();
            if (health != null)
                health.invulnerable = true;

            UnitSettings settings = unit.GetComponent<UnitSettings>();
            if (settings != null)
                settings.canBeKnockedDown = false;

            InputManager.playerControlEnabled = false;

            animator = unit.animator;

            originalFixedDeltaTime = Time.fixedDeltaTime;
            unit.StopMoving();




            // Apply slow-mo
            Time.timeScale = powerUpSlowMoTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.Play("PowerUp");

            //// Calculate zoom size based on player height
            //SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();
            //if (sr != null)
            //{
            //    float playerHeight = sr.bounds.size.y;
            //    float paddingFactor = 2.1f;
            //    zoomedCamSize = playerHeight * paddingFactor * 1.2f;
            //}

            //// Call zoom-in-out camera
            //Camera camera = Camera.main;
            //if (camera != null)
            //{
            //    Debug.Log("[PlayerPowerUp] Calling CameraZoom ZoomInOut");
            //    camera.GetComponent<CameraZoom>()?.ZoomInOut(
            //        zoomedCamSize,
            //        zoomDuration,
            //        zoomHoldTime,
            //        unit.transform.position
            //    );
            //}

            // VFX + SFX
            //unit.ShowEffect("FireHydrantWaterSplash");
            //unit.StartCoroutine(PlayPowerUpSoundSequence());
            //unit.CamShake();
        }


        //private IEnumerator PlayPowerUpSoundSequence()
        //{
        //    float delay = 0.25f;
        //    for (int i = 0; i < 5; i++)
        //    {
        //        //unit.ShowEffect("FireHydrantWaterSplash");
        //        unit.PlaySFX("DefendHit");
        //        unit.CamShake();
        //        yield return new WaitForSecondsRealtime(delay);
        //    }
        //}


        public override void Exit()
        {

            Time.timeScale = 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime;
            unit.animator.updateMode = AnimatorUpdateMode.Normal;

            // No need to call ZoomTo() here � the camera will zoom back automatically

            InputManager.playerControlEnabled = true;

            // Restore normal vulnerability and knockdown
            HealthSystem health = unit.GetComponent<HealthSystem>();
            if (health != null)
                health.invulnerable = false;

            UnitSettings settings = unit.GetComponent<UnitSettings>();
            if (settings != null)
                settings.canBeKnockedDown = true;
        }

        public override void Update(){
            if(Time.time - stateStartTime > animDuration)  {
                Exit();
                unit.GetComponent<BuffSystem>()?.ApplyZynBuff(buffDuration, buffSpeedMultiplier);
                unit.stateMachine.SetState(new PlayerIdle()); //go to idle state
            }
        }
    }
}
