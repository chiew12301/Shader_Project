using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATEMACHINE;

namespace STATEMACHINE_EXAMPLE
{
    public class PlayerIdleState : BaseState<PLAYERSTATE>
    {
        public PlayerIdleState(StateManager<PLAYERSTATE> sm) : base(PLAYERSTATE.IDLE,sm){}

        public override void EnterState()
        {
            Debug.Log("Entered IDLE");
            this.m_mySM.ChangeState(PLAYERSTATE.RUN);
        }

        public override void ExitState()
        {
            Debug.Log("Exited IDLE");
        }

        public override PLAYERSTATE GetNextState()
        {
            return PLAYERSTATE.WALK;
        }

        public override void OnCollisionEnter(Collision other)
        {

        }

        public override void OnCollisionExit(Collision other)
        {

        }

        public override void OnCollisionStay(Collision other)
        {

        }

        public override void OnTriggerEnter(Collider other)
        {

        }

        public override void OnTriggerExit(Collider other)
        {

        }

        public override void OnTriggerStay(Collider other)
        {

        }

        public override void UpdateState()
        {
            Debug.Log("UPDATE IDLE");
        }
    }
}