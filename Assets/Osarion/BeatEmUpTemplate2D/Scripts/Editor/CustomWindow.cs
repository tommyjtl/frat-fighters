using UnityEngine;
using UnityEditor;

namespace BeatEmUpTemplate2D {

    //custom Window class for displaying help and documentation information
    public class CustomWindow : EditorWindow {

        private string windowTitle;
        private string content;
        private string url = "https://www.osarion.com/BeatEmUpTemplate2D/documentation.html";
        private int padding = 25;

        public static void ShowWindow(string title, string content, Vector2 size) {
            CustomWindow window = GetWindow<CustomWindow>(title);

            window.windowTitle = title;
            window.content = content;
            window.Repaint();

            //set window size
            window.minSize = size;
            window.maxSize = new Vector2(1024, 1024);

            //put window in screen center position
            Vector2 screenCenter = new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
            Vector2 windowSize = size;
            Vector2 windowPosition = screenCenter - (windowSize / 2);
            window.position = new Rect(windowPosition.x, windowPosition.y, windowSize.x, windowSize.y);

            //set the window's position and size
            window.position = new Rect(windowPosition.x, windowPosition.y, windowSize.x, windowSize.y);
        }

        private void OnGUI() {

            //title
            ShowTitle(windowTitle);

            //show content if it exists
            if (!string.IsNullOrEmpty(content)) {
                EditorGUILayout.TextArea(content, labelStyle(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }

            //show footeLink to documentation
            EditorGUILayout.Space(10);
            ShowTitle("Documentation");
            GUILayout.Label("For detailed documentation, FAQ, tutorials, and videos, please visit the website:", labelStyle());

            //button to website
            if (GUILayout.Button(new GUIContent("Online Documentation", "Open link"), buttonStyle())) {
                Application.OpenURL(url);
            }
        }

        //style
        GUIStyle buttonStyle() {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = new Color(1, 1, 1, .6f);
            style.hover.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            style.margin = new RectOffset(padding, padding, 0, 10);
            style.fixedHeight = 40;
            return style;
        }

        //style for labels
        GUIStyle labelStyle(bool bold = false) {
            GUIStyle style = bold? new GUIStyle(EditorStyles.boldLabel) : new GUIStyle(EditorStyles.label);
            style.wordWrap = true;
            style.richText = true;
            style.padding = new RectOffset(padding, padding, 0, 0);
            style.alignment = TextAnchor.UpperLeft;
            return style;
        }
 
        //title void
        void ShowTitle(string label){
            string richText = $"<b><size=14><color=#FFFFFF>{label}</color></size></b>";
           GUILayout.Label(richText, titleStyle());
        }

        //style for titles
        GUIStyle titleStyle() {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.wordWrap = true;
            style.richText = true;
            style.padding = new RectOffset(padding, padding, padding, 0);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 14;
            style.richText = true;
            return style;
        }
    }
}