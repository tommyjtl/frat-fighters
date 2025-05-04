using UnityEngine;
using UnityEngine.UI;
using System;

namespace BeatEmUpTemplate2D
{

    //class for unit or objects healthbars in the User Interface
    public class UIHUDHealthBar : MonoBehaviour
    {

        private enum HEALTHBARTYPE { PlayerHealthBar, EnemyHealthBar, BossHealthBar }
        [SerializeField] private HEALTHBARTYPE healthBarType = HEALTHBARTYPE.PlayerHealthBar;
        public Text nameField;
        public Text currentHp;
        public Text maxHp;
        public Image portrait;
        public Image healthBar;
        private bool initialized;
        public float PlayerBarDivisions = 26;
        public float EnemyBarDivisions = 26;
        public float BossBarDivisions = 184;

        private HealthSystem currentHealthSystem;

        void OnEnable()
        {
            GlobalVariables.OnMaxHPChanged += UpdateCurrentMaxHPText;
            GlobalVariables.OnRecalculateHPChanged += UpdateShouldRecalculateHPText;

            HealthSystem.onHealthChange += UpdateHealth; //subscribe to health update events
            if (healthBarType == HEALTHBARTYPE.EnemyHealthBar) ShowHealthBar(false); //hide enemy healthbar by default

            //check portrait reference
            if (portrait == null) Debug.Log("no portrait image was linked in the UIHealthbar");
        }

        void OnDisable()
        {
            GlobalVariables.OnMaxHPChanged -= UpdateCurrentMaxHPText;
            GlobalVariables.OnRecalculateHPChanged -= UpdateShouldRecalculateHPText;

            HealthSystem.onHealthChange -= UpdateHealth; //unsubscribe to health update events
        }

        public void UpdateHealth(HealthSystem hs)
        {
            if (healthBar == null) return;

            //update player healthbar
            if (healthBarType == HEALTHBARTYPE.PlayerHealthBar && hs.isPlayer)
            {
                if (!initialized) InitializePlayerBar(hs); //this is only done once at the start of the level

                currentHealthSystem = hs; // Store reference here

                double step = 1.0/PlayerBarDivisions;
                float fill = (float)(Math.Ceiling(hs.healthPercentage / step) * step);
                healthBar.fillAmount = fill;

                currentHp.text = hs.currentHp.ToString();
                maxHp.text = hs.maxHp.ToString();

                if (GlobalVariables.Instance != null)
                {
                    if (GlobalVariables.Instance.globalMaxHP == 0)
                    {
                        GlobalVariables.Instance.globalMaxHP = hs.maxHp;
                        // GlobalVariables.Instance.globalCurrentHP = hs.currentHp;
                    }
                    else
                    {
                        hs.maxHp = GlobalVariables.Instance.globalMaxHP;
                        // hs.currentHp = GlobalVariables.Instance.globalCurrentHP;
                    }
                }
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
                    float fill;
                    if (healthBarType == HEALTHBARTYPE.BossHealthBar) {
                        double step = 1.0/EnemyBarDivisions;
                        fill = (float)(Math.Ceiling(hs.healthPercentage / step) * step);
                    } else if (healthBarType == HEALTHBARTYPE.EnemyHealthBar) {
                        double step = 1.0/BossBarDivisions;
                        fill = (float)(Math.Ceiling(hs.healthPercentage / step) * step);
                    } else {
                        fill = hs.healthPercentage;
                    }
                    healthBar.fillAmount = fill;
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
            maxHp.text = hs.maxHp.ToString();
            currentHp.text = hs.currentHp.ToString();


            if (GlobalVariables.Instance != null)
            {
                // GlobalVariables.Instance.globalMaxHP = hs.maxHp;
                if (GlobalVariables.Instance.globalMaxHP == 0)
                {
                    GlobalVariables.Instance.globalMaxHP = hs.maxHp;
                    // GlobalVariables.Instance.globalCurrentHP = hs.currentHp;
                }
                else
                {
                    hs.maxHp = GlobalVariables.Instance.globalMaxHP;
                    // hs.currentHp = GlobalVariables.Instance.globalCurrentHP;
                }

                // Debug.Log($"[UIHUDHealthBar] GlobalVariables.Instance.globalMaxHP: {GlobalVariables.Instance.globalMaxHP}");
            }

            initialized = true;
        }

        //show or hide this healthbar
        void ShowHealthBar(bool state)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = state ? 1f : 0f;
        }

        void UpdateCurrentMaxHPText(int hp)
        {
            // update local max hp value
            if (currentHealthSystem != null)
            {
                currentHealthSystem.maxHp = hp;
                // currentHealthSystem.currentHp = hp;

                // Recalculate fill amount using updated values
                float percent = (float)currentHealthSystem.currentHp / (float)currentHealthSystem.maxHp;
                healthBar.fillAmount = percent;
                currentHp.text = currentHealthSystem.currentHp.ToString();
            }

            if (maxHp != null) maxHp.text = hp.ToString();
        }

        void UpdateShouldRecalculateHPText(bool state)
        {
            if (currentHealthSystem != null)
            {
                // Recalculate fill amount using updated values
                float percent = (float)currentHealthSystem.currentHp / (float)currentHealthSystem.maxHp;
                healthBar.fillAmount = percent;
                currentHp.text = currentHealthSystem.currentHp.ToString();

                if (GlobalVariables.Instance != null)
                    GlobalVariables.Instance.globalRecalculateHP = false;
            }
        }
    }
}
