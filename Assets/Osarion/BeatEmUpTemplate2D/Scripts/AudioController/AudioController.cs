using UnityEngine;

namespace BeatEmUpTemplate2D {
    
    //class for playing audio
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour {

        public static AudioController Instance { get; private set; }

	    public AudioItem[] AudioList;
	    private AudioSource source;
	    private float sfxVolume = 1f;
        private static AudioController controller;

	    void Awake(){
            source = GetComponent<AudioSource>();

            //singleton pattern (only one AudioController allowed in a scene)
            if (Instance == null){ 
                Instance = this;

            } else {
                Debug.Log("Multiple AudioControllers found in this scene, there can be only one.");
                Destroy(gameObject);
            }
	    }

        //play a Sound Effect
        public static void PlaySFX(string name = "", Vector3? pos = null, Transform parent = null){
            if (string.IsNullOrEmpty(name)) return; //nothing to play

            //if no position is given, play at camera position
            Vector3 finalPos = pos ?? Camera.main.transform.position;
            
            //if no parent if given, set camera as parent, so the sound is always heard
            Transform finalParent = parent ?? Camera.main.transform; 

            //play audio item
            if(Instance) Instance.PlayAudioItem(name, finalPos, finalParent);
        }

        //returns the duration of a sfx
        public static float GetSFXDuration(string name){
            foreach(AudioItem audioItem in Instance.AudioList){
			    if(audioItem.name == name) return audioItem.clip.Length;
            }
            return 0;
        }

	    //play a sfx (parameters: name, position, parent)
	    private void PlayAudioItem(string name, Vector3 worldPosition, Transform parent){
            if(name.Length == 0) return;
		    bool SFXFound = false;
		    foreach(AudioItem audioItem in AudioList){
			    if(audioItem.name == name){
                    if(audioItem.clip.Length == 0){ Debug.Log("AudioClip '" + name + "' is missing, please add an audioclip in the AudioController to play a sfx"); return; }

				    //check the time threshold
				    if (Time.time - audioItem.lastTimePlayed < audioItem.minTimeBetweenCall) {
					    return;
				    } else {
					    audioItem.lastTimePlayed = Time.time;
				    }

				    //create gameobject for the audioSource
				    GameObject audioObj = new GameObject("AudioObj_" + name);
				    audioObj.name = name;
                    audioObj.transform.parent = audioItem.range == 0? Camera.main.transform : parent;
				    audioObj.transform.position = worldPosition;
				    AudioSource audiosource = audioObj.AddComponent<AudioSource>();

				    //set audio source settings
                    int rand = Random.Range (0, audioItem.clip.Length);
				    audiosource.clip = audioItem.clip[rand];
				    audiosource.spatialBlend = 1.0f;
                    audiosource.pitch = 1f + Random.Range(-audioItem.randomPitch, audioItem.randomPitch);
				    audiosource.volume = audioItem.volume * sfxVolume + Random.Range(-audioItem.randomVolume, audioItem.randomVolume);
				    audiosource.outputAudioMixerGroup = source.outputAudioMixerGroup;
                    audiosource.rolloffMode = AudioRolloffMode.Custom;
				    audiosource.loop = audioItem.loop;
                    if(audioItem.range > 0) audiosource.maxDistance = audioItem.range;
                    if(audioItem.range > 3) audiosource.minDistance = audiosource.maxDistance - 3;
				    audiosource.Play();

				    //Destroy on finish
				    if(!audioItem.loop && audiosource.clip != null) Destroy(audioObj, audiosource.clip.length + Time.deltaTime);					
				    SFXFound = true;
			    }
		    }
		    if (!SFXFound) Debug.Log ("No Audio Item found with name: " + name);
	    }
    }
}