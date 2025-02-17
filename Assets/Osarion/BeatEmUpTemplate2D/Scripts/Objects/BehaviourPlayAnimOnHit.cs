using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for playing an animation on Hit
    [RequireComponent(typeof(Animator))]
    public class BehaviourPlayAnimOnHit : MonoBehaviour {

        public string hitAnimation = "Hit";

        void OnEnable() {
		    UnitActions.onUnitDealDamage += OnHitObject; //subscribe to event
	    }

	    void OnDisable() {
		    UnitActions.onUnitDealDamage -= OnHitObject; //unsubscribe to event
	    }

        //when this object was hit, play hit animation
        void OnHitObject(GameObject obj, AttackData attackData){
            if(obj == this.gameObject){

                Animator animator = GetComponent<Animator>();
                if(animator){
                    animator.Play(hitAnimation, 0, 0f); //play animation from start

                } else {
                    Debug.Log("Animator component could not be found on " + gameObject.name);
                }

            }
        }
    }
}
