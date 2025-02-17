namespace BeatEmUpTemplate2D {

    public class PlayerIdle : State {

        private string animationName = "Idle";
    
        public override void Enter(){
            unit.animator.Play(animationName);
        }

        public override void Update(){
        
            //stop moving
            unit.StopMoving(false);

            //defend
            if(InputManager.DefendKeyDown()){ unit.stateMachine.SetState(new UnitDefend()); return; }

            //jump
            if(InputManager.JumpKeyDown()){ unit.stateMachine.SetState(new PlayerJump()); return; }

            //use weapon
            if(unit.weapon && InputManager.PunchKeyDown()){ unit.stateMachine.SetState(new PlayerWeaponAttack()); return; }

            //check for nearby enemy to ground Punch
            if(InputManager.PunchKeyDown() && unit.NearbyEnemyDown()){ unit.stateMachine.SetState(new PlayerGroundPunch()); return; }

            //check for nearby enemy to ground kick
            if(InputManager.KickKeyDown() && unit.NearbyEnemyDown()){ unit.stateMachine.SetState(new PlayerGroundKick()); return; }

            //punch Key pressed
            if(InputManager.PunchKeyDown()){ unit.stateMachine.SetState(new PlayerAttack(ATTACKTYPE.PUNCH)); return; }

            //kick Key pressed
            if(InputManager.KickKeyDown()){ unit.stateMachine.SetState(new PlayerAttack(ATTACKTYPE.KICK)); return; }

            //grab something (enemy or item)
            if(InputManager.GrabKeyDown() && !unit.weapon){ unit.stateMachine.SetState(new PlayerTryGrab()); return; }

            //drop current weapon
            if(InputManager.GrabKeyDown() && unit.weapon){ unit.stateMachine.SetState(new UnitDropWeapon()); return; }
                
            //move
            if(InputManager.GetInputVector().magnitude > 0) unit.stateMachine.SetState(new PlayerMove());
        }
    }
}