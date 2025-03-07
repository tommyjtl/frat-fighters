using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUpTemplate2D
{

    //class for unit or objects XP bar in the User Interface
    public class UIHUBXPBar : MonoBehaviour
    {

        public Text xpValue;
        public Text spValue;
        public Image xpBar;

        private bool initialized; // is the xpbar initialized?

        void OnEnable()
        {
            XPSystem.onXPChange += UpdateXP; //subscribe to xp update events
        }

        void OnDisable()
        {
            XPSystem.onXPChange -= UpdateXP; //unsubscribe to xp update events
        }

        void UpdateXP(XPSystem xs)
        {
            if (xpBar == null) return;

            if (!initialized) InitializeXpBar(xs); //this is only done once at the start of the level

            xpBar.fillAmount = xs.stageXpPercentage;

            // xs.currentSP is of int type, need to convert to unity text
            spValue.text = xs.currentSP.ToString();
        }

        //load player data on initialize
        void InitializeXpBar(XPSystem xs)
        {
            // nameField.text = hs.GetComponent<UnitSettings>().unitName; //get name
            // if(hs.GetComponent<UnitSettings>().showNameInAllCaps) nameField.text = nameField.text.ToUpper(); //show in capital letters
            initialized = true;
        }

        //show or hide this xpbar
        void ShowXpBar(bool state)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = state ? 1f : 0f;
        }
    }
}
