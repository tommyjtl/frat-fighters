using UnityEngine;
using System.Collections.Generic;

namespace BeatEmUpTemplate2D
{

    public enum UNITTYPE { PLAYER = 0, ENEMY = 10, NPC = 20, BOSS = 30 }

    [System.Serializable]
    public class UnitSettings : MonoBehaviour
    {

        public UNITTYPE unitType = UNITTYPE.PLAYER;
        public bool debugHitbox = false; // Make hitbox visible

        //LINKED OBJECTS
        public GameObject shadowPrefab; //shadow prefab
        public GameObject shadow; //shadow that follows this unit
        public GameObject weaponBone; //position for weapon attachments
        public GameObject hitEffect; //effect that gets played when we've hit something
        public SpriteRenderer hitBox; //sprite bounding box used for hit collision
        public SpriteRenderer spriteRenderer; //this unit's sprite renderer

        //MOVEMENT SETTINGS
        public DIRECTION startDirection = DIRECTION.RIGHT; //start direction
        public float moveSpeed = 4; //move speed while on the ground
        public float moveSpeedAir = 4; //moving speed while in the air
        public bool useAcceleration = false; //use acceleration over time if true, or move instantly when false

        //ACCELERATION / DECELERATION
        public float moveAcceleration = 25f; //how fast we gain speed
        public float moveDeceleration = 10f; //how fast we lose speed

        //JUMP SETTINGS
        public float jumpHeight = 4; //how high this unit can jump
        public float jumpSpeed = 3.5f; //how fast the jump is simulated
        public float jumpGravity = 5f; //the downward force

        //ATTACK DATA
        [Space(10)]
        [Help("* Only PUNCH and KICK Attack Types can be used in combos")]
        public List<Combo> comboData = new List<Combo>();
        public float comboResetTime = .55f; //when this time expires a combo will be reset
        public bool continueComboOnHit; //only continue the combo when the previous attack hit a target
        [Space(10)]
        public AttackData jumpPunch;
        public AttackData jumpKick;
        [Space(10)]
        public AttackData grabPunch;
        public AttackData grabKick;
        public AttackData grabThrow;
        [Space(10)]
        public AttackData groundPunch;
        public AttackData groundKick;

        //ENEMY ATTACK DATA
        public List<AttackData> enemyAttackList = new List<AttackData>(); //list of enemy attacks

        [HideInInspector]
        public int attackDamageAddUp;

        //KNOCKDOWN SETTINGS
        public bool canBeKnockedDown = true; //if this unit can be knocked down
        public float knockDownHeight = 3; //how high the unit flies in the air during a knockdown
        public float knockDownDistance = 3; //horizontal movement distance
        public float knockDownSpeed = 3; //how fast the knockdown simulates
        public float knockDownFloorTime = 1; //how long this unit will stay on the floor before standing up
        public bool hitOtherEnemiesWhenFalling = false; //option to hit other enemies when falling down

        //THROW SETTINGS
        public float throwHeight = 3; //how high this unit flies when thrown
        public float throwDistance = 5; //how far this unit is thrown
        public bool hitOtherEnemiesWhenThrown = true; //option to hit other enemies when being thrown by the player

        //DEFENCE SETTINGS
        public float defendChance; //a percentage change that an enemy defends an incoming attack
        public float defendDuration; //how long an enemy stays in defend state
        public bool canChangeDirWhileDefending; //enable/disable changing direction while defending
        public bool rearDefenseEnabled; //can defend attacks coming from behind while defending

        //GRAB SETTINGS
        public bool canBeGrabbed = true;
        public string grabAnimation = "Grab";
        public Vector2 grabPosition = new Vector2(0.93f, 0);
        public float grabDuration = 3f;

        //EQUIPPED WEAPON SETTINGS
        public bool loseWeaponWhenHit = true;
        public bool loseWeaponWhenKnockedDown = true;

        //UNIT NAME AND PORTRAIT
        public string unitName = ""; //the name of this unit
        public bool showNameInAllCaps; //this the name in capital letters
        public Sprite unitPortrait; //small unit portrait next to the healtbar
        public bool loadRandomNameFromList; //true if you want to load a random name from a txt file
        public TextAsset unitNamesList; //list of names (.txt file)

        //ENEMY SETTINGS
        public float enemyPauseBeforeAttack = .3f; //timeout before enemy attacks the target

        //FIELD OF VIEW
        public bool enableFOV; //use FOV to spot the target, when false the target is always spotted by default
        public float viewDistance = 5f; // The maximum distance the GameObject can see
        public float viewAngle = 45f; // The angle of the field of view
        public Vector2 viewPosOffset; //the view cone position offset (eye level)
        public bool showFOVCone; //show the FOV cone in the Unity Editor
        [ReadOnlyProperty] public bool targetInSight; //true if the target is in the field of view of this enemy
        private UnitActions unitActions => GetComponent<UnitActions>();

        void OnEnable()
        {
            GlobalVariables.OnMoveSpeedChanged += UpdateMoveSpeed;
            GlobalVariables.OnMoveSpeedAirChanged += UpdateMoveSpeedAir;

            GlobalVariables.OnJumpHeightChanged += UpdateJumpHeight;
            GlobalVariables.OnJumpSpeedChanged += UpdateJumpSpeed;
            GlobalVariables.OnJumpGravityChanged += UpdateJumpGravity;

            GlobalVariables.OnRearDefenseEnabledChanged += UpdateRearDefenseEnabled;

            GlobalVariables.OnAttackDamageAddUpChanged += UpdateAttackDamageAddUp;
        }

        void OnDisable()
        {
            GlobalVariables.OnMoveSpeedChanged -= UpdateMoveSpeed;
            GlobalVariables.OnMoveSpeedAirChanged -= UpdateMoveSpeedAir;

            GlobalVariables.OnJumpHeightChanged -= UpdateJumpHeight;
            GlobalVariables.OnJumpSpeedChanged -= UpdateJumpSpeed;
            GlobalVariables.OnJumpGravityChanged -= UpdateJumpGravity;

            GlobalVariables.OnRearDefenseEnabledChanged -= UpdateRearDefenseEnabled;

            GlobalVariables.OnAttackDamageAddUpChanged -= UpdateAttackDamageAddUp;
        }

        void Start()
        {

            //create shadow
            if (!shadow && shadowPrefab) shadow = GameObject.Instantiate(shadowPrefab, transform.parent) as GameObject;

            //hide hitbox at start
            if (hitBox) {
                if (!debugHitbox) hitBox.color = Color.clear;
            }
            else {
                Debug.LogError("Please assign a HitBox to GameObject " + gameObject.name + " in UnitSettings/Linked Components");
            }

            //check sprite renderer
            if (spriteRenderer == null) Debug.Log("Please assign a SpriteRenderer to GameObject " + gameObject.name + " in UnitSettings/Linked Components");

            //load name
            if (loadRandomNameFromList) unitName = GetRandomName();

            if (GlobalVariables.Instance != null)
            {
                //update global move speed
                if (
                    GlobalVariables.Instance.globalMoveSpeed == 0
                    || GlobalVariables.Instance.globalMoveSpeedAir == 0
                    || GlobalVariables.Instance.globalJumpHeight == 0
                    || GlobalVariables.Instance.globalJumpSpeed == 0
                    || GlobalVariables.Instance.globalJumpGravity == 0
                    || GlobalVariables.Instance.globalRearDefenseEnabled == false
                // || GlobalVariables.Instance.globalAttackDamageAddUp == 0
                )
                {
                    // only update the move speed for player, not for enemies
                    if (unitType == UNITTYPE.PLAYER)
                    {
                        GlobalVariables.Instance.globalMoveSpeed = moveSpeed;
                        GlobalVariables.Instance.globalMoveSpeedAir = moveSpeedAir;

                        GlobalVariables.Instance.globalJumpHeight = jumpHeight;
                        GlobalVariables.Instance.globalJumpSpeed = jumpSpeed;
                        GlobalVariables.Instance.globalJumpGravity = jumpGravity;

                        GlobalVariables.Instance.globalRearDefenseEnabled = rearDefenseEnabled;

                        GlobalVariables.Instance.globalAttackDamageAddUp = attackDamageAddUp;
                    }
                }
                else
                {
                    //update the move speed of this unit
                    if (unitType == UNITTYPE.PLAYER)
                    {
                        moveSpeed = GlobalVariables.Instance.globalMoveSpeed;
                        moveSpeedAir = GlobalVariables.Instance.globalMoveSpeedAir;

                        jumpHeight = GlobalVariables.Instance.globalJumpHeight;
                        jumpSpeed = GlobalVariables.Instance.globalJumpSpeed;
                        jumpGravity = GlobalVariables.Instance.globalJumpGravity;

                        rearDefenseEnabled = GlobalVariables.Instance.globalRearDefenseEnabled;

                        attackDamageAddUp = GlobalVariables.Instance.globalAttackDamageAddUp;
                    }
                }
            }

            // // List all the attacks in the enemy attack list
            // foreach (var attack in enemyAttackList)
            // {
            //     Debug.Log($"Attack Name: {attack.name}, Damage: {attack.damage}");
            //     // Scripts/Units/AttackData.cs
            // }
        }

