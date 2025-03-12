using UnityEngine;

using System;
using System.Collections.Generic;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }

    // Global XP and SP

    public int globalStageXP;
    public int globalXP;
    public int globalSP;

    // Perks and ultimates
    // - We have a fixed amount of perk items
    // - We have a fixed amount of ultimate trees, each tree contains a fixed amount of ultimate nodes
    // We need to keep track of player's unlock progress for each of these items

    // Perks, list type: a list containing the unlock status and the name of the perk
    public List<Tuple<bool, string>> perks;
    // An example perk list:
    // perks = new List<Tuple<bool, string>>()
    // {
    //     new Tuple<bool, string>(false, "PerkName"),
    //     new Tuple<bool, string>(false, "PerkName"),
    // }
    // false means the perk is locked (hasn't been unlocked), true means the perk is unlocked

    // Ultimates, dict type: key is the name of each ultimate tree, value is a list containing the unlock status and the name of the ultimate node
    public Dictionary<string, List<Tuple<bool, string>>> ultimates;
    // An example ultimate dict:    
    // ultimates = new Dictionary<string, List<Tuple<bool, string>>>()
    // {
    //     {
    //         "UltimateTreeName",
    //         new List<Tuple<bool, string>>()
    //         {
    //             new Tuple<bool, string>(false, "UltimateNodeName"),
    //             new Tuple<bool, string>(false, "UltimateNodeName"),
    //         }
    //     }
    // }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
