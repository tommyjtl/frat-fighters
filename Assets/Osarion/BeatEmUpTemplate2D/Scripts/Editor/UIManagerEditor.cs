using UnityEditor;
using UnityEngine;

namespace BeatEmUpTemplate2D {

    //Editor script for extending the UIManager Inspector (adds button to documentation at the bottom)
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor {

        string newLine = "\n\n"; //using double lines for better readability

        public override void OnInspectorGUI() {

            DrawDefaultInspector();

            GUILayout.Space(10);

            //button for more information about this component
            if(GUILayout.Button("Click here for more information about this component", GUILayout.Height(30))){
                string title = "UI Manager";
                string content = "Each scene contains a GameObject that holds all the user interfaces, identifiable by the prefix 'UI' (e.g., UI_MainMenu, UI_LevelSelection) in the Hierarchy window. By expanding these GameObjects, you can view all the screens associated with the current scene." + newLine;
                content += "* Note that some menus are located in separate scenes." + newLine;
                content += "For example, when you load the '03_Level1' scene, you will find a GameObject named 'UI' at the top of the Hierarchy. Expanding this GameObject will reveal UI elements such as UIGameOver, UILevelCompleted, UIAllLevelsCompleted, and the player and enemy health bars. You can make changes to these elements, and once you are satisfied, apply the changes to the prefab to ensure they are carried over to all other scenes" + newLine;

                CustomWindow.ShowWindow(title, content, new Vector2(400, 500));
            }
        }

        //shortcut to highlight items
        string highlightItem(string label, int size = 13){
            return "<b><size=" + size + "><color=#FFFFFF>" + label + "</color></size></b>";
        }
    }
}



