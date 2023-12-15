using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATEMACHINE;

namespace STATEMACHINE_EXAMPLE
{
    public class PlayerWalkState : BaseState<PLAYERSTATE>
    {
        public PlayerWalkState(StateManager<PLAYERSTATE> sm) : base(PLAYERSTATE.WALK, sm){}

        public override void EnterState()
        {
            Debug.Log("Entered WALK");
        }

        public override void ExitState()
        {
            Debug.Log("Exited WALK");
        }

        public override PLAYERSTATE GetNextState()
        {
            return PLAYERSTATE.RUN;
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
            Debug.Log("UPDATE w");
        }
    }
}