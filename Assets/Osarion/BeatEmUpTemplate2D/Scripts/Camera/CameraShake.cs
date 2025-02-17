using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D {

    //class for shaking the camera
    [RequireComponent(typeof(CameraFollow))]
    public class CameraShake : MonoBehaviour {

        public AnimationCurve CameraShakeAnimation;
        public float intensity = .15f;
        public float duration = .3f;

        private float yOffset;
        private CameraFollow cf => GetComponent<CameraFollow>();

        //use default settings
        public void ShowCamShake(){
             StartCoroutine(camShakeRoutine(intensity, duration));
        }

        //use custom settings
        public void ShowCamShake(float _intensity, float _duration){
             StartCoroutine(camShakeRoutine(_intensity, _duration));
        }

        IEnumerator camShakeRoutine(float _intensity, float _duration){
            if(CameraShakeAnimation.length == 0) yield break; //do nothing when there is no animation 
            float animCurveDuration = (CameraShakeAnimation[CameraShakeAnimation.length-1].time); //get duration of the animation curve

            //calculate shak animation
            float t=0;
            while(t<animCurveDuration){

                //calculate offset
                yOffset = CameraShakeAnimation.Evaluate(t) * _intensity;

                //send camshake data to CameraFollow component
                if(cf != null) cf.additionalYOffset = yOffset;

                t += Time.deltaTime / _duration;
                yield return 0;
            }
            yOffset = 0;
        }
    }
}