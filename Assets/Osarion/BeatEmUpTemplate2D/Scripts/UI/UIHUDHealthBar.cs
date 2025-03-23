using UnityEngine;
using UnityEngine.UI;

namespace BeatEmUpTemplate2D
{

    //class for unit or objects healthbars in the User Interface
    public class UIHUDHealthBar : MonoBehaviour
    {

        private enum HEALTHBARTYPE { PlayerHealthBar, EnemyHealthBar, BossHealthBar }
        [SerializeField] private HEALTHBARTYPE healthBarType = HEALTHBARTYPE.PlayerHealthBar;
        public Text nameField;
        public Image portrait;
        public Image healthBar;
        private bool initialized;

        void OnEnable()
        {
            HealthSystem.onHealthChange += UpdateHealth; //subscribe to health update events
            if (healthBarType == HEALTHBARTYPE.EnemyHealthBar) ShowHealthBar(false); //hide enemy healthbar by default

            //check portrait reference
            if (portrait == null) Debug.Log("no portrait image was linked in the UIHealthbar");
        }

        void OnDisable()
        {
            HealthSystem.onHealthChange -= UpdateHealth; //unsubscribe to health update events
        }

        void UpdateHealth(HealthSystem hs)
        {
            if (healthBar == null) return;

            //update player healthbar
            if (healthBarType == HEALTHBARTYPE.PlayerHealthBar && hs.isPlayer)
            {
                if (!initialized) InitializePlayerBar(hs); //this is only done once at the start of the level
                healthBar.fillAmount = hs.healthPercentage;
            }

            //update enemy healthbar
            if (hs.isEnemy)
            {

                //check if we need to show the regular enemy healthbar or large boss healthbar
                bool showBossHealthBar = (healthBarType == HEALTHBARTYPE.BossHealthBar && hs.showLargeHealthBar);
                bool showDefaultEnemyHealthBar = (healthBarType == HEALTHBARTYPE.EnemyHealthBar && !hs.showLargeHealthBar);

                if (showDefaultEnemyHealthBar || showBossHealthBar)
                {
                    ShowHealthBar(true);
                    SetUnitPortrait(hs);
                    healthBar.gameObject.SetActive(true);
                    healthBar.fillAmount = hs.healthPercentage;
                    nameField.text = hs.GetComponent<UnitSettings>().unitName; //get enemy name from unit settings
                    if (hs.GetComponent<UnitSettings>().showNameInAllCaps) nameField.text = nameField.text.ToUpper(); //show in capital letters
                    if (hs.currentHp == 0) ShowHealthBar(false); //hide enemy healthbar when hp = 0
                }
            }
        }

        //loads the HUD icon of the player from the player prefab (Healthsystem)
        void SetUnitPortrait(HealthSystem hs)
        {
            if (hs == null || portrait == null) return;

            //load portrait icon from unit settings
            UnitSettings settings = hs.GetComponent<UnitSettings>();
            portrait.sprite = (settings.unitPortrait != null) ? settings.unitPortrait : null;
            portrait.enabled = (portrait.sprite != null); //only show portrait is there is a sprite
        }

        //load player data on initialize
        void InitializePlayerBar(HealthSystem hs)
        {
            SetUnitPortrait(hs); //get portrait
            nameField.text = hs.GetComponent<UnitSettings>().unitName; //get name
            if (hs.GetComponent<UnitSettings>().showNameInAllCaps) nameField.text = nameField.text.ToUpper(); //show in capital letters
            initialized = true;
        }

        //show or hide this healthbar
        void ShowHealthBar(bool state)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = state ? 1f : 0f;
        }
    }
}
