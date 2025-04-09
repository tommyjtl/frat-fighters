using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BeatEmUpTemplate2D
{

    //this class spawns waves of enemies until all enemy waves have been defeated
    public class WaveManager : MonoBehaviour
    {

        [Header("Wave Data")]
        [ReadOnlyProperty] public int totalNumberOfWaves = 0;
        [ReadOnlyProperty] public int currentWave = 0; //the number of the current active wave
        [ReadOnlyProperty] public int enemiesLeftInThisWave = 0; //the number of enemies left in this wave
        [ReadOnlyProperty] public int totalEnemiesLeft = 0; //the number of all enemies left in this level
        public bool endLevelWhenAllEnemiesAreDefeated = true; //automatically go to the level completed screen when all enemies are defeated
        public bool triggerSlowMotionOnLastEnemyKill = true; //slows down time when the last enemy is defeated
        private bool slowMotionInProgress = false;
        private bool smoothLevelBoundTransition = true; //use smooth transition of wave levelBounds
        private float levelBoundTransitionDuration = 2f; //time it takes to transition from levelBound1 to levelBound2
        private List<GameObject> enemyWaves = new List<GameObject>(); //list of all enemies waves

        [Header("Menu List")]
        [SerializeField] private string menuToOpenOnLevelFinish = "LevelCompleted";
        [SerializeField] private string menuToOpenOnAllLevelCompleted = "AllLevelsCompleted";
        [SerializeField] private string menuToOpenOnPlayerDeath = "GameOver";

        void OnEnable()
        {
            HealthSystem.onUnitDeath += OnUnitDeath; //subscribe to event
        }

        void OnDisable()
        {
            HealthSystem.onUnitDeath -= OnUnitDeath; //unsubscribe to event
        }

        void Start()
        {
            GetAllEnemyWaves();
            DisableAllWaves();
            ActivateWave(0);
            totalEnemiesLeft = EnemyManager.GetTotalEnemyCount();
        }

        //get all enemy waves (Child gameobjects)
        void GetAllEnemyWaves()
        {
            for (int i = 0; i < transform.childCount; i++) enemyWaves.Add(transform.GetChild(i).gameObject);
            totalNumberOfWaves = enemyWaves.Count;
        }

        //disable all waves
        void DisableAllWaves()
        {
            foreach (GameObject wave in enemyWaves) wave.SetActive(false);
        }

        //enable current wave
        void ActivateWave(int wave)
        {
            // print enemyWaves.Count
            // Debug.Log("enemyWaves.Count: " + enemyWaves.Count);

            // print the wave count:L 
            // Debug.Log("Wave " + wave + " of " + totalNumberOfWaves + " has been activated");
            // Debug.Log("EnemyManager.GetTotalEnemyCount()\t" + EnemyManager.GetTotalEnemyCount());


            //do nothing if there are no waves
            if (enemyWaves.Count == 0 && wave == 0) return;

            //finish if there are no more waves left, or if all enemies are dead
            if (wave >= enemyWaves.Count || EnemyManager.GetTotalEnemyCount() == 0)
            {
                OnFinish();
                return;
            }

            //otherwise activate next wave
            currentWave = wave;
            if (enemyWaves[wave] == null) return;
            else enemyWaves[wave].SetActive(true);

            //set new levelBound
            if (smoothLevelBoundTransition && currentWave > 0) StartCoroutine(LevelBoundTransition()); //smoothly transition from levelbound A to B
            else SetCameraLevelBound(); //instantly set levelBound A to B

            //show enemies left in this wave (debug)
            enemiesLeftInThisWave = EnemyManager.GetCurrentEnemyCount();
        }

        //event
        public void OnUnitDeath(GameObject unit)
        {

            //update total enemies left in this level (debug)
            totalEnemiesLeft = EnemyManager.GetTotalEnemyCount();

            //if the player is dead...
            if (unit.GetComponent<UnitSettings>()?.unitType == UNITTYPE.PLAYER)
            {
                OnPlayerDeath();
                return;
            }

            //if an enemy is dead...
            if (
                unit.GetComponent<UnitSettings>()?.unitType == UNITTYPE.ENEMY || // if an enemy is dead
                unit.GetComponent<UnitSettings>()?.unitType == UNITTYPE.BOSS) // or if a boss is dead
            {
                // Debug.Log("[WaveManager] " + "An enemy has been defeated");

                // obj?.GetComponent<XPSystem>()?.AddXP(20);
                // Debug.Log("[WaveManager] " + "Added 20 XP");

                //show slow motion effect on last enemy destroy
                if (triggerSlowMotionOnLastEnemyKill && totalEnemiesLeft == 0) StartCoroutine(StartSlowMotionEffectRoutine());

                //if 0 enemies are left in this wave... start next wave
                enemiesLeftInThisWave = EnemyManager.GetCurrentEnemyCount();
                if (enemiesLeftInThisWave == 0) ActivateWave(currentWave + 1);
            }
        }

        //move LevelBound1 to LevelBound2 for smooth transition
        IEnumerator LevelBoundTransition()
        {

            //get levelBound 1 and 2
            LevelBound LevelBound1 = enemyWaves[currentWave - 1].GetComponentInChildren<LevelBound>();
            LevelBound LevelBound2 = enemyWaves[currentWave].GetComponentInChildren<LevelBound>();

            //skip transition if levelbounds cannot be found
            if (!LevelBound1 || !LevelBound2) { SetCameraLevelBound(); yield break; }

            //transition by moving levelBound1 to position of levelBound2
            Vector3 From = LevelBound1.transform.position; //from position
            Vector3 To = LevelBound2.transform.position; //to position

            float t = 0;
            while (t < 1)
            {
                LevelBound1.transform.position = Vector3.Lerp(From, To, MathUtilities.CoSinLerp(t));
                t += Time.deltaTime / levelBoundTransitionDuration;
                yield return 0;
            }

            SetCameraLevelBound();
        }

        //set camera levelBound
        void SetCameraLevelBound()
        {

            //find levelBound of current wave
            LevelBound currentWaveLevelBound = enemyWaves[currentWave].GetComponentInChildren<LevelBound>();

            //set camera levelbound to current wave levelBound
            CameraFollow camFol = Camera.main.GetComponent<CameraFollow>();
            if (currentWaveLevelBound != null && camFol != null) camFol.levelBound = currentWaveLevelBound;

            //cleanup / destroy previous wave
            if (currentWave > 0) Destroy(enemyWaves[currentWave - 1].gameObject);
        }

        //all waves have been defeated
        void OnFinish()
        {

            //update field
            currentWave = totalNumberOfWaves;

            //stop here if the user does not want to show a screen when all enemies are finished
            if (!endLevelWhenAllEnemiesAreDefeated) return;

            //complete this level
            SaveLevelProgress();

            //try to find the UI Manager
            UIManager uiManager = GameObject.FindObjectOfType<UIManager>();
            if (uiManager == null) { Debug.Log("No UI Manager found in this Level, can't show level completed screen"); return; }

            //if this is the last level, show all levels completed screen
            if (LevelProgress.isLastLevel)
            {
                var pauseMenu = FindObjectOfType<UIPauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.CanBePaused = false;
                }
                uiManager.ShowMenu(menuToOpenOnAllLevelCompleted);

            }
            else
            {
                var pauseMenu = FindObjectOfType<UIPauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.CanBePaused = false;
                }

                //show level finished screen
                uiManager.ShowMenu(menuToOpenOnLevelFinish);
            }
        }

        //save level as completed
        void SaveLevelProgress()
        {
            string currentLevelScene = SceneManager.GetActiveScene().name;
            if (!LevelProgress.levelsCompleted.Contains(currentLevelScene)) LevelProgress.levelsCompleted.Add(currentLevelScene);
        }

        //start slow motion effect
        IEnumerator StartSlowMotionEffectRoutine()
        {
            if (slowMotionInProgress) yield break;
            Time.timeScale = .5f; //slow down time
            yield return new WaitForSecondsRealtime(1.5f);
            while (Time.timeScale < 1f) Time.timeScale += Time.deltaTime; //speed up again
            Time.timeScale = 1f;
        }

        //show menu when player is dead
        void OnPlayerDeath()
        {
            UIManager UI = GameObject.FindObjectOfType<UIManager>();
            if (UI != null) UI.ShowMenu(menuToOpenOnPlayerDeath);
        }
    }
}