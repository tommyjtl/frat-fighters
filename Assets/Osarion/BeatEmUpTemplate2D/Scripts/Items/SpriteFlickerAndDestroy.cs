using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D {

    //class for flickering a sprite (on and off) and then destroy it
    public class SpriteFlickerAndDestroy : MonoBehaviour {

        public float startDelay = 0;

        IEnumerator Start(){
            yield return new WaitForSeconds(startDelay);

            SpriteRenderer[] allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            if(allSpriteRenderers == null){ Debug.LogError("No SpriteRenders found on " + gameObject.name); yield break; }

            float startTime = Time.time;
            float speed = 15;
            float osc = 0;

            while(true){

                //flicker sprites on/off
			    osc = Mathf.Sin((Time.time - startTime) * speed);
			    speed += Time.deltaTime * 30;
                foreach(SpriteRenderer sr in allSpriteRenderers) sr.enabled = (osc>0);

                if(speed>50) Destroy(gameObject);
                yield return 0;
            }
        }
    }
}
