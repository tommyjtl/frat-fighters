using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUpTemplate2D {

    //class that fades in or out an image in the User Interface
    public class UIFader : MonoBehaviour {
    
        [SerializeField] private Image imageToFade;
        [SerializeField] private float delayBeforeStart;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] FADETYPE fadeType = FADETYPE.FADEIN;
        private enum FADETYPE { FADEIN, FADEOUT }

        [Header("Actions on Start / Finish")]
        [SerializeField] GameObject[] DisableOnStart;
        [SerializeField] GameObject[] EnableOnFinish;

        private bool fadeInProgress;

        void OnEnable() {
            if(!fadeInProgress) StartCoroutine(StartFade());
        }

        IEnumerator StartFade() {
            fadeInProgress = true;

            //disable these gameObjects on start
            foreach(GameObject g in DisableOnStart) g.SetActive(false);

            //set start and end color
            Color fromColor = imageToFade.color;
            Color toColor = imageToFade.color;

            //set alpha depending on FADETYPE
            fromColor.a = (fadeType == FADETYPE.FADEIN)? 0f : imageToFade.color.a; //set start transparancy
            toColor.a = (fadeType == FADETYPE.FADEIN)? imageToFade.color.a : 0; //set end transparancy

            //delay before start
            imageToFade.color = fromColor;
            yield return new WaitForSeconds(delayBeforeStart);

            //transition fromColor -> toColor
            float t=0;
            while(t<1){
                imageToFade.color = Color.Lerp(fromColor, toColor, t);
                t += Time.deltaTime/fadeDuration;
                yield return 0;
            }

            //ensure end color
            imageToFade.color = toColor;

            //enable these gameObjects on finish
            foreach(GameObject g in EnableOnFinish) g.SetActive(true);

            fadeInProgress = false;
        }
    }
}
