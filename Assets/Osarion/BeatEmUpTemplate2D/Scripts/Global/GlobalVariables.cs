using UnityEngine;

using System;
using System.Collections.Generic;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }

    [HideInInspector]
    public int regularEnemyXPAdd = 60;

    [HideInInspector]
    public int bossEnemyXPAdd = 120;

    // Global XP and SP
    [HideInInspector]
    public int globalStageXP;

    [HideInInspector]
    public int globalXP;

    [HideInInspector]
    public int globalSP;

    // initial player stats for reference
    // maxHP = 200
    [HideInInspector]
    public int globalMaxHP;

    // // movement settings
    // moveSpeed = 4 
    // moveSpeedAir = 4
    [HideInInspector]
    public float globalMoveSpeed;

    [HideInInspector]
    public float globalMoveSpeedAir;

    // // jump settings
    // jumpHeight = 3.5
    // jumpSpeed = 3.5
    // jumpGravity = 3.8
    [HideInInspector]
    public float globalJumpHeight;

    [HideInInspector]
    public float globalJumpSpeed;

    [HideInInspector]
    public float globalJumpGravity;

    // attack data settings
    // - jumpPunch
    // - jumpKick
    // - grabPunch
    // - grabKick
    // - grabThrow
    // - groundPunch
    // - groundKick
    // damage = 1  // each of above attack data settings has this variable

    // // defense settings
    // canChangeDirWhileDefending = false
    // rearDefenseEnabled = false
    [HideInInspector]
    public bool globalCanChangeDirWhileDefending;

    [HideInInspector]
    public bool globalRearDefenseEnabled;

    [HideInInspector]
    public List<(bool, string, string, int, string, string, int)> perks;

    [HideInInspector]
    public string perkIdxSelected;

    // Ultimates (UltimateTrees)

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