        void Update()
        {
            if (GlobalVariables.Instance != null)
            {
                //update global move speed
                if (
                    GlobalVariables.Instance.globalMoveSpeed == 0
                    || GlobalVariables.Instance.globalMoveSpeedAir == 0
                    || GlobalVariables.Instance.globalJumpHeight == 0
                    || GlobalVariables.Instance.globalJumpSpeed == 0
                    || GlobalVariables.Instance.globalJumpGravity == 0
                    || GlobalVariables.Instance.globalRearDefenseEnabled == false
                )
                {
                    // only update the move speed for player, not for enemies
                    if (unitType == UNITTYPE.PLAYER)
                    {
                        GlobalVariables.Instance.globalMoveSpeed = moveSpeed;
                        GlobalVariables.Instance.globalMoveSpeedAir = moveSpeedAir;

                        GlobalVariables.Instance.globalJumpHeight = jumpHeight;
                        GlobalVariables.Instance.globalJumpSpeed = jumpSpeed;
                        GlobalVariables.Instance.globalJumpGravity = jumpGravity;

                        GlobalVariables.Instance.globalRearDefenseEnabled = rearDefenseEnabled;

                        GlobalVariables.Instance.globalAttackDamageAddUp = attackDamageAddUp;
                    }
                }
                else
                {
                    //update the move speed of this unit
                    if (unitType == UNITTYPE.PLAYER)
                    {
                        moveSpeed = GlobalVariables.Instance.globalMoveSpeed;
                        moveSpeedAir = GlobalVariables.Instance.globalMoveSpeedAir;

                        jumpHeight = GlobalVariables.Instance.globalJumpHeight;
                        jumpSpeed = GlobalVariables.Instance.globalJumpSpeed;
                        jumpGravity = GlobalVariables.Instance.globalJumpGravity;

                        rearDefenseEnabled = GlobalVariables.Instance.globalRearDefenseEnabled;

                        attackDamageAddUp = GlobalVariables.Instance.globalAttackDamageAddUp;
                    }
                }
            }

            //Show hitbox debug info in Unity Editor
            if (hitBox && hitBox.gameObject.activeSelf) MathUtilities.DrawRectGizmo(hitBox.bounds.center, hitBox.bounds.size, Color.red, Time.deltaTime);

            //let blobshadow follow this unit
            if (shadow)
            {
                shadow.transform.position = new Vector3(transform.position.x, unitActions.groundPos, 0);
                if (spriteRenderer) shadow.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder - 1;//put shadow behind unit
            }

            //set unit sorting order
            ObjectSorting.Sort(spriteRenderer, new Vector2(transform.position.x, unitActions ? unitActions.groundPos : transform.position.y));

            //target in FOV
            targetInSight = unitActions != null ? unitActions.targetInSight() : false;
        }

        //returns a random name
        string GetRandomName()
        {

            if (unitNamesList == null)
            {
                Debug.Log("no list of unit names was found, please create a .txt file with names on each line, and link it in the unitSettings component.");
                return "";
            }

            //convert the lines of the txt file to an array
            string data = unitNamesList.ToString();
            string cReturns = System.Environment.NewLine + "\n" + "\r";
            string[] lines = data.Split(cReturns.ToCharArray());

            //pick a random name from the list
            string name = "";
            int cnt = 0;
            while (name.Length == 0 && cnt < 100)
            {
                int rand = Random.Range(0, lines.Length);
                name = lines[rand];
                cnt += 1;
            }
            return name;
        }

        //show start direction in Unity Editor
        private void OnValidate()
        {
            transform.localRotation = (startDirection == DIRECTION.LEFT) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        //update the move speed of this unit
        private void UpdateMoveSpeed(float newMoveSpeed)
        {
            if (unitType == UNITTYPE.PLAYER)
                moveSpeed = newMoveSpeed;
        }
        //update the move air speed of this unit
        private void UpdateMoveSpeedAir(float newMoveSpeedAir)
        {
            if (unitType == UNITTYPE.PLAYER)
                moveSpeedAir = newMoveSpeedAir;
        }

        //update the jump height of this unit
        private void UpdateJumpHeight(float newJumpHeight)
        {
            if (unitType == UNITTYPE.PLAYER)
                jumpHeight = newJumpHeight;
        }

        //update the jump speed of this unit
        private void UpdateJumpSpeed(float newJumpSpeed)
        {
            if (unitType == UNITTYPE.PLAYER)
                jumpSpeed = newJumpSpeed;
        }

        //update the jump gravity of this unit
        private void UpdateJumpGravity(float newJumpGravity)
        {
            if (unitType == UNITTYPE.PLAYER)
                jumpGravity = newJumpGravity;
        }

        //  update the 
        private void UpdateRearDefenseEnabled(bool newRearDefenseEnabled)
        {
            if (unitType == UNITTYPE.PLAYER)
                rearDefenseEnabled = newRearDefenseEnabled;
        }

        // update the attack damage of this unit
        private void UpdateAttackDamageAddUp(int newAttackDamageAddUp)
        {
            if (unitType == UNITTYPE.PLAYER)
            {
                Debug.Log($"[UnitSettings] Attack Damage Add Up: {newAttackDamageAddUp}");

                jumpPunch.damage += newAttackDamageAddUp;
                jumpKick.damage += newAttackDamageAddUp;
                grabPunch.damage += newAttackDamageAddUp;
                grabKick.damage += newAttackDamageAddUp;
                groundPunch.damage += newAttackDamageAddUp;
                groundKick.damage += newAttackDamageAddUp;
            }
        }

        private void GatherAllAttacks()
        {
            // Gather all attacks from the enemyAttackList, and put them into a multiline string
            string allAttacks = "All Attacks:\n";
            foreach (var attack in enemyAttackList)
            {
                allAttacks += $"{attack.name}: {attack.damage}\n";
            }
        }
    }
}