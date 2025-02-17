using UnityEditor;
using UnityEngine;

namespace BeatEmUpTemplate2D {

    //Editor script for extending the InputManager Inspector (adds button to documentation at the bottom)
    [CustomEditor(typeof(InputManager))]
    public class InputManagerEditor : Editor {

        string newLine = "\n\n"; //using double lines for better readability

        public override void OnInspectorGUI() {

            DrawDefaultInspector();

            GUILayout.Space(10);

            //button for more information about this component
            if(GUILayout.Button("Click here for more information about this component", GUILayout.Height(30))){
                string title = "Input Manager";
                string content = "The Input Manager handles both keyboard and joystick inputs. It also provides options for customizing key and button mappings." + newLine;
                
                content += highlightItem("How to change controls \n");
                content +="Go to Osarion/BeatEmUpTemplate2D/Scripts/Input/PlayerControls." + newLine;
                content += "In the Inspector window, you'll see a button named 'Edit Asset', click on it to open a window where you can set up the button mappings for the entire project.\n";

                CustomWindow.ShowWindow(title, content, new Vector2(700, 500));
            }
        }

        //shortcut to highlight items
        string highlightItem(string label, int size = 13){
            return "<b><size=" + size + "><color=#FFFFFF>" + label + "</color></size></b>";
        }
    }
}