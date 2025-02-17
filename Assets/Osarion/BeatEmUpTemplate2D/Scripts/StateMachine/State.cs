namespace BeatEmUpTemplate2D {

    //state base class
    public abstract class State {

        public virtual bool canGrab => true;
        public float stateStartTime = 0;
        public UnitActions unit;
        public virtual void Update(){}
        public virtual void LateUpdate(){}
        public virtual void FixedUpdate(){}
        public virtual void Enter(){}
        public virtual void Exit(){}
    }
}