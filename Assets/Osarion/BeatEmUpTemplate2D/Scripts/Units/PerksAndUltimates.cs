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
        public List<(bool, string, string, int, string, string, int)> perks = new List<(bool, string, string, int, string, string, int)> {
            // currently all perks only require 1 SP to unlock, for development purposes
            // total of 13 perks at the moment

            // Health Perks
            (false, "Fortitude", "Increase max HP by 50", 2, "Health", "max", 50),
            (false, "Ironclad", "Increase max HP by 200", 3, "Health", "max", 200),

            // Damage Perks
            (false, "Universal Power", "Increase all attack damage by 2", 1, "Attack", "all", 2),
            (false, "Ground Pounder", "Increase ground attack damage by 2", 1, "Attack", "ground", 2), // ground attack damage: groundPunch, groundKick
            (false, "Sky Striker", "Increase air attack damage by 2", 1, "Attack", "jump", 2), // air attack damage: jumpPunch, jumpKick
            (false, "Grapple Master", "Increase grab attack damage by 2", 1, "Attack", "grab", 2), // grab attack damage: grabPunch, grabKick, grabThrow

            // Speed Perks
            (false, "Swift Foot", "Increase ground move speed by 1", 1, "Movement", "ground speed", 1),
            (false, "Air Agility", "Increase air move speed by 1", 1, "Movement", "air speed", 1),

            // Jump Perks
            (false, "High Jumper", "Increase jump height by 1", 9, "Jump", "height", 1),
            (false, "Jump Speed", "Increase jump speed by 1", 1, "Jump", "speed", 1),
            (false, "Gravity Defier", "Decrease jump gravity by 0.5", 1, "Jump", "gravity", -2),

            // Defense Perks
            (false, "Quick Guard", "Enable changing direction while defending", 1, "Defense", "change dir", 1),
            (false, "Rear Guard", "Enable rear defenses", 1, "Defense", "rear", 1)
        };

        [HideInInspector]
        public string perkIdxSelected = "0";

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

        // setter for unlocking a perk, return the cost of the perk
        public void unlockPerk(int index)
        {
            // if the perk is not unlocked
            if (!perks[index].Item1)
            {
                // unlock the perk
                perks[index] = (true, perks[index].Item2, perks[index].Item3, perks[index].Item4, perks[index].Item5, perks[index].Item6, perks[index].Item7);
                // Debug.Log("Perk unlocked: " + index);
            }
        }

        public void lockPerk(int index) // return the cost of the perk
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
            foreach (var perk in perks)
                currentPerks += $"({perk.Item1}, Cost: {perk.Item4},\t\"{perk.Item5}\",\t\"{perk.Item6}\",\t{perk.Item7})\n";

            return currentPerks;
        }

        public string getPerkIdxSelected()
        {
            // if perkIdxSelected is null
            if (perkIdxSelected == null)
                return "-";
            else
            {
                // if the length exceeds 1, return the first character
                if (perkIdxSelected.Length > 1)
                    return perkIdxSelected.Substring(0, 1);
                else
                    return perkIdxSelected;
            }
        }


    }
}
