using UnityEngine;

namespace BeatEmUpTemplate2D {

    //class for playing a sfx on Start of a scene
    public class PlayAudioOnStart : MonoBehaviour {

        public string audioItemName = "";
        public Transform parentTransform; //optional

        void Start(){
            if(audioItemName.Length > 0) BeatEmUpTemplate2D.AudioController.PlaySFX(audioItemName, transform.position, parentTransform? parentTransform : null);
        }
    }
}