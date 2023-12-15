using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATEMACHINE;

namespace STATEMACHINE_EXAMPLE
{
    [System.Serializable]
    public enum PLAYERSTATE
    {
        IDLE,
        WALK,
        RUN,
        CRINGE
    }

    public class PlayerStateMachine : StateManager<PLAYERSTATE>
    {
        protected override void Awake() {
            base.Awake();
            this.InitializeStateMachine();
        }

        protected override void InitializeStateMachine()
        {
            PlayerIdleState idleState = new PlayerIdleState(this);
            PlayerWalkState walkState = new PlayerWalkState(this);
            PlayerRunState runState = new PlayerRunState(this);

            this.m_statesDict.Add(idleState.StateKey, idleState);
            this.m_statesDict.Add(walkState.StateKey, walkState);
            this.m_statesDict.Add(runState.StateKey, runState);

            this.m_currentState = this.m_statesDict[PLAYERSTATE.IDLE];
        }

        [ContextMenu("Change to walk")]
        private void ChangeToWalk()
        {
            this.ChangeState(PLAYERSTATE.WALK);
        }

        [ContextMenu("Change to idle")]
        private void ChangeToIdle()
        {
            this.ChangeState(PLAYERSTATE.IDLE);
        }

        [ContextMenu("Change to run")]
        private void ChangeToRun()
        {
            this.ChangeState(PLAYERSTATE.RUN);
        }

        [ContextMenu("Change to Cringe")]
        private void ChangeToCringe()
        {
            this.ChangeState(PLAYERSTATE.CRINGE); //expected warning due to no initialization
        }
    }
}