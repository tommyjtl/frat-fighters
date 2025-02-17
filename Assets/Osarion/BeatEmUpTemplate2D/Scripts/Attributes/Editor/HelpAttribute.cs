using UnityEngine;
using UnityEditor;

namespace BeatEmUpTemplate2D {

    //Editor extension for drawing help messages in the Unity Inspector
    [CustomPropertyDrawer(typeof(HelpAttribute))]
    public class HelpAttributeDrawer : DecoratorDrawer {

        private GUIStyle style;

        public override float GetHeight() {
            return 16f;
        }

        public override void OnGUI(Rect position){

            //get Attribute
            var helpAttribute = attribute as HelpAttribute;
            if(helpAttribute == null) return;

            //set label style and color
            style = GUI.skin.GetStyle("WhiteMiniLabel");
            style.normal.textColor = Color.yellow;

            //set label field
            EditorGUI.LabelField(position, helpAttribute.text, style);
        }
    }
}