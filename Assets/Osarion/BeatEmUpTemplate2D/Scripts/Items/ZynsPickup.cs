using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class ZynsPickup : Item
    {
        [Header("Zyn Buff Settings")]
        public float zynBuffDuration = 5f;
        public float zynSpeedMultiplier = 5.5f;
        public GameObject showEffect;
        public override void OnPickUpItem(GameObject target)
        {

            // Trigger power-up state on the player
            StateMachine sm = target?.GetComponent<StateMachine>();
            if (sm != null)
            {
                PlayerPowerUp powerUpState = new PlayerPowerUp
                {
                    buffDuration = zynBuffDuration,
                    buffSpeedMultiplier = zynSpeedMultiplier
                };
                sm.SetState(powerUpState);
            }

            // Show pickup effect + sound
            if (showEffect) Instantiate(showEffect, transform.position, Quaternion.identity);
            AudioController.PlaySFX(pickupSFX, transform.position);

            // Destroy the Zyns item
            Destroy(gameObject);

        }
    }
}
