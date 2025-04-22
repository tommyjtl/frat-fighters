using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for creating a parallax scrolling effect
    public class BackgroundScrolling : MonoBehaviour {
	
	    public float xOffset = 0; //additional x offset
	    public float yOffset = 0; //additional y offset

	    void LateUpdate() {
            if(!Camera.main) return;
            Vector3 newPosition = Camera.main.transform.position + new Vector3(xOffset,yOffset,0);
            newPosition.z = transform.position.z;
		    transform.position = newPosition; 
	    }
    }
}