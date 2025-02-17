using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace BeatEmUpTemplate2D {

    //class for the level Selection menu
    public static class LevelProgress {
        public static List<string> levelsCompleted = new List<string>(); //list of finished levels
        public static bool isLastLevel; //true if the current level is the last level
    }

    public class UILevelSelection : MonoBehaviour {

        public Transform UIItemList; //link to the UI that hold the level items
        public GameObject levelItemPrefab; //level item prefab
        public Level[] levelData; //list with level data
        [SerializeField] private List <string> completedLevels; //a local list of levels that have been completed (shown for Debugging purposes)

        void Start(){

            //remove all existing items (if any)
            ClearLevelList();

            //create level items
            CreateLevelList();

            //show level info in the unity editor
            completedLevels = LevelProgress.levelsCompleted; 
        }

        //clear all items in level list
        void ClearLevelList(){
            if(UIItemList == null) return;        

            //remove all items from the list
            foreach (Transform child in UIItemList) Destroy(child.gameObject);
        }

        //create level items based on the level Data
        void CreateLevelList(){
            if(UIItemList == null) return;

            //The UI gameobject of the furthest level completed
            GameObject furthestLevelCompleted = null;

            for(int i=0; i<levelData.Length; i++){

                //create a level item
                GameObject level = GameObject.Instantiate(levelItemPrefab, UIItemList);
                level.name = levelData[i].levelName;

                //set level image 
                Image levelImage = level.GetComponent<Image>();

                //lock or unlock this level
                bool previousLevelCompleted = LevelProgress.levelsCompleted.Contains(levelData[i>0? i-1 : 0].levelToLoad)? false : true;

                levelData[i].locked = previousLevelCompleted; //unlock this level when the previous level was completed
                if(levelData[i].alwaysUnlocked) levelData[i].locked = false; //always unlock this level if the 'alwaysUnlocked' checkbox is true

                //set locked or unlocked sprite
                levelImage.sprite = levelData[i].locked? levelData[i].levelImageLocked : levelData[i].levelImageUnlocked; 

                //get reference to furthest level
                if(!levelData[i].locked) furthestLevelCompleted = level;

                //assign OnClick button action
                int buttonId = i;
                Button levelButton = level.GetComponent<Button>();
                levelButton?.onClick.AddListener(() => OnButtonClick(buttonId));

                //enable disable button interaction whether the level is locked or not
                levelButton.interactable = !levelData[i].locked;
            }

            //select furthest level in the list by default
            if(furthestLevelCompleted && furthestLevelCompleted.GetComponent<Button>() != null){
                furthestLevelCompleted.GetComponent<Button>().Select();
            }
        }

        //when the player clicks a level
        void OnButtonClick(int buttonId) {
            if(levelData[buttonId].locked){ Debug.Log("Level is locked"); return; } //do nothing if the level is locked
            LevelProgress.isLastLevel = (buttonId == levelData.Length-1); //mark this as last level
            SceneManager.LoadScene(levelData[buttonId].levelToLoad); //load level
        }
    }

    [System.Serializable]
    public class Level {
        public string levelName;
        public Sprite levelImageLocked;
        public Sprite levelImageUnlocked;
        public string levelToLoad = "";
        public bool alwaysUnlocked;
        [ReadOnlyProperty] public bool locked;
    }
}
