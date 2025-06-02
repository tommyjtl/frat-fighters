using UnityEngine;

using System;
using System.Collections.Generic;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }

    public bool isPerkMenuActive = false;
    public bool isPauseMenuActive = false;
    public bool isPauEscButtonPressed = false;

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

    private bool _globalRecalculateHP;
    public bool globalRecalculateHP
    {
        get { return _globalRecalculateHP; }
        set
        {
            if (_globalRecalculateHP != value)
            {
                _globalRecalculateHP = value;
                OnRecalculateHPChanged?.Invoke(_globalRecalculateHP);
            }
        }
    }
    public static event System.Action<bool> OnRecalculateHPChanged;

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
    private int _globalAttackDamageMultiplier;
    public int globalAttackDamageMultiplier
    {
        get { return _globalAttackDamageMultiplier; }
        set
        {
            if (_globalAttackDamageMultiplier != value)
            {
                _globalAttackDamageMultiplier = value;
                OnAttackDamageMultiplierChanged?.Invoke(_globalAttackDamageMultiplier);
            }
        }
    }
    public static event System.Action<int> OnAttackDamageMultiplierChanged;
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

    public bool globalStealOnEnemyKill = false;

    // private bool _globalStealOnEnemyKill;
    // public bool globalStealOnEnemyKill
    // {
    //     get { return _globalStealOnEnemyKill; }
    //     set
    //     {
    //         if (_globalStealOnEnemyKill != value)
    //         {
    //             _globalStealOnEnemyKill = value;
    //             OnStealOnEnemyKillChanged?.Invoke(_globalStealOnEnemyKill);
    //         }
    //     }
    // }
    // public static event System.Action<bool> OnStealOnEnemyKillChanged;

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
                int         // perk cost
            )> perks = new List<(bool, string, string, int)> {
                // Goto `Scripts/Units/PerksAndUltimates.cs` for detailed attributes manipulation
                (
                    // [Fully implemented]
                    false,
                    "Chicken and Rice",
                    "Double damage and player size.",
                    6 // skill points required
                ),
                (
                    false,
                    "Spartan Resolve",
                    "Max health increased by 100.",
                    4 // skill points required
                ),
                (
                    false,
                    "Hazing Specialist",
                    "Lifesteal on kill.",
                    3 // skill points required
                ),
                (
                    false,
                    "Light- weight",
                    "Increases jump height and duration.",
                    1 // skill points required
                )
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
    // Abandonded feature ☠️

    public string gatherPlayerAttributes()
    {
        string playerAttributes = "";
        playerAttributes += "[Max HP] " + globalMaxHP + "\n";
        playerAttributes += "[Move Speed] " + globalMoveSpeed + "\n";
        playerAttributes += "[Move Speed Air] " + globalMoveSpeedAir + "\n";
        playerAttributes += "[Jump Height] " + globalJumpHeight + "\n";
        playerAttributes += "[Jump Speed] " + globalJumpSpeed + "\n";
        playerAttributes += "[Jump Gravity] " + globalJumpGravity + "\n";
        playerAttributes += "[Attack Damage Add Up] " + globalAttackDamageMultiplier + "\n";
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
