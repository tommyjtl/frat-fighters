using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for creating a parallax scrolling effect
    public class ParalaxScrolling : MonoBehaviour {
	
	    public float ParallaxScale = -.3f; //the amount off parallax
	    public float xOffset = 0; //additional x offset
	    public float yOffset = 0; //additional y offset
        public bool useX = true; //disable/enable the X Axis;
        public bool useY = true; //disable/enable the Y Axis;

        private Vector2 prevPos = Vector2.zero;
        private float xPos = 0;
        private float yPos = 0;

	    void Update() {
            if(!useX && !useY || !Camera.main) return;
		    Vector2 diff = (Vector2) Camera.main.transform.position - prevPos;

            xPos = useX? (diff.x * ParallaxScale) + xOffset : transform.position.x; //determine the x position
            yPos = useY? (diff.y * ParallaxScale) + yOffset : transform.position.y; //determine the y position

		    transform.position = new Vector3(xPos, yPos, transform.position.z); //set position
	    }
    }
}