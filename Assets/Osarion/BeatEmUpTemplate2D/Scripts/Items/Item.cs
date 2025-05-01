using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D {

    //generic item class
    public class Item : ObjectSorting {

        [Header("Item Data")]
        public string itemName = "";
        public string pickupSFX;
    
        [Header("Bounce Animation")]
        public AnimationCurve bounceCurve;
        public float bounceHeight = 2;
        public float bounceSpeed = 1;
        public bool playBounceAnimation;
        private bool bounceInProgress;
        
        public override void Start(){
            base.Start();

            //show bounce animation
            if(playBounceAnimation) ShowBounceAnimation();
        }

        public virtual void OnPickUpItem(GameObject target){}
        public virtual void OnStandUp(GameObject target){}

        public void ShowBounceAnimation(){
            if(!bounceInProgress) StartCoroutine(ItemBounceRoutine());
        }

        //bounce animation routine
        IEnumerator ItemBounceRoutine(){
            bounceInProgress = true;
            if(bounceCurve.length == 0) yield break; //do nothing when there is no bounce animation
            float duration = (bounceCurve[bounceCurve.length-1].time) * bounceSpeed; //get duration of the animation

            //play bounce animation
            Vector3 startPos = transform.position;
            float t=0;
            while(t<duration){
                transform.position = startPos + Vector3.up * bounceCurve.Evaluate(t) * bounceHeight;
                t += Time.deltaTime / duration;
                yield return 0;
            }

            transform.position = startPos; //return to original position
            bounceInProgress = false;
        }

    }
}