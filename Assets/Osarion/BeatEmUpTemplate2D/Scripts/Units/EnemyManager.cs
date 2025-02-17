using UnityEngine;
using System.Collections.Generic;

namespace BeatEmUpTemplate2D {

    public static class EnemyManager {

	    public static List<GameObject> enemyList = new List<GameObject>(); //total list of enemies in the current level

	    //Remove enemy from enemyList
	    public static void RemoveEnemyFromList(GameObject enemy){
		    enemyList.Remove(enemy);
	    }

        //Add enemy to enemyList
	    public static void AddEnemyToList(GameObject enemy){
            if(!enemyList.Contains(enemy)) enemyList.Add(enemy);
	    }

        //Set all enemies to Idle state
	    public static void DisableAllEnemyAI(){
            foreach(GameObject enemy in enemyList){
                enemy.GetComponent<EnemyBehaviour>().AI_Active = false;
                enemy.GetComponent<StateMachine>()?.SetState(new EnemyIdle());
            }
	    }

        //returns the number of enemies that are currently attacking the player
        public static int GetEnemyAttackerCount(){
            int attackerCount = 0;
            foreach(GameObject enemy in enemyList){
                if(!enemy) continue;
                StateMachine stateMachine = enemy.GetComponent<StateMachine>();
                if(stateMachine == null) continue;
                else if(stateMachine.GetCurrentState() is EnemyAttack || stateMachine.GetCurrentState() is EnemyMoveToTargetAndAttack) attackerCount ++;
            }
            return attackerCount;
        }

        //Return a random enemy from enemyList
        public static GameObject GetRandomEnemy(){
            if(enemyList.Count == 0) return null; //return null if there are no enemies       
            int i = Random.Range(0, enemyList.Count);
            return enemyList[i]; //return a random enemy from  enemyList
        }

        //Returns the total number of enemies left in this level
        public static int GetTotalEnemyCount(){
            int totalEnemyCount = 0;
            foreach(HealthSystem hs in GameObject.FindObjectsOfType<HealthSystem>(true)){
               if(hs.isEnemy && hs.currentHp > 0) totalEnemyCount ++;
            }
            return totalEnemyCount;
        }

        //Returns the number of enemies that are currently active
        public static int GetCurrentEnemyCount(){
            int totalEnemyCount = 0;
            foreach(HealthSystem hs in GameObject.FindObjectsOfType<HealthSystem>(false)){
               if(hs.isEnemy && hs.currentHp > 0) totalEnemyCount ++;
            }
            return totalEnemyCount;
        }
    }
}