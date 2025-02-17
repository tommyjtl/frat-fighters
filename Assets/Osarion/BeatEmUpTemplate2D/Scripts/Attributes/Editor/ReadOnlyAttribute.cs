using UnityEngine;
using UnityEditor;

namespace BeatEmUpTemplate2D {

    //Editor extension for showing read only variables in the Unity Inspector
    [CustomPropertyDrawer(typeof(ReadOnlyProperty))]
    public class ReadOnlyAttribute : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
            string valueStr;

            switch (prop.propertyType) {
                case SerializedPropertyType.Integer:
                    valueStr = prop.intValue.ToString();
                    break;

                case SerializedPropertyType.Boolean:
                    valueStr = prop.boolValue.ToString();
                    break;

                case SerializedPropertyType.Float:
                    valueStr = prop.floatValue.ToString("0.00");
                    break;

                case SerializedPropertyType.String:
                    valueStr = prop.stringValue;
                    break;

                default:
                    valueStr = "(not supported)";
                    break;
            }

            EditorGUI.LabelField(position,label.text, valueStr);
        }
    }
}
