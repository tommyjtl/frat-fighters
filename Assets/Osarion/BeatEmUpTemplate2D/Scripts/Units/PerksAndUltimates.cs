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
        public List<(bool, string, string, int)> perks;

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


        public (bool, string, string, int) getPerk(int index)
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

                // See more in `Scripts/Global/GlobalVariables.cs`
                // - GlobalVariables.Instance.globalMaxHP;
                // - GlobalVariables.Instance.globalAttackDamageAddUp;
                // - GlobalVariables.Instance.globalMoveSpeed;
                // - GlobalVariables.Instance.globalMoveSpeedAir;
                // - GlobalVariables.Instance.globalJumpHeight;
                // - GlobalVariables.Instance.globalJumpGravity;
                // - GlobalVariables.Instance.globalRearDefenseEnabled;
                switch (perk_name)
                {
                    // Goto `Scripts/Global/GlobalVariables.cs`
                    case "Lightweight":
                        GlobalVariables.Instance.globalJumpHeight += 2; // higher jump
                        GlobalVariables.Instance.globalJumpGravity += -2; // lower gravity
                        // @TODO: "better benefit of drugs" left out for now
                        break;
                    case "Chicken and Rice":
                        GlobalVariables.Instance.globalAttackDamageAddUp = 5;
                        GameObject player = GameObject.FindWithTag("Player");
                        if (player != null)
                        {
                            // Double the player's size
                            player.transform.localScale *= 2;
                        }
                        break;
                    case "Hazing Specialist":
                        GlobalVariables.Instance.globalStealOnEnemyKill = true;
                        break;
                    case "Hoplite Training":
                        // @TODO: implement two hit combos
                        // public List<Combo> comboData = new List<Combo>();            in `UnitSettings.cs`
                        // private Combo FindComBoMatch(List<ATTACKTYPE> attackList);   in `PlayerAttack.cs`
                        break;
                    default:
                        break;
                }

                perks[index] = (true, perks[index].Item2, perks[index].Item3, perks[index].Item4);

            }
            // Debug.Log("Perk unlocked: " + index);
        }

        public void lockPerk(int index)
        {
            // if the perk is unlocked
            if (perks[index].Item1)
            {
                // lock the perk
                perks[index] = (false, perks[index].Item2, perks[index].Item3, perks[index].Item4);
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
                currentPerks += $"{idx}\t{perk.Item1}, {perk.Item2}, {perk.Item4}\n";
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
