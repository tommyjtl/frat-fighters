using UnityEngine;

namespace BeatEmUpTemplate2D {

    public class EnemyBehaviour : MonoBehaviour {
    
        public bool AI_Active;
        public float delayBeforeStart = 1f;
        public float decisionInterval = 1f;
        public float randomizeAmount = 1f; //value to randomize (add to) decision time
        private float lastDecisionTime = 0;
        private float startTime = 0;
        [ReadOnlyProperty] public bool targetSpotted;
        private StateMachine statemachine;
        private UnitSettings settings;

        void Start(){
            startTime = Time.time;
            statemachine = GetComponent<StateMachine>();
            settings = GetComponent<UnitSettings>();
        }

        void Update() {

            //delay before start
            if(Time.time - startTime < delayBeforeStart) return;

            //the target has been spotted
            if(!targetSpotted){
                if(statemachine.targetInSight()) targetSpotted = true;
            }

            //decision time
            if(targetSpotted && Time.time - lastDecisionTime > decisionInterval) DoSomething();
        }

        void DoSomething(){
            if(!AI_Active) return; //do nothing
            else lastDecisionTime = Time.time + Random.Range(0f, randomizeAmount);
       
            //check if this enemy is currently in EnemyIdle state
            bool isIdle = (statemachine.GetCurrentState() is EnemyIdle);
            if(!isIdle) return; //do nothing

            //get a random attack from Unit Settings
            GetRandomAttack();

            //75% chance to attack if nobody is attacking the player
            if(EnemyManager.GetEnemyAttackerCount() == 0 && Random.Range(0, 100) < 75) {
                AttackData attack = GetRandomAttack(); //get a random attack
                statemachine?.SetState(new EnemyMoveToTargetAndAttack(attack)); 
                return; 

            //25% change that this enemy attacks when 2 or less enemies are attacking the player
            } else if(EnemyManager.GetEnemyAttackerCount() <= 2 && Random.Range(0, 100) < 25) {
                AttackData attack = GetRandomAttack(); //get a random attack
                statemachine?.SetState(new EnemyMoveToTargetAndAttack(attack)); 
                return; 
            }

            //otherwise random action 1 or 2
            int i = Random.Range(1,3);

            //move closer to player
            if(i == 1) statemachine?.SetState(new EnemyKeepDistance(2f, 2f, -.5f, .5f)); 

            //move away from player
            if(i == 2) statemachine?.SetState(new EnemyKeepDistance(4f, 4f, -1f, 1f));
        }

        private AttackData GetRandomAttack(){

            //check available data
            if(settings == null || settings.enemyAttackList.Count == 0) { 
                Debug.Log("No enemy attacks available to choose from"); 
                return null; 
            }

            //select the random attack
            int rand = Random.Range(0, settings.enemyAttackList.Count);
            return settings.enemyAttackList[rand];
        }
    }
}
