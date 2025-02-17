using UnityEngine;

namespace BeatEmUpTemplate2D {

    //state Machine class
    public class StateMachine : UnitActions {
    
        [SerializeField] private bool showStateInGame; //shows the current state in a textfield below this unit
        [ReadOnlyProperty] public string currentState; //used for displaying the current state in the unity inspector
        private TextMesh stateText; //textfield for showing state in game for debugging
        private State state; //the current state

        void Start(){

            //set to starting state
            if(isPlayer) SetState(new PlayerIdle()); //if unit if a player, go to state PlayerIdle
            else if(isEnemy) SetState(new EnemyIdle()); //if unit if a enemy, go to state EnemyIdle
        }

        public void SetState(State _state){
        
            //exit current state
            if (this.state != null) state.Exit();
       
            //set new state
            state = _state;
            state.unit = this;

            //set data
            currentState = GetCurrentStateShortName(); //debug info
            state.stateStartTime = Time.time;

            //enter the state
            state.Enter();
        }

        public State GetCurrentState(){
            return state;;
        }

        void Update(){
            state?.Update();
            UpdateStateText();
        }

        void LateUpdate(){
            state?.LateUpdate();
        }

        void FixedUpdate(){
            state?.FixedUpdate();
        }

        void UpdateStateText(){

            //if stateText should not be shown or is not initialized, do nothing
            if(!showStateInGame){
                if (stateText != null) {
                    Destroy(stateText.gameObject);
                    stateText = null;
                }
                return;
            }

            //create stateText if it does not exist
            if(stateText == null){
                GameObject stateTxtGo = Instantiate(Resources.Load("StateText"), transform) as GameObject;
                if (stateTxtGo != null) {
                    stateTxtGo.name = "StateText";
                    stateTxtGo.transform.localPosition = new Vector2(0, -0.2f);
                    stateText = stateTxtGo.GetComponent<TextMesh>();
                }
            }

            //update the state text if it's initialized
            if(stateText != null){
                stateText.text = GetCurrentStateShortName();
                stateText.transform.localRotation = Quaternion.Euler(0, dir == DIRECTION.LEFT ? 180 : 0, 0);
            }
        }

        //returns the name of the current state without the namespace
        string GetCurrentStateShortName(){
            string currentState = stateMachine?.GetCurrentState().GetType().ToString();
            string[] splitStrings = currentState.Split('.');                  
            if(splitStrings.Length >= 2) return splitStrings[1];
            return "";
        }
    }
}