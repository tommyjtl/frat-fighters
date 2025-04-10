using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeatEmUpTemplate2D
{

    //functions that can be utilized by units across different states.
    public class UnitActions : MonoBehaviour
    {

        [HideInInspector] public GameObject target;
        [HideInInspector] public float groundPos = 0; //the current position on the ground
        [HideInInspector] public Vector2 currentPosition => new Vector2(transform.position.x, groundPos); //the current position on the ground
        [HideInInspector] public float lastAttackTime = 0; //time of the last attack
        [HideInInspector] public ATTACKTYPE lastAttackType; //last attack type
        [HideInInspector] public float yForce = 0; //used for jump calculations
        [HideInInspector] public bool isGrounded = true; //if a unit is currently on the ground
        [HideInInspector] public WeaponPickup weapon; //the current weapon this unit is holding
        [HideInInspector] public bool targetSpotted; //true if a target was spotted at least once
        [HideInInspector] public CapsuleCollider2D col2D;
        [HideInInspector] public List<ATTACKTYPE> attackList = new List<ATTACKTYPE>();

        public Animator animator => GetComponent<Animator>();
        public StateMachine stateMachine => GetComponent<StateMachine>();
        public UnitSettings settings => GetComponent<UnitSettings>();
        public Rigidbody2D rb => GetComponent<Rigidbody2D>();
        public bool isPlayer => settings?.unitType == UNITTYPE.PLAYER;
        public bool isEnemy => settings?.unitType == UNITTYPE.ENEMY || settings?.unitType == UNITTYPE.BOSS; // if this unit is an enemy, include bosses
        public bool isBoss => settings?.unitType == UNITTYPE.BOSS; // if this unit is a boss
        private bool onApplicationQuit;
        private float currentSpeed = 0f;
        private float animDuration;
        public DIRECTION dir => transform.localRotation == Quaternion.Euler(Vector3.zero) ? DIRECTION.RIGHT : DIRECTION.LEFT; //the current direction
        public DIRECTION invertedDir => (DIRECTION)((int)dir * -1); //the inverted direction

        public delegate void OnUnitDealDamage(GameObject recipient, AttackData attackData);
        public static event OnUnitDealDamage onUnitDealDamage;

        void OnDestroy()
        {

            //when this unit is destroyed, also destroy its shadow
            if (settings?.shadow && !onApplicationQuit) Destroy(settings.shadow);
        }

        //returns the closest player
        public GameObject findClosestPlayer()
        {
            List<GameObject> allPlayers = GameObject.FindGameObjectsWithTag("Player").ToList(); //find all players in the scene
            allPlayers = allPlayers.OrderBy(player => Vector2.Distance(this.transform.position, player.transform.position)).ToList(); //sort players based on distance
            if (allPlayers.Count > 0) return allPlayers[0]; //return the first player
            return null; //return nothing if there is no player
        }

        //returns the distance to the target x,y
        public Vector2 distanceToTarget()
        {
            if (!target) return Vector2.positiveInfinity;
            return new Vector2(Mathf.Abs(target.transform.position.x - transform.position.x), Mathf.Abs(target.transform.position.y - transform.position.y));
        }

        //turn sprite towards the target
        public void TurnToTarget()
        {
            if (!target) return;
            transform.localRotation = (target.transform.position.x < transform.position.x) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        //turn towards a direction
        public void TurnToDir(DIRECTION dir)
        {
            transform.localRotation = (dir == DIRECTION.LEFT) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        //turn towards a floating point direction: above zero is right, below zero is left, do nothing when 0
        public void TurnToFloatDir(float x)
        {
            if (x == 0) return; //do nothing
            if (x > 0) TurnToDir(DIRECTION.RIGHT);
            else if (x < 0) TurnToDir(DIRECTION.LEFT);
        }

        // Handle revenge behaviour execution
        private void HandleRevengeBehavior(UnitActions targetUnit, AttackData attackData)
        {
            RevengeSystem revengeSystem = targetUnit?.GetComponent<RevengeSystem>();
            if (revengeSystem == null) return;

            // If attack is from the player and is a melee attack, increase revenge
            if (attackData.attackType == ATTACKTYPE.PUNCH || attackData.attackType == ATTACKTYPE.KICK)
            {
                revengeSystem.IncreaseRevengeMeter(1);
            }

            // If target is already in revenge mode, let the revenge system handle response
            //if (revengeSystem.IsInRevengeMode())
            //{
            //    targetUnit.stateMachine.SetState(new EnemyRevengeMoveToTargetAndAttack(revengeSystem));
            //}
        }

        //check if a enemy was hit by this unit's hitbox
        public bool CheckForHit(AttackData attackData)
        {
            bool damageDealt = false;
            if (attackData.inflictor == null) attackData.inflictor = gameObject; //pass this gameobject as the inflictor
            if (HitBoxActive())
            {

                //apply damage to objects that are hit
                foreach (GameObject obj in GetObjectsHit(attackData))
                {

                    //check if the target unit is able to defend against attacks coming from this direction
                    UnitActions targetUnit = obj.GetComponent<UnitActions>();
                    if (targetUnit?.IsDefending(invertedDir) == true) { damageDealt = true; continue; }

                    //skip knockdown if this unit is already knocked down
                    bool unitKnockdownInProgress = obj.GetComponent<StateMachine>()?.GetCurrentState() is UnitKnockDown;
                    if (unitKnockdownInProgress) continue;

                    //show hit effect
                    ShowHitEffectAtPosition(settings.hitBox.transform.position + (Vector3.right * Random.Range(0, .5f)));

                    // Handle Revenge System behavior (separate from health)
                    HandleRevengeBehavior(targetUnit, attackData);

                    //substract health
                    HealthSystem targetHealthSystem = obj.GetComponent<HealthSystem>();
                    if (attackData != null) targetHealthSystem?.SubstractHealth(attackData.damage);

                    //play hit sfx (if any)
                    if (attackData.sfx.Length > 0) AudioController.PlaySFX(attackData.sfx);

                    //send event
                    if (onUnitDealDamage != null) onUnitDealDamage(obj, attackData);

                    //hit this unit
                    if (targetUnit != null && targetUnit.isGrounded)
                    {
                        //check if this unit is dead
                        if (targetHealthSystem.isDead)
                        {
                            obj.GetComponent<StateMachine>().SetState(new UnitDeath(true));

                            // print the object name in the console if it's dead
                            // Debug.Log("[UnitActions]\t" + obj.name + " is dead");

                            // if (attackData != null)
                            // {
                            //     obj?.GetComponent<XPSystem>()?.AddXP(20);
                            //     Debug.Log("[UnitActions]\t" + "Added 20 XP to " + obj.name);
                            // }
                        }
                        else
                        {
                            //determine if this is a knockdown and if the unit can be knocked down
                            bool doAKnockdown = (attackData.knockdown && targetUnit.settings.canBeKnockedDown);

                            if (!doAKnockdown)
                            {

                                //do a default hit
                                targetUnit.stateMachine.SetState(new UnitHit());

                            }
                            else
                            {

                                //do a knockdown hit
                                Vector2 knockDownForce = new Vector2(targetUnit.settings.knockDownDistance, targetUnit.settings.knockDownHeight); //get knockdown data
                                targetUnit.stateMachine.SetState(new UnitKnockDown(attackData, knockDownForce.x, knockDownForce.y));
                            }
                        }
                    }
                    damageDealt = true;
                }
            }
            return damageDealt;
        }

        //returns a list of gameObjects that we've hit
        public List<GameObject> GetObjectsHit(AttackData attackData)
        {
            List<GameObject> hitableObjects = new List<GameObject>(); //list of possible objects to check
            List<GameObject> ObjectsHit = new List<GameObject>(); //list of objects that are hit

            //if this unit is a player, create a list of enemies and hitable objects
            if (isPlayer)
            {

                //collect enemies
                hitableObjects = GameObject.FindGameObjectsWithTag("Enemy").ToList();

                //collect objects
                hitableObjects.AddRange(GameObject.FindGameObjectsWithTag("Object").ToList());
            }

            //if this unit is a enemy, create a list hitable objects
            if (isEnemy)
            {

                //check if this enemy is being thrown
                bool enemyIsBeingThrown = (attackData.attackType == ATTACKTYPE.GRABTHROW);

                //check if this enemy is being thrown
                bool enemyDoesFallDamage = settings.hitOtherEnemiesWhenFalling;

                //collect player
                if (!enemyIsBeingThrown) hitableObjects = GameObject.FindGameObjectsWithTag("Player").ToList();

                //if this enemy if being thrown or does fall damage to other enemies, add other enemies to the list of hitable objects
                if (enemyIsBeingThrown || enemyDoesFallDamage)
                {
                    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                    {
                        bool enemyIsMyself = (enemy.gameObject == this.gameObject);
                        bool enemyIsKnockedDown = enemy.GetComponent<StateMachine>()?.GetCurrentState() is UnitKnockDown || enemy.GetComponent<StateMachine>()?.GetCurrentState() is UnitKnockDownGrounded;
                        if (!enemyIsMyself && !enemyIsKnockedDown) hitableObjects.Add(enemy);
                    }
                }
            }

            //exclude dead units/objects
            for (int i = hitableObjects.Count - 1; i >= 0; i--)
            {
                if (hitableObjects[i].GetComponent<HealthSystem>()?.isDead == true) hitableObjects.RemoveAt(i);
            }

            //exclude units already in their hit state
            for (int i = hitableObjects.Count - 1; i >= 0; i--)
            {
                if (hitableObjects[i].GetComponent<StateMachine>()?.GetCurrentState() is UnitHit) hitableObjects.RemoveAt(i);
            }

            //sort objects based on distance
            hitableObjects = hitableObjects.OrderBy(obj => Vector2.Distance(transform.position, obj.transform.position)).ToList();

            //check if the hitbox intersects a hitable object
            foreach (GameObject obj in hitableObjects)
            {
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null && settings.hitBox.bounds.Intersects(sr.bounds) && targetInZRange(obj.gameObject, .5f)) ObjectsHit.Add(sr.gameObject);
            }

            //return objects that are hit
            return ObjectsHit;
        }


        //returns a nearby pickup (if there is one)
        public GameObject GetClosestPickup(Vector2 pickupRange)
        {
            List<GameObject> allPickups = GameObject.FindGameObjectsWithTag("Pickup").ToList().OrderBy(pickup => Vector2.Distance(transform.position, pickup.transform.position)).ToList(); //find objects and sort them based on distance
            if (allPickups.Count == 0) return null;

            //check if a pickup is within the specified range
            foreach (var pickup in allPickups)
            {
                if (Vector2.Distance(transform.position, pickup.transform.position) <= pickupRange.magnitude) return pickup; //return the first pickup found within range
            }
            return null;
        }

        //check if there is an enemy down nearby
        public GameObject NearbyEnemyDown()
        {
            if (EnemyManager.enemyList.Count == 0) return null; //do nothing if there are no enemies
            float range = 1;
            foreach (GameObject enemy in EnemyManager.enemyList)
            {
                if (enemy && Vector2.Distance(transform.position, enemy.transform.position) < range)
                {

                    //skip dead enemies
                    if (enemy.GetComponent<HealthSystem>()?.isDead == true) continue;

                    //check if a nearby enemy is currently in UnitKnockDownGrounded state
                    StateMachine targetStateMachine = enemy.GetComponent<StateMachine>();
                    if (targetStateMachine?.GetCurrentState() is UnitKnockDownGrounded) return enemy;
                }
            }
            return null;
        }

        //returns true if a target is within z range
        bool targetInZRange(GameObject target, float zRange)
        {
            return (Mathf.Abs(target.transform.position.y - groundPos) < zRange);
        }

        //move to vector dir
        public void MoveToVector(Vector2 moveDir, float moveSpeed)
        {
            if (rb == null) return;
            if (isGrounded) groundPos = transform.position.y;

            //accelerate or decelerate
            if (settings.useAcceleration)
            {
                if (moveDir.magnitude > 0.1f) currentSpeed = Mathf.Min(currentSpeed + settings.moveAcceleration * Time.fixedDeltaTime, moveSpeed); //Accelerate towards top Speed
            }
            else
            {
                if (moveDir.magnitude > 0.1f) currentSpeed = settings.moveSpeed; //instantly set to top speed (don't use acceleration)
            }

            //move
            rb.velocity = moveDir * currentSpeed;
            if (col2D == null) col2D = GetComponent<CapsuleCollider2D>();
            if (moveDir.x != 0) TurnToDir((moveDir.x > 0) ? DIRECTION.RIGHT : DIRECTION.LEFT); //turn towards move dir
        }

        public bool WallDetected(Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.Linecast(currentPosition, currentPosition + dir, 1 << LayerMask.NameToLayer("Environment"));
            Debug.DrawRay(currentPosition, dir, Color.yellow, Time.deltaTime);
            return (hit.collider != null);
        }

        //burst forward in the facing direction
        public void AddForce(float force)
        {
            StartCoroutine(AddForceRoutine(force, .25f));
        }

        //add force coroutine
        private IEnumerator AddForceRoutine(float force, float duration)
        {
            Vector2 startPos = transform.position;
            Vector2 endPos = startPos + Vector2.right * (int)dir * force;
            float t = 0;
            while (t < 1)
            {
                transform.position = Vector2.Lerp(startPos, endPos, MathUtilities.Sinerp(t));
                t += Time.deltaTime / duration;
                yield return 0;
            }
            transform.position = endPos;
        }

        //code for unit movement during a jump
        public void JumpSequence()
        {
            Vector2 moveVector = transform.position;

            //horizontal movement
            float inputVectorX = InputManager.GetInputVector().x; //get user input in x direction
            if (inputVectorX != 0) TurnToDir(inputVectorX > 0 ? DIRECTION.RIGHT : DIRECTION.LEFT); //turn towards x input direction
            moveVector.x = transform.position.x + (inputVectorX * settings.moveSpeedAir * Time.fixedDeltaTime); //calculate speed based on settings

            //vertical movement
            moveVector.y += yForce * Time.fixedDeltaTime * settings.jumpSpeed;
            yForce -= settings.jumpGravity * Time.fixedDeltaTime * settings.jumpSpeed;

            //position collider on the floor by applying offset
            if (col2D) col2D.offset = new Vector2(col2D.offset.x, -(moveVector.y - groundPos));

            //set unit position
            transform.position = moveVector;
        }

        //returns the duration (time) of an animation
        public float GetAnimDuration(string animName)
        {
            //Debug.Log("current clip name: " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(animName) && (animator.GetCurrentAnimatorStateInfo(0).length > 0)) animDuration = animator.GetCurrentAnimatorStateInfo(0).length;
            return animDuration;
        }

        //stop moving
        public void StopMoving(bool stopInstantly = true)
        {
            if (isGrounded) groundPos = transform.position.y;
            if (!settings.useAcceleration) stopInstantly = true;

            //instant stop
            if (stopInstantly)
            {
                rb.velocity = Vector2.zero;
                currentSpeed = 0;
                return;
            }

            //stop using Deceleration
            Vector2 moveDir = rb.velocity.normalized;
            currentSpeed = Mathf.Max(currentSpeed - settings.moveDeceleration * Time.fixedDeltaTime, 0f); //Decelerate
            rb.velocity = moveDir * currentSpeed;
        }

        //returns true if this unit is currently defending
        public bool IsDefending(DIRECTION attackDir)
        {

            //is this is an enemy that has a defend chance enabled, calculate defence probability
            if (isEnemy && settings.defendChance > 0)
            {
                if (Random.Range(0, 100) < settings.defendChance) stateMachine.SetState(new UnitDefend()); //defend
            }

            //check if this unit is currently in defended state
            bool unitIsDefending = stateMachine.GetCurrentState() is UnitDefend;
            if (!unitIsDefending) return false;

            //send hit event to UnitDefend State
            if (settings.rearDefenseEnabled || dir == attackDir) (stateMachine.GetCurrentState() as UnitDefend)?.Hit();

            if (settings.rearDefenseEnabled) return true; //defend from all directions
            if (dir == attackDir) return true; //only defend front facing attacks
            return false;
        }

        //returns true if the hitbox is currently active
        public bool HitBoxActive()
        {
            if (settings.hitBox == null) return false;
            return settings.hitBox.gameObject.activeSelf;
        }

        //show a hit effect
        public void ShowHitEffectAtPosition(Vector2 pos)
        {
            if (!settings.hitEffect) return;
            GameObject effect = GameObject.Instantiate(settings.hitEffect, (Vector3)pos, Quaternion.identity) as GameObject;
        }

        //play sfx
        public void PlaySFX(string sfx)
        {
            BeatEmUpTemplate2D.AudioController.PlaySFX(sfx, transform.position);
        }

        //play footstep SFX depending on the surface this unit walks on
        public void Footstep()
        {
            Collider2D[] overlappedColliders = Physics2D.OverlapPointAll(transform.position); //check overlap witch other colliders
            foreach (Collider2D col2D in overlappedColliders)
            {
                Surface surface = col2D.GetComponent<Surface>();
                if (surface && surface.footstepSFX.Length > 0)
                {
                    AudioController.PlaySFX(surface.footstepSFX, transform.position);
                    return;
                }
            }
            AudioController.PlaySFX("FootstepDefault", transform.position);
        }

        //displays an effect loaded from the resources folder
        public void ShowEffect(string effectName)
        {

            //load particle effect
            GameObject effect = GameObject.Instantiate(Resources.Load(effectName), transform.position, Quaternion.identity) as GameObject;

            //set particle effect sorting order
            ParticleSystem[] allParticleSystems = effect?.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in allParticleSystems) ObjectSorting.Sort(ps?.GetComponent<Renderer>(), new Vector2(transform.position.x, transform.position.y));

            //destroy after 3 secs
            if (effect) Destroy(effect, 3f);
        }

        //spawn a gameobject loaded from the resources folder
        public void SpawnProjectile(string objName)
        {

            //determine object spawn position
            WeaponAttachment wa = GetComponentInChildren<WeaponAttachment>();
            Vector2 spawnPos = (wa != null) ? wa.transform.position : transform.position;

            //load projectile gameobject
            GameObject projectile = GameObject.Instantiate(Resources.Load(objName), spawnPos, Quaternion.identity) as GameObject;
            if (!projectile) return;

            //set data for projectile component
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent == null) return;
            projectileComponent.dir = dir; //set to unit direction
        }

        //show small camera shake
        public void CamShake()
        {
            Camera.main?.GetComponent<CameraShake>()?.ShowCamShake();
        }

        //calculates if the target sprite bounding box is within the Field Of View
        public bool targetInSight()
        {
            if (!target || !settings) return false;
            if (!settings.enableFOV) { targetSpotted = true; return true; } //if FOV is disabled, always spot the target

            //calculate the distance to the target
            Vector2 directionToTarget = target.transform.position - transform.position + (Vector3)settings.viewPosOffset;
            float distanceToTarget = directionToTarget.magnitude;
            if (distanceToTarget > settings.viewDistance) return false; // Target is outside range

            //get target spriteRenderer
            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr == null) return false;

            //transform corners of the sprite's bounding box to world space coords
            Bounds spriteBounds = sr.bounds;
            Vector3[] corners = {
                spriteBounds.min, // Bottom-left
                spriteBounds.max, // Top-right
                new Vector3(spriteBounds.min.x, spriteBounds.max.y), // Top-left
                new Vector3(spriteBounds.max.x, spriteBounds.min.y)  // Bottom-right
            };

            //check if any corners are within the FOV
            foreach (Vector3 corner in corners)
            {
                Vector2 directionToCorner = corner - (transform.position + (Vector3)settings.viewPosOffset);
                float distanceToCorner = directionToCorner.magnitude;
                if (distanceToCorner <= settings.viewDistance)
                {
                    float angleToCorner = Vector2.Angle(transform.right, directionToCorner);
                    if (angleToCorner <= settings.viewAngle / 2)
                    {
                        targetSpotted = true;
                        return true; //at least one corner is within the FOV
                    }
                }
            }
            return false;
        }

        //draw enemy FOV when this GameObject is selected in the editor
        void OnDrawGizmos()
        {
            if (settings == null || !settings.showFOVCone || settings.viewDistance <= 0) return;

            int lineSegments = settings.viewAngle > 180 ? 40 : 20; //more detail for larger circumference
            Gizmos.color = Color.red;
            Vector3 viewOffset = new Vector3(settings.viewPosOffset.x * (int)dir, settings.viewPosOffset.y, 0);

            //Calculate left and right boundary
            Vector3 forward = transform.right;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, settings.viewAngle / 2) * forward * settings.viewDistance + viewOffset;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -settings.viewAngle / 2) * forward * settings.viewDistance + viewOffset;

            //Draw arc
            Vector3 previousPoint = transform.position + rightBoundary;
            for (int i = 0; i <= lineSegments; i++)
            {

                float angle = -settings.viewAngle / 2 + (settings.viewAngle / lineSegments) * i;
                Vector3 nextPoint = Quaternion.Euler(0, 0, angle) * forward * settings.viewDistance;
                nextPoint = transform.position + nextPoint + viewOffset;

                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }

            //Draw top and bottom line
            Gizmos.DrawLine(transform.position + viewOffset, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position + viewOffset, transform.position + rightBoundary);
        }

        void OnApplicationQuit()
        {
            onApplicationQuit = true;
        }

    }

    public enum DIRECTION
    {
        LEFT = -1,
        RIGHT = 1,
    }
}