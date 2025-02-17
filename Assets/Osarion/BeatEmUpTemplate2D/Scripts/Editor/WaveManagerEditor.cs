using UnityEditor;
using UnityEngine;

namespace BeatEmUpTemplate2D {

    //Editor script for extending the Wave Manager Inspector (adds button to documentation at the bottom)
    [CustomEditor(typeof(WaveManager))]
    public class WaveManagerEditor : Editor {

        string newLine = "\n\n"; //using double lines for better readability

        public override void OnInspectorGUI() {

            DrawDefaultInspector();

            GUILayout.Space(10);

            // button for more information about this component
            if(GUILayout.Button("Click here for more information about this component", GUILayout.Height(30))){
                string title = "Wave Manager";
                string content = "The Wave Manager oversees level progression by managing the activation of enemy groups, directing the camera’s movement to the right, and determining when all enemies have been defeated, thus signaling the end of a level." + newLine + "The Wave Manager data updates in real-time during play mode, allowing you to monitor the current state at all times." + newLine;
                content += highlightItem("Total Number Of Waves: ") + "Read only value that shows the total number of waves (during play mode)." + newLine;
                content += highlightItem("Current Wave: ") + "Read only value that shows the current wave (during play mode)." + newLine;
                content += highlightItem("Enemies Left In This Wave: ") + "Read only value that shows the amount of enemies left in this wave (during play mode)." + newLine;
                content += highlightItem("Total Enemies Left: ") + "Read only value that shows the total amount of enemies left in this level (during play mode)." + newLine + "\n";
                content += highlightItem("End Level When All Enemies Are Defeated: ") + "ends the level once all enemies are defeated. Disable this option if you prefer to end the level through other means." + newLine;
                content += highlightItem("Menu to open on Level Finish: ") + "The name of the menu to open when this level is completed." + newLine;
                content += highlightItem("Menu to open on all Levels Completed: ") + "The name of the menu to open when all levels have been completed." + newLine;
                content += highlightItem("Menu to open on Player Death: ") + "The name of the menu to open when the player is defeated." + newLine + "\n";
                 content += "For more information on setting up enemy waves in the level, please refer to the online documentation." + newLine;

                CustomWindow.ShowWindow(title, content, new Vector2(600, 750));
            }
        }

        //shortcut to highlight items
        string highlightItem(string label, int size = 13){
            return "<b><size=" + size + "><color=#FFFFFF>" + label + "</color></size></b>";
        }
    }
}