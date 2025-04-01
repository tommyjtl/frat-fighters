using UnityEngine;
using UnityEngine.UI;

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

        private bool initialized = false;

        private PAUSystem pauSystem;

        // [SerializeField] private GameObject pauPanel;
        public GameObject pauPanel;


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
        }

        void UpdatePAU(PAUSystem paus)
        {
            if (!initialized) InitializePauBar(); //this is only done once at the start of the level

            if (GlobalVariables.Instance != null)
            {
                paus.perks = GlobalVariables.Instance.perks;
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
                pauSystem.perks = GlobalVariables.Instance.perks;
                currentPerks.text = pauSystem.getCurrentPerksInString();

                pauSystem.perkIdxSelected = GlobalVariables.Instance.perkIdxSelected;
                perkIdxSelected.text = pauSystem.getPerkIdxSelected();

                playerAttributes.text = GlobalVariables.Instance.gatherPlayerAttributes();
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
                    int currentSPInt = int.Parse(currentSP.text);
                    if (cost > currentSPInt)
                    {
                        Debug.Log("Insufficient SP");
                        return;
                    }

                    // otherwise, activate the perk
                    pauSystem.ActivatePerk(perkIdxSelectedInt);

                    // deduct the cost from the current SP
                    currentSPInt -= cost;
                    currentSP.text = currentSPInt.ToString();

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
                    int currentSPInt = int.Parse(currentSP.text);
                    int cost = (int)pauSystem.getPerkValue(perkIdxSelectedInt, "cost");
                    currentSPInt += cost;
                    currentSP.text = currentSPInt.ToString();

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
            // // Handle the button click, maybe update a variable or perform an action
            // if (message != null)
            // {
            //     Debug.Log("Menu Toggle Button Clicked");
            //     message.text = "Menu Toggle Button Clicked";
            // }
            // Debug.Log("Menu Toggle Button Clicked");

            if (pauPanel != null)
            {
                bool isActive = pauPanel.activeSelf;
                pauPanel.SetActive(!isActive); // Toggle visibility
                // Debug.Log("Menu Toggle Button Clicked. PAUPanel active: " + !isActive);
            }
            else
            {
                // Debug.LogWarning("PAUPanel reference is not assigned in the Inspector.");
            }
        }

        void UpdateCurrentSPText(int sp)
        {
            currentSP.text = sp.ToString();
            PerkStatusCheck();
        }

        void UpdateCurrentPerkIdxText(string perkIdx)
        {
            if (perkIdxSelected != null)
            {
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
                        selectedPerkCost.text = ((int)pauSystem.getPerkValue(perkIdxSelectedInt, "cost")).ToString() + " SP";

                        PerkStatusCheck();
                    }
                }
                else
                {
                    if (selectedPerkName != null)
                        selectedPerkName.text = "-";
                    if (selectedPerkDescription != null)
                        selectedPerkDescription.text = "-";
                    if (selectedPerkCost != null)
                        selectedPerkCost.text = "";
                }

                // if (GlobalVariables.Instance != null)
                //     GlobalVariables.Instance.perkIdxSelected = pauSystem.getPerkIdxSelected();
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
                    selectedPerkCost.text = "?";
                    return;
                }

                int perkIdxSelectedInt = int.Parse(GlobalVariables.Instance.perkIdxSelected);
                bool unlocked = (bool)pauSystem.getPerkValue(perkIdxSelectedInt, "unlocked");
                int cost = (int)pauSystem.getPerkValue(perkIdxSelectedInt, "cost");
                int currentSPInt = int.Parse(currentSP.text);
                if (unlocked)
                {
                    unlockButton.interactable = false; // disable the button
                    selectedPerkCost.text = "Already Unlocked";

                }
                else
                {
                    unlockButton.interactable = true; // enable the button
                    if (cost > currentSPInt)
                        unlockButton.interactable = false; // disable the button
                }
            }
        }

    }
}
