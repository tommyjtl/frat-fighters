using UnityEditor;
using UnityEngine;

namespace BeatEmUpTemplate2D
{

    //Editor script for extending the HealthSystem Inspector (adds button to documentation)
    [CustomEditor(typeof(HealthSystem))]
    public class HealthSystemEditor : Editor
    {

        string newLine = "\n\n"; //using double lines for better readability

        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            GUILayout.Space(10);

            // button for more information about this component
            if (GUILayout.Button("Click here for more information about this component", GUILayout.Height(30)))
            {
                string title = "Health System";
                string content = "The Health System is utilized by various units, including both Player and Enemy units, as well as objects like wooden crates and drum barrels. It tracks the maximum and current health of each unit or object." + newLine;

                content += highlightItem("Settings Overview" + newLine);
                content += highlightItem("Max Hp: ") + "The maximum number of health points that this unit/object has." + newLine;
                content += highlightItem("Current Hp: ") + "The current number of health points that this unit/object has. The object/unit is destroyed when it reaches 0." + newLine;
                content += highlightItem("Invulnerable: ") + "A unit/object cannot be destroyed or receive damage when it is invulneable." + newLine;
                content += highlightItem("Show Small Health Bar: ") + "An option to show a small healthbar near this unit in the game." + newLine;
                content += highlightItem("Small Health Bar Offset: ") + "The position of the small healthbar." + newLine;
                content += highlightItem("Play SFX On Hit: ") + "The sound effect that is played when this object/unit is hit." + newLine;
                content += highlightItem("Play SFX On Destroy: ") + "The sound effect that is played when this object/unit is destroyed." + newLine;
                content += highlightItem("Show Hit Flash: ") + "If this unit/object should flash to a white color when it is hit." + newLine;
                content += highlightItem("Hit Flash Duration: ") + "The duration of the hit flash." + newLine;
                content += highlightItem("Show Shake Effect: ") + "If this object should do a shake effect." + newLine;
                content += highlightItem("Shake Intensity: ") + "The size of the shake effect." + newLine;
                content += highlightItem("Shake Duration: ") + "The duration (in seconds) of the shake effect." + newLine;
                content += highlightItem("Shake speed: ") + "The speed of he shake effect." + newLine;
                content += highlightItem("Show Effect On Hit: ") + "Effect shown when this object/unit is hit." + newLine;
                content += highlightItem("Show Effect On Destroy: ") + "Effect shown when this object/unit is destroyed" + newLine;
                CustomWindow.ShowWindow(title, content, new Vector2(600, 750));
            }
        }

        //shortcut to highlight items
        string highlightItem(string label, int size = 13)
        {
            return "<b><size=" + size + "><color=#FFFFFF>" + label + "</color></size></b>";
        }
    }
}