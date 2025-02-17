using UnityEditor;
using UnityEngine;

namespace BeatEmUpTemplate2D {

    //Editor script for extending the CameraFollow component Inspector (adds button to documentation at the bottom)
    [CustomEditor(typeof(CameraFollow))]
    public class CameraFollowEditor : Editor {

        string newLine = "\n\n"; //using double lines for better readability

        public override void OnInspectorGUI() {

            DrawDefaultInspector();

            GUILayout.Space(10);

            // button for more information about this component
            if(GUILayout.Button("Click here for more information about this component", GUILayout.Height(30))){
                string title = "Camera Follow Component";
                 string content = "This component is designed to dynamically follow targets, usually the player, ensuring they remain centered or within a specified area on the screen. Additionally, the system offers an option to restrict players from moving outside the camera's view, creating boundaries that prevent off-screen navigation and keeping the player within the visual field at all times. Furthermore, the camera has adjustable controls over the visible area on the screen." + newLine;

                content += highlightItem("Player Targets: ") + newLine;
                content += highlightItem("Targets: ") + "The target to follow. If there are multiple targets, the camera's follow point will be the center position of all these targets combined. At the start of the level, the camera will try to find all gameObjects tagged as 'Player' and assign them as the follow target." + newLine;
                content += highlightItem("Restrict Targets To Cam View: ") + "If the targets should be kept inside of the camera view area." + newLine;
                content += highlightItem("Border Margin: ") + "The border margin can be use to increase/decrease the restricted area." + newLine + "\n";
               
                content += highlightItem("Follow Settings: ") + newLine;
                content += highlightItem("Y Ofset: ") + "A variable to position the camera slightly higher (or lower) than it's exact follow position." + newLine;
                
                content += highlightItem("Damp Settings: ") + "\n";
                content += "Dampening refers to the smoothing of camera movement as it tracks a target. Instead of instantly snapping to the target's position, the camera moves gradually, creating a more fluid and natural motion." + newLine;
                content += highlightItem("Damp X: ") + "The smoothing of the camera in horizontal movement." + newLine;
                content += highlightItem("Damp Y: ") + "The smoothing of the camera in vertical movement." + newLine;

                content += highlightItem("View Area:") + "\n";
                content += "The View Area refers to the portion of the game world that is visible on the screen at any given time. Setting the boundaries of the player's visual field by setting the Left, Right, Top and Bottom area of the screen." + newLine;
               
                content += highlightItem("Backtracking: ") + "\n"; 
                content += "Backtracking refers to the player's ability to move backward within a level. In a sidescrolling Beat 'em up game, the camera typically moves from left to right. When allow backtracking is disabled, the camera is prevented from moving backward, requiring the player to continue moving forward toward the end of the level."+ newLine;

                content += highlightItem("Allow Backtracking: ") + "Enable or disable the ability for the camera to move back." + newLine;
                content += highlightItem("Back Track Margin: ") + "A buffer zone that provides space for the camera to move slightly backwards." + newLine;

                content += highlightItem("Level Bounds: ");
                content += "Specific points in a game level where players must defeat a certain number of enemies before they can advance. These bounds are set by the Wave Manager during play mode." + newLine;
               
                CustomWindow.ShowWindow(title, content, new Vector2(1024, 850));
            }
        }

        //shortcut to highlight items
        string highlightItem(string label, int size = 13){
            return "<b><size=" + size + "><color=#FFFFFF>" + label + "</color></size></b>";
        }
    }
}