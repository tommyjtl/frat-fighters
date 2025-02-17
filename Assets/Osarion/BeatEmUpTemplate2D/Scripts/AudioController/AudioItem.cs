using UnityEngine;

namespace BeatEmUpTemplate2D {

    //audioItem class for use with the AudioController
    [System.Serializable]
    public class AudioItem {

	    public string name;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0f, 1f)] public float randomVolume = 0f;
        [Range(0f, 1f)] public float randomPitch = 0f;
        public float minTimeBetweenCall = .1f;
        public float range = 0f; //use a value of 0 to disable distance-based attenuation and always play a sfx
	    public bool loop;
	    public AudioClip[] clip;
	    [HideInInspector] public float lastTimePlayed = 0;
    }
}