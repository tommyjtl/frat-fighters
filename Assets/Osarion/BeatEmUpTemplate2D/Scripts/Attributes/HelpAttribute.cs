using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class HelpAttribute : PropertyAttribute {
        public string text;
        public HelpAttribute(string text) {
            this.text = text;
        }
    }
}