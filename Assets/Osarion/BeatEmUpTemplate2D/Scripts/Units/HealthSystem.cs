using UnityEngine;
using System.Collections;

namespace BeatEmUpTemplate2D
{

    //healthsystem class for player, enemy and objects
    public class HealthSystem : MonoBehaviour
    {

        public int maxHp = 1;
        public int currentHp = 1;
        public bool invulnerable;
        public bool isDead => (currentHp == 0);
        public float healthPercentage => (float)currentHp / (float)maxHp;

        [Header("HealthBar Settings")]
        public bool showSmallHealthBar; //small healthbar above the unit
        public Vector2 smallHealthBarOffset = Vector2.zero;
        public bool showLargeHealthBar;//shows a large healthbar, at the bottom of the screen
        private GameObject healthBar;

        [Header("SFX")]
        public string playSFXOnHit = "";
        public string playSFXOnDestroy = "";

        [Header("Effects")]
        public bool showHitFlash = true;
        public float hitFlashDuration = .15f;
        private bool hitflashInProgress;

        [Space(10)]
        public bool showShakeEffect;
        public float shakeIntensity = .08f;
        public float shakeDuration = .5f;
        public float shakeSpeed = 50;

        [Space(10)]
        public GameObject showEffectOnHit;
        public GameObject showEffectOnDestroy;

        public bool isPlayer => gameObject.CompareTag("Player");
        public bool isEnemy => gameObject.CompareTag("Enemy");

        public delegate void OnHealthChange(HealthSystem hs);
        public static event OnHealthChange onHealthChange;
        public delegate void OnUnitDeath(GameObject Unit);
        public static event OnUnitDeath onUnitDeath;

        void OnEnable()
        {

            //add enemies to enemyList
            if (isEnemy) EnemyManager.AddEnemyToList(gameObject);
        }

        void OnDisable()
        {
            //remove enemies from enemyList
            if (isEnemy) EnemyManager.RemoveEnemyFromList(gameObject);
        }

        void Start()
        {

            //if true, create a small health bar above this unit
            if (showSmallHealthBar) CreateSmallHealthbar();

            //initialize player healthbar
            if (isPlayer && onHealthChange != null) onHealthChange(this);
        }

        //create healthbar gameobject and set it into position
        void CreateSmallHealthbar()
        {
            if (!healthBar)
            {
                healthBar = GameObject.Instantiate(Resources.Load("HealthBar")) as GameObject;
                if (healthBar == null) return;
                healthBar.transform.parent = transform;
                healthBar.transform.position = transform.position + (Vector3)smallHealthBarOffset;
                healthBar.transform.GetChild(0).transform.localScale = new Vector3((float)currentHp / (float)maxHp, 1, 1); //set hp bar to current hp
            }
        }

        //substract health
        public void SubstractHealth(int damage)
        {

            //reduce hp
            if (!invulnerable) currentHp = Mathf.Clamp(currentHp -= damage, 0, maxHp);

            //broadcast Event
            SendEvent();

            //update HealthBar
            if (!invulnerable && healthBar) healthBar.transform.GetChild(0).transform.localScale = new Vector3((float)currentHp / (float)maxHp, 1, 1);

            //play sfx
            if (currentHp > 0) BeatEmUpTemplate2D.AudioController.PlaySFX(playSFXOnHit, transform.position);
            else BeatEmUpTemplate2D.AudioController.PlaySFX(playSFXOnDestroy, transform.position);

            //show hitflash
            if (showHitFlash)
            {
                StartCoroutine(HitFlashRoutine());
            }

            //shake this object
            if (showShakeEffect && !isDead)
            {
                StopCoroutine(ShakeRoutine());
                StartCoroutine(ShakeRoutine());
            }

            //unit/object health has reached 0
            if (isDead)
            {

                //show effect on destroy
                if (showEffectOnDestroy) CreateEffect(showEffectOnDestroy);


                if (isEnemy || isPlayer)
                {

                    //send event on unit death
                    onUnitDeath(gameObject);

                }
                else
                {

                    //destroy this object
                    Destroy(gameObject);
                }

            }
            else
            {

                //show effect when hit
                if (showEffectOnHit) CreateEffect(showEffectOnHit);
            }

            // Debug.Log(gameObject.name + " has " + currentHp + " HP left");
        }

        //add health
        public void AddHealth(int amount)
        {
            currentHp = Mathf.Clamp(currentHp += amount, 0, maxHp);
            SendEvent();
        }

        //health update event
        private void SendEvent()
        {
            float CurrentHealthPercentage = 1f / maxHp * currentHp;
            if (onHealthChange != null) onHealthChange(this);
        }

        //flash white
        private IEnumerator HitFlashRoutine()
        {
            if (hitflashInProgress) yield break;
            hitflashInProgress = true;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) yield break;
            Material defaultMat = sr.material;

            //change sprite material to hitflash material
            sr.material = Resources.Load("HitFlashMat") as Material;
            yield return new WaitForSeconds(hitFlashDuration);

            //change sprite material back to normal
            sr.material = defaultMat;
            hitflashInProgress = false;
        }

        //shake this object horizontally
        private IEnumerator ShakeRoutine()
        {
            Vector3 startPos = transform.position;
            float t = 0;
            while (t < 1)
            {
                transform.position = Vector3.Lerp(startPos + (Vector3.left * shakeIntensity / 2), startPos + (Vector3.right * shakeIntensity / 2), Mathf.Sin(t * shakeSpeed));
                t += Time.deltaTime / shakeDuration;
                yield return 0;
            }
            transform.position = startPos;
        }

        //adjust healthbar positon
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (showSmallHealthBar && !healthBar) CreateSmallHealthbar(); //create healthbar if it does not exist
                if (healthBar) healthBar.transform.position = transform.position + (Vector3)smallHealthBarOffset; //update healthbar position
                if (healthBar && !showSmallHealthBar) Destroy(healthBar);
            }
        }

        //show an effect on Destroy
        public void CreateEffect(GameObject effectPrefab)
        {

            //nothing to show
            if (effectPrefab == null) return;

            //create effect
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity) as GameObject;

            if (effect != null)
            {

                //get components
                SpriteRenderer effectSpriteRenderer = effect.GetComponent<SpriteRenderer>();
                SpriteRenderer unitSpriteRenderer = GetComponent<SpriteRenderer>();

                //set the effect sorting order to the same sorting order as this unit
                if (effectSpriteRenderer != null && unitSpriteRenderer != null)
                {
                    effectSpriteRenderer.sortingOrder = unitSpriteRenderer.sortingOrder + 1;
                }
            }
        }
    }
}
