using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BeatEmUpTemplate2D
{
    //xpsystem class for player, enemy and objects
    public class PAUSystem : MonoBehaviour
    {
        // XP
        [HideInInspector]
        public List<(bool, string, string, int, string, string, int)> perks;

        [HideInInspector]
        public string perkIdxSelected;

        // Event

        public delegate void OnPAUChange(PAUSystem paus); // Renamed the delegate
        public static event OnPAUChange onPAUChange;

        void OnEnable() { }

        void OnDisable() { }

        void Start()
        {
            //initialize
            if (onPAUChange != null) onPAUChange(this);

            // Debug.Log("PAU System initialized");
        }

        // Activate perk
        public void ActivatePerk(int perkId)
        {
            unlockPerk(perkId);

            SendPAUPEvent();
        }

        public void DeactivatePerk(int perkId)
        {
            lockPerk(perkId);

            SendPAUPEvent();
        }

        //PAU update event
        private void SendPAUPEvent()
        {
            if (onPAUChange != null) onPAUChange(this);
        }


        public (bool, string, string, int, string, string, int) getPerk(int index)
        {
            return perks[index];
        }

        // and a method to retrieve a specifc value of a perk, 
        // - input is the index of the perk and the value to be retrieved
        public object getPerkValue(int index, string value)
        {
            switch (value)
            {
                case "unlocked":
                    return perks[index].Item1;
                case "name":
                    return perks[index].Item2;
                case "description":
                    return perks[index].Item3;
                case "cost":
                    return perks[index].Item4;
                case "type":
                    return perks[index].Item5;
                case "stat":
                    return perks[index].Item6;
                case "value":
                    return perks[index].Item7;
                default:
                    return null;
            }
        }

        // setter for unlocking a perk
        public void unlockPerk(int index)
        {
            // if the perk is not unlocked
            if (!perks[index].Item1)
            {
                // unlock the perk
                string perk_name = (string)getPerkValue(index, "name");
                int value_added = (int)getPerkValue(index, "value");

                switch (perk_name)
                {
                    case "Fortitude":
                        GlobalVariables.Instance.globalMaxHP += 200;
                        // (false, "Ironclad", "Increase max HP by 200", 3, "Health", "max", 200),
                        break;
                    case "Universal Power I":
                        // GlobalVariables.Instance.globalMoveSpeed += value_added;
                        GlobalVariables.Instance.globalAttackDamageAddUp = value_added;
                        // (false, "Universal Power I",
                        break;
                    case "Universal Power II":
                        GlobalVariables.Instance.globalAttackDamageAddUp = value_added;
                        // (false, "Universal Power II",
                        break;
                    case "Swift Foot":
                        GlobalVariables.Instance.globalMoveSpeed += 2;
                        // (false, "Swift Foot", "Increase ground move speed", 1, "Movement", "ground speed", 2),
                        break;
                    case "Air Agility":
                        GlobalVariables.Instance.globalMoveSpeedAir += 4;
                        // (false, "Air Agility", "Increase air move speed", 1, "Movement", "air speed", 2),
                        break;
                    case "High Jumper":
                        GlobalVariables.Instance.globalJumpHeight += 2;
                        // (false, "High Jumper", "Increase jump height", 1, "Jump", "height", 2),
                        break;
                    case "Gravity Defier":
                        GlobalVariables.Instance.globalJumpGravity += -2;
                        // (false, "Gravity Defier", "Decrease jump gravity", 1, "Jump", "gravity", -2),
                        break;
                    case "Safe Guard":
                        GlobalVariables.Instance.globalRearDefenseEnabled = true;
                        // (false, "Safe Guard", "Enable rear defense", 1, "Defense", "rear", 0),
                        break;
                    default:
                        break;
                }

                perks[index] = (true, perks[index].Item2, perks[index].Item3, perks[index].Item4, perks[index].Item5, perks[index].Item6, perks[index].Item7);

            }
            // Debug.Log("Perk unlocked: " + index);
        }

        public void lockPerk(int index)
        {
            // if the perk is unlocked
            if (perks[index].Item1)
            {
                // lock the perk
                perks[index] = (false, perks[index].Item2, perks[index].Item3, perks[index].Item4, perks[index].Item5, perks[index].Item6, perks[index].Item7);
                // Debug.Log("Perk locked: " + index);
            }
        }

        public string getCurrentPerksInString()
        {
            // if perks is null
            if (perks == null)
                return "No perks available";

            string currentPerks = "";
            int idx = 0;
            foreach (var perk in perks)
            {
                currentPerks += $"{idx}\t{perk.Item1}, {perk.Item2}, {perk.Item4},\t\"{perk.Item5}\",\t\"{perk.Item6}\",\t{perk.Item7})\n";
                idx++;
            }

            return currentPerks;
        }

        public string getPerkIdxSelected()
        {
            // if perkIdxSelected is null
            if (perkIdxSelected == null)
                return "-1";
            else
                return perkIdxSelected;
        }


    }
}
