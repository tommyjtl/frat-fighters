using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D {

    //class for creating scripted animations in y position using animation curves
    public class AnimationCurveAnim : MonoBehaviour {

        [Header("Y Position")]
        public AnimationCurve AnumCurveY;
        public float ScaleY = 1;

        [Header("X Position")]
        public AnimationCurve AnumCurveX;
        public float ScaleX = 1;

        [Header("Settings")]
        public float Duration = 1;

        void  Start(){
            StartCoroutine(PlayBounceAnim());
        }

        //bounce anim routine
        IEnumerator PlayBounceAnim(){
            if(AnumCurveY.length == 0) yield break; //do nothing when there is no bounce animation 
            float duration = Mathf.Max(AnumCurveX[AnumCurveX.length-1].time * Duration, AnumCurveY[AnumCurveY.length-1].time * Duration);
            Vector3 startPos = transform.position;
        
            //play animation
            float t=0;
            while(t<duration){
                transform.position = startPos + (Vector3.up * AnumCurveY.Evaluate(t) * ScaleY) + (Vector3.right * AnumCurveX.Evaluate(t) * ScaleX) ;
                t += Time.deltaTime / duration;
                yield return 0;
            }
        }
    }
}