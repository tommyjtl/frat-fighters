using UnityEngine;

namespace BeatEmUpTemplate2D
{
    public class ZynsPickup : Item
    {
        public GameObject showEffect;
        public override void OnPickUpItem(GameObject target)
        {

            // Trigger power-up state on the player
            StateMachine sm = target?.GetComponent<StateMachine>();
            if (sm != null)
            {
                sm.SetState(new PlayerPowerUp());
            }

            // Show pickup effect + sound
            if (showEffect) Instantiate(showEffect, transform.position, Quaternion.identity);
            AudioController.PlaySFX(pickupSFX, transform.position);

            // Destroy the Zyns item
            Destroy(gameObject);

        }
    }
}
