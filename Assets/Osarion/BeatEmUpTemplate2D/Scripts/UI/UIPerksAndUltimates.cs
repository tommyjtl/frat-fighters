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
        public Text message;


        // 
        private bool initialized = false;

        private PAUSystem pauSystem;

        //
        void OnEnable()
        {
            PAUSystem.onPAUChange += UpdatePAU; //subscribe to paus update events
            InitializePauBar(); //initialize xp bar with global values
        }

        void OnDisable()
        {
            PAUSystem.onPAUChange -= UpdatePAU; //unsubscribe to paus update events
        }

        void Start()
        {
            // Find the player object and get the XPSystem component
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                pauSystem = player.GetComponent<PAUSystem>();
                if (pauSystem != null)
                    InitializePauBar(); //initialize with global values
            }
        }

        void UpdatePAU(PAUSystem paus)
        {
            if (!initialized) InitializePauBar(); //this is only done once at the start of the level

            currentPerks.text = paus.getCurrentPerksInString();//
            perkIdxSelected.text = paus.getPerkIdxSelected();


            if (GlobalVariables.Instance != null)
            {
                GlobalVariables.Instance.perks = paus.perks;
                GlobalVariables.Instance.perkIdxSelected = paus.getPerkIdxSelected();
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
            }

            currentSP.text = 10.ToString(); // hard coded value for now 
            // @TODO: change this to retrieve from GlobalVariables

            // Set the initialized flag to true
            initialized = true;
        }

        public void OnInputFieldChanged(string inputValue)
        {
            // Handle the inputValue, maybe update a variable or perform an action
            Debug.Log("Input Field Changed: " + inputValue);

            if (perkIdxSelected != null)
            {
                perkIdxSelected.text = inputValue;
                pauSystem.perkIdxSelected = inputValue;

                if (GlobalVariables.Instance != null)
                    GlobalVariables.Instance.perkIdxSelected = pauSystem.getPerkIdxSelected();
            }

        }

        public void OnUnlockButtonClick()
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

    }
}
