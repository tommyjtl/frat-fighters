using UnityEngine;

using System;
using System.Collections.Generic;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }

    /*
 __  _____                _   ___ ___ 
 \ \/ / _ \  __ _ _ _  __| | / __| _ \
  >  <|  _/ / _` | ' \/ _` | \__ \  _/
 /_/\_\_|   \__,_|_||_\__,_| |___/_|  
                                      
    */

    // Stage XP
    public int globalStageXP;

    // Global XP
    public int globalXP;

    // Global SP
    private int _globalSP;
    public int globalSP
    {
        get { return _globalSP; }
        set
        {
            if (_globalSP != value)
            {
                _globalSP = value;
                OnSPChanged?.Invoke(_globalSP);
            }
        }
    }
    public static event System.Action<int> OnSPChanged;

    /*
  ___       __           _ _     ___ _        _      
 |   \ ___ / _|__ _ _  _| | |_  / __| |_ __ _| |_ ___
 | |) / -_)  _/ _` | || | |  _| \__ \  _/ _` |  _(_-<
 |___/\___|_| \__,_|\_,_|_|\__| |___/\__\__,_|\__/__/
    
    - Initial player stats for reference
                                                     
    */

    // `Scripts/Units/HealthSystem.cs`
    private int _globalMaxHP;
    public int globalMaxHP
    {
        get { return _globalMaxHP; }
        set
        {
            if (_globalMaxHP != value)
            {
                _globalMaxHP = value;
                OnMaxHPChanged?.Invoke(_globalMaxHP);
            }
        }
    }
    public static event System.Action<int> OnMaxHPChanged;

    // `Scripts/Units/UnitSettings.cs`
    private float _globalMoveSpeed;
    public float globalMoveSpeed
    {
        get { return _globalMoveSpeed; }
        set
        {
            if (_globalMoveSpeed != value)
            {
                _globalMoveSpeed = value;
                OnMoveSpeedChanged?.Invoke(_globalMoveSpeed);
            }
        }
    }
    public static event System.Action<float> OnMoveSpeedChanged;
    private float _globalMoveSpeedAir; // 4
    public float globalMoveSpeedAir
    {
        get { return _globalMoveSpeedAir; }
        set
        {
            if (_globalMoveSpeedAir != value)
            {
                _globalMoveSpeedAir = value;
                OnMoveSpeedAirChanged?.Invoke(_globalMoveSpeedAir);
            }
        }
    }
    public static event System.Action<float> OnMoveSpeedAirChanged;

    // `Scripts/Units/UnitSettings.cs`
    private float _globalJumpHeight; // 3.5
    public float globalJumpHeight
    {
        get { return _globalJumpHeight; }
        set
        {
            if (_globalJumpHeight != value)
            {
                _globalJumpHeight = value;
                OnJumpHeightChanged?.Invoke(_globalJumpHeight);
            }
        }
    }
    public static event System.Action<float> OnJumpHeightChanged;
    private float _globalJumpSpeed; // 3.5
    public float globalJumpSpeed
    {
        get { return _globalJumpSpeed; }
        set
        {
            if (_globalJumpSpeed != value)
            {
                _globalJumpSpeed = value;
                OnJumpSpeedChanged?.Invoke(_globalJumpSpeed);
            }
        }
    }
    public static event System.Action<float> OnJumpSpeedChanged;
    private float _globalJumpGravity; // 3.8
    public float globalJumpGravity
    {
        get { return _globalJumpGravity; }
        set
        {
            if (_globalJumpGravity != value)
            {
                _globalJumpGravity = value;
                OnJumpGravityChanged?.Invoke(_globalJumpGravity);
            }
        }
    }
    public static event System.Action<float> OnJumpGravityChanged;

    // `Scripts/Units/UnitSettings.cs`
    // `Scripts/Units/AttackData.cs`
    private int _globalAttackDamageAddUp;
    public int globalAttackDamageAddUp
    {
        get { return _globalAttackDamageAddUp; }
        set
        {
            if (_globalAttackDamageAddUp != value)
            {
                _globalAttackDamageAddUp = value;
                OnAttackDamageAddUpChanged?.Invoke(_globalAttackDamageAddUp);
            }
        }
    }
    public static event System.Action<int> OnAttackDamageAddUpChanged;
    public string globalAttackDamageOverview;

    // `Scripts/Units/UnitSettings.cs`
    private bool _globalRearDefenseEnabled;
    public bool globalRearDefenseEnabled
    {
        get { return _globalRearDefenseEnabled; }
        set
        {
            if (_globalRearDefenseEnabled != value)
            {
                _globalRearDefenseEnabled = value;
                OnRearDefenseEnabledChanged?.Invoke(_globalRearDefenseEnabled);
            }
        }
    }
    public static event System.Action<bool> OnRearDefenseEnabledChanged;

    /*
    @TODO: properties to be added in the future
    - Can be knocked down (Knockdown Settings)
    - ...
    */

    /*
  ___         _       
 | _ \___ _ _| |__ ___
 |  _/ -_) '_| / /(_-<
 |_| \___|_| |_\_\/__/
                      
    */
    public List<(
                bool,       // whether the perk is unlocked
                string,     // perk name
                string,     // perk description
                int,        // perk cost
                string,     // perk type
                string,     // perk sub-type (we don't need this anymore, but I'll keep it for now)
                int         // perk value to be added
            )> perks = new List<(bool, string, string, int, string, string, int)> {
            // Health Perks
            (false, "Fortitude", "Increase max HP by 200", 3, "Health", "max", 200),

            // Damage Perks
            (false, "Universal Power I", "Increase all attack damage", 1, "Attack", "all", 2),
            (false, "Universal Power II", "Increase (even more) all attack damage", 1, "Attack", "all", 4),

            // Speed Perks
            (false, "Swift Foot", "Increase ground move speed", 1, "Movement", "ground speed", 2),
            (false, "Air Agility", "Increase air move speed", 1, "Movement", "air speed", 2),

            // Jump Perks
            (false, "High Jumper", "Increase jump height", 1, "Jump", "height", 2),
            (false, "Gravity Defier", "Decrease jump gravity", 1, "Jump", "gravity", -2),

            // Defense Perks
            (false, "Safe Guard", "Enable rear defense", 1, "Defense", "rear", 0),
        };
    private string _perkIdxSelected = "-1";
    public string perkIdxSelected
    {
        get { return _perkIdxSelected; }
        set
        {
            if (_perkIdxSelected != value)
            {
                _perkIdxSelected = value;
                OnPerkIdxSelectedChanged?.Invoke(_perkIdxSelected);
            }
        }
    }
    public static event System.Action<string> OnPerkIdxSelectedChanged;

    /*
  _   _ _ _   _            _      _____            
 | | | | | |_(_)_ __  __ _| |_ __|_   _| _ ___ ___ 
 | |_| | |  _| | '  \/ _` |  _/ -_)| || '_/ -_) -_)
  \___/|_|\__|_|_|_|_\__,_|\__\___||_||_| \___\___|
                                                   
    */
    // to be wrote

    public string gatherPlayerAttributes()
    {
        string playerAttributes = "";
        playerAttributes += "[Max HP] " + globalMaxHP + "\n";
        playerAttributes += "[Move Speed] " + globalMoveSpeed + "\n";
        playerAttributes += "[Move Speed Air] " + globalMoveSpeedAir + "\n";
        playerAttributes += "[Jump Height] " + globalJumpHeight + "\n";
        playerAttributes += "[Jump Speed] " + globalJumpSpeed + "\n";
        playerAttributes += "[Jump Gravity] " + globalJumpGravity + "\n";
        playerAttributes += "[Attack Damage Add Up] " + globalAttackDamageAddUp + "\n";
        playerAttributes += "[Rear Defense] " + globalRearDefenseEnabled + "\n";
        return playerAttributes;
    }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
    }
}
