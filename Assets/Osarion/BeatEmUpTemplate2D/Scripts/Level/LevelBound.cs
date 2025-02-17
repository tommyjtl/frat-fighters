using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for visualizing CameraBounds in the Unity Editor
    public class LevelBound : MonoBehaviour {
   
        public float lineLength = 10;
        public Color lineColor = Color.yellow;
        public bool showDebugLine = true;

        private void OnDrawGizmos(){
            if(showDebugLine){
                Gizmos.color = lineColor;
                Gizmos.DrawLine(transform.position + Vector3.up * lineLength,  transform.position + Vector3.down * lineLength);
            }
        }
    }
}
