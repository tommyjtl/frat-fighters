using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BeatEmUpTemplate2D
{

    //class for unit or objects XP bar in the User Interface
    public class UIPerksAndUltimates : MonoBehaviour
    {

        public Text currentSP;
        public Text currentPerks;
        public Text perkIdxSelected;

        public Text selectedPerkName;
        public Text selectedPerkDescription;
        public Text selectedPerkCost;

        public Text playerAttributes;

        public Button unlockButton;

        public Text message;
        public GameObject perkPrompt;
        public GameObject selectPerkDetail;
        public GameObject alreadyUnlockedObj;
        public GameObject insufficentObj;
        public GameObject unlockObj;

        private bool initialized = false;

        private PAUSystem pauSystem;

        // [SerializeField] private GameObject pauPanel;
        public GameObject pauPanel;

        private bool isPaused = false;
        private int currentSPInt = 0;
        [SerializeField] private string menuToggleSFX = "UIButtonClick";

        //
        void OnEnable()
        {
            GlobalVariables.OnSPChanged += UpdateCurrentSPText;
            GlobalVariables.OnPerkIdxSelectedChanged += UpdateCurrentPerkIdxText;

            PAUSystem.onPAUChange += UpdatePAU; //subscribe to paus update events
            InitializePauBar(); //initialize xp bar with global values
        }

        void OnDisable()
        {
            GlobalVariables.OnSPChanged -= UpdateCurrentSPText;
            GlobalVariables.OnPerkIdxSelectedChanged -= UpdateCurrentPerkIdxText;

            PAUSystem.onPAUChange -= UpdatePAU; //unsubscribe to paus update events
        }

        void Start()
        {
            // Hide panel at the start
            // if (pauPanel != null)
            //     pauPanel.SetActive(false);
            initialized = false;

            // Find the player object and get the XPSystem component
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                pauSystem = player.GetComponent<PAUSystem>();
                if (pauSystem != null)
                    InitializePauBar(); //initialize with global values
            }

            if (unlockButton != null)
                unlockButton.interactable = false; // disable the button by default

            pauPanel.SetActive(false);
            perkPrompt.SetActive(true);
            selectPerkDetail.SetActive(false);
            alreadyUnlockedObj.SetActive(false);
            unlockObj.SetActive(false);
            insufficentObj.SetActive(true);
        }

        void UpdatePAU(PAUSystem paus)
        {
            if (!initialized) InitializePauBar(); //this is only done once at the start of the level

            if (GlobalVariables.Instance != null)
            {
                paus.perks = new System.Collections.Generic.List<(bool, string, string, int)> (GlobalVariables.Instance.perks);
                paus.perkIdxSelected = GlobalVariables.Instance.perkIdxSelected;

                currentPerks.text = paus.getCurrentPerksInString();
                perkIdxSelected.text = paus.getPerkIdxSelected();

                playerAttributes.text = GlobalVariables.Instance.gatherPlayerAttributes();
            }


        }

        //load player data on initialize
        void InitializePauBar()
        {
            if (GlobalVariables.Instance != null && pauSystem != null)
            {
                pauSystem.perks = new System.Collections.Generic.List<(bool, string, string, int)>(GlobalVariables.Instance.perks);
                currentPerks.text = pauSystem.getCurrentPerksInString();

                GlobalVariables.Instance.perkIdxSelected = "-1";
                pauSystem.perkIdxSelected = GlobalVariables.Instance.perkIdxSelected;
                perkIdxSelected.text = pauSystem.getPerkIdxSelected();

                playerAttributes.text = GlobalVariables.Instance.gatherPlayerAttributes();

                // for (int i = 0; i < pauSystem.perks.Count; i++)
                // {
                //     // Debug.Log("Perk " + pauSystem.perks[i].Item2 + " is locked");
                //     int index = i;

                //     GameObject perkToggle = GameObject.Find("PerkToggle" + (index + 1).ToString());
                //     GameObject background = perkToggle.transform.Find("Background").gameObject;
                //     Image backgroundImage = background.GetComponent<Image>();
                //     backgroundImage.sprite = Resources.Load<Sprite>("PerkItems/perk" + (index + 1) + "");
                // }
            }

            // Set the initialized flag to true
            initialized = true;
        }

        public void OnUnlockButtonClick()
        {
            // Handle the button click, maybe update a variable or perform an action
            if (perkIdxSelected != null)
            {
                // Debug.Log($"Button Clicked: {GlobalVariables.Instance.perkIdxSelected}");

                // perkIdxSelected is a string, need to convert it to an int
                int perkIdxSelectedInt = int.Parse(GlobalVariables.Instance.perkIdxSelected);

                // check if the perkIdxSelectedInt is within the range of the perks list
                if (perkIdxSelectedInt >= 0 && perkIdxSelectedInt < GlobalVariables.Instance.perks.Count)
                {
                    // check if the perk is unlocked already
                    bool unlocked = (bool)pauSystem.getPerkValue(perkIdxSelectedInt, "unlocked");
                    // if so, return
                    if (unlocked)
                    {
                        Debug.Log("Perk already unlocked");
                        return;
                    }

                    // check if the cost is greater than the current SP
                    int cost = (int)pauSystem.getPerkValue(perkIdxSelectedInt, "cost");
                    currentSPInt = GlobalVariables.Instance.globalSP;
                    if (cost > currentSPInt)
                    {
                        Debug.Log("Insufficient SP");
                        return;
                    }

                    // otherwise, activate the perk
                    pauSystem.ActivatePerk(perkIdxSelectedInt);

                    // deduct the cost from the current SP
                    currentSPInt -= cost;
                    if (currentSPInt == 1) 
                        currentSP.text = currentSPInt.ToString() + " POINT";
                    else 
                        currentSP.text = currentSPInt.ToString() + " POINTS";

                    // Update the global SP
                    if (GlobalVariables.Instance != null)
                    {
                        GlobalVariables.Instance.globalSP = currentSPInt;
                    }
                }
                else
                {
                    Debug.Log("Invalid Perk Index");
                }
            }
            else
            {
                // Debug.Log("Button Clicked");
            }
        }


        public void OnLockButtonClick()
        {
            // Handle the button click, maybe update a variable or perform an action
            if (perkIdxSelected != null)
            {
                Debug.Log($"Button Clicked: {GlobalVariables.Instance.perkIdxSelected}");

                // perkIdxSelected is a string, need to convert it to an int
                int perkIdxSelectedInt = int.Parse(GlobalVariables.Instance.perkIdxSelected);

                // check if the perkIdxSelectedInt is within the range of the perks list
                if (perkIdxSelectedInt >= 0 && perkIdxSelectedInt < GlobalVariables.Instance.perks.Count)
                {
                    // check if the perk is unlocked already
                    bool unlocked = (bool)pauSystem.getPerkValue(perkIdxSelectedInt, "unlocked");
                    // if so, return
                    if (!unlocked)
                    {
                        Debug.Log("Perk already locked");
                        return;
                    }

                    // otherwise, activate the perk
                    pauSystem.DeactivatePerk(perkIdxSelectedInt);

                    // deduct the cost from the current SP
                    int currentSPInt = GlobalVariables.Instance.globalSP;
                    int cost = (int)pauSystem.getPerkValue(perkIdxSelectedInt, "cost");
                    currentSPInt += cost;
                    if (currentSPInt == 1) 
                        currentSP.text = currentSPInt.ToString() + " POINT";
                    else 
                        currentSP.text = currentSPInt.ToString() + " POINTS";

                    // Update the global SP
                    if (GlobalVariables.Instance != null)
                    {
                        GlobalVariables.Instance.globalSP = currentSPInt;
                    }
                }
                else
                {
                    Debug.Log("Invalid Perk Index");
                }
            }
            else
            {
                Debug.Log("Button Clicked");
            }
        }

        public void OnMenuToggleButtonClick()
        {
            // Don't allow toggling if another menu has paused the game
            if (Time.timeScale == 0f && !isPaused)
                return;

            if (pauPanel != null)
            {
                bool isActive = pauPanel.activeSelf;
                pauPanel.SetActive(!isActive); // Toggle visibility
                //perkPrompt.SetActive(!isActive);
                //selectPerkDetail.SetActive(isActive);

                if (!isActive)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
        }

        private void PauseGame()
        {
            Time.timeScale = 0f; // Pause game physics & movement
            StartCoroutine(DelayAudioPause(0f));

            PlayMenuToggleSFX();
            isPaused = true;
            if (GlobalVariables.Instance != null)
                GlobalVariables.Instance.isPerkMenuActive = true; // Set perk menu active
        }

        private void ResumeGame()
        {
            if (GlobalVariables.Instance != null)
            {
                if (!GlobalVariables.Instance.isPauseMenuActive)
                {
                    Time.timeScale = 1f;
                    AudioListener.pause = false;
                }
            }

            PlayMenuToggleSFX();
            isPaused = false;
            if (GlobalVariables.Instance != null)
                GlobalVariables.Instance.isPerkMenuActive = false; // Set perk menu inactive
        }

        private void PlayMenuToggleSFX()
        {
            BeatEmUpTemplate2D.AudioController.PlaySFX(menuToggleSFX, Camera.main.transform.position);
        }

        private IEnumerator DelayAudioPause(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            AudioListener.pause = true;
        }

        void UpdateCurrentSPText(int sp)
        {
            if (sp == 1) 
                currentSP.text = sp.ToString() + " POINT";
            else 
                currentSP.text = sp.ToString() + " POINTS";
            GlobalVariables.Instance.globalSP = sp;
            PerkStatusCheck();
        }

        void UpdateCurrentPerkIdxText(string perkIdx)
        {
            if (perkIdxSelected != null)
            {
                selectPerkDetail.SetActive(true);
                perkPrompt.SetActive(false);
                perkIdxSelected.text = perkIdx;
                pauSystem.perkIdxSelected = perkIdx;


                // Update the perk name based on the selected index
                int perkIdxSelectedInt = int.Parse(perkIdx);
                Debug.Log($"[UIPerksAndUltimates.cs] Update Current Perk Index Text: {perkIdxSelectedInt}");

                if (IsPerkIdxValid(perkIdx))
                {
                    if (pauSystem == null)
                    {
                        Debug.LogError("PAUSystem is null");
                        return;
                    }

                    if (selectedPerkName != null)
                        selectedPerkName.text = (string)pauSystem.getPerkValue(perkIdxSelectedInt, "name");

                    if (selectedPerkDescription != null)
                        selectedPerkDescription.text = (string)pauSystem.getPerkValue(perkIdxSelectedInt, "description");

                    if (selectedPerkCost != null)
                    {
                        selectedPerkCost.text = "COST: " + ((int)pauSystem.getPerkValue(perkIdxSelectedInt, "cost")).ToString() + " SP";

                        PerkStatusCheck();
                    }
                }
                else
                {
                    //if (selectedPerkName != null)
                    //    selectedPerkName.text = "Select a perk";

                    //if (selectedPerkDescription != null)
                    //    selectedPerkDescription.text = "Select a perk to see its description";

                    //if (selectedPerkCost != null)
                    //    selectedPerkCost.text = "?";
                    selectPerkDetail.SetActive(false);
                    perkPrompt.SetActive(true);
                }

                // if (GlobalVariables.Instance != null)
                //     GlobalVariables.Instance.perkIdxSelected = pauSystem.getPerkIdxSelected();
            } else {
                selectPerkDetail.SetActive(false);
                perkPrompt.SetActive(true);
            }
        }

        private bool IsPerkIdxValid(string perkIdx)
        {
            // Check if the perkIdx is a valid integer and within the range of the perks list
            if (int.TryParse(perkIdx, out int index))
            {
                return index >= 0 && index < GlobalVariables.Instance.perks.Count;
            }
            return false;
        }

        public void SetButtonInteractable(bool isInteractable)
        {
            if (unlockButton != null)
            {
                unlockButton.interactable = isInteractable;
            }
        }

        public void PerkStatusCheck()
        {
            // check if the perk is unlocked already, and if current SP is greater than the cost
            if (perkIdxSelected != null && pauSystem != null)
            {
                // if perkIdxSelected is "-1", disable the button
                if (perkIdxSelected.text == "-1")
                {
                    unlockButton.interactable = false; // disable the button
                    selectedPerkCost.text = "Something broke";
                    return;
                }

                int perkIdxSelectedInt = int.Parse(GlobalVariables.Instance.perkIdxSelected);
                bool unlocked = (bool)pauSystem.getPerkValue(perkIdxSelectedInt, "unlocked");
                int cost = (int)pauSystem.getPerkValue(perkIdxSelectedInt, "cost");
                int currentSPInt = GlobalVariables.Instance.globalSP;

                if (unlocked)
                {
                    unlockButton.interactable = false; // disable the button
                    selectedPerkCost.text = "COST: " + cost.ToString() + " SP";

                    // Activate already unlocked display
                    alreadyUnlockedObj.SetActive(true);
                    unlockObj.SetActive(false);
                    insufficentObj.SetActive(false);

                }
                else if (cost > currentSPInt) {
                    unlockButton.interactable = false; // disable the button
                    selectedPerkCost.text = "COST: " + cost.ToString() + " SP";

                    // Activate insufficient points display
                    alreadyUnlockedObj.SetActive(false);
                    unlockObj.SetActive(false);
                    insufficentObj.SetActive(true);

                } else {
                    unlockButton.interactable = true; // enable the button
                    selectedPerkCost.text = "COST: " + cost.ToString() + " SP";

                    // Activate button
                    alreadyUnlockedObj.SetActive(false);
                    unlockObj.SetActive(true);
                    insufficentObj.SetActive(false);

                }

                
            }
        }

        void Update()
        {
            // Only allow keyboard toggle if game isn't paused by another menu
            if (InputManager.PerkMenuDown() && (Time.timeScale != 0f || isPaused))
            {
                OnMenuToggleButtonClick();
            }
        }
    }
}
