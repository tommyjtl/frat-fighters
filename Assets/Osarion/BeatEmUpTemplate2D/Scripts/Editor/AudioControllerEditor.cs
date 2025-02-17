using UnityEditor;
using UnityEngine;

namespace BeatEmUpTemplate2D {

    //Editor script for extending the AudioController Inspector (adds button to documentation at the bottom)
    [CustomEditor(typeof(AudioController))]
    public class AudioControllerEditor : Editor {

        string newLine = "\n\n"; //using double lines for better readability

        public override void OnInspectorGUI() {

            DrawDefaultInspector();

            GUILayout.Space(10);

            //button for more information about this component
            if(GUILayout.Button("Click here for more information about this component", GUILayout.Height(30))){
                string title = "Audio Controller";
                string content = "Each scene contains an Audio Controller, which has a list of AudioClips that can be played on audio events. These audio events can be triggered through animation or through code." + newLine;
                content += highlightItem("Audio List - Item Settings") + "" + newLine;
                content += highlightItem("Name: ") + "The name of this audioclip, and reference for playing this sound effect." + newLine;
                content += highlightItem("Volume: ") + "The loudness of this sound effect, ranging from 0.0 to 1.0." + newLine;
                content += highlightItem("Random Volume: ") + "Use this setting if you want variation in volume, each time when this sound if played." + newLine;
                content += highlightItem("Random Pitch: ") + "This setting is used to vary the pitch of a sound effect each time it is played, introducing a degree of randomness." + newLine;
                content += highlightItem("Min Time Between Call: ") + "Controls the minimum amount of time that must elapse between consecutive plays of this sound effect." + newLine;
                content += highlightItem("Range: ") + "Optional setting to adjust the range/distance of this SFX. Set it to 0 to disable range, ensuring the SFX is always audible." + newLine;
                content += highlightItem("Loop: ") + "Whether a sound effect is played continuously in a repeating cycle or just once." + newLine;
                content += highlightItem("Clip: ") + "Reference to the audio file for this clip." + newLine + "\n";

                content += "TIP: If you add multiple clips to an audio item, one will be chosen at random. By pairing this with pitch randomization, you can get more variation." + newLine;
                CustomWindow.ShowWindow(title, content, new Vector2(700, 650));
            }
        }

        //shortcut to highlight items
        string highlightItem(string label, int size = 13){
            return "<b><size=" + size + "><color=#FFFFFF>" + label + "</color></size></b>";
        }
    }
}