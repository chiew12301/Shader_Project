using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATEMACHINE;

namespace STATEMACHINE_EXAMPLE
{
    public class PlayerRunState : BaseState<PLAYERSTATE>
    {
        public PlayerRunState(StateManager<PLAYERSTATE> sm) : base(PLAYERSTATE.RUN, sm){}

        public override void EnterState()
        {
            Debug.Log("Entered RUN");
        }

        public override void ExitState()
        {
            Debug.Log("Exited RUN");
        }

        public override PLAYERSTATE GetNextState()
        {
            return PLAYERSTATE.IDLE;
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
            Debug.Log("UPDATE r");
        }
    }
}