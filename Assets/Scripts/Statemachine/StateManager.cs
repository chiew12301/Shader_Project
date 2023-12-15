using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STATEMACHINE
{
    public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> m_statesDict = new Dictionary<EState, BaseState<EState>>();

        protected BaseState<EState> m_currentState;

        protected bool m_isTransitioningToNextState = false;

        protected virtual void Awake() {}

        protected virtual void Start()
        {
            this.m_currentState.EnterState();
        }

        protected virtual void Update()
        {
            if(!this.m_isTransitioningToNextState)
            {
                this.m_currentState.UpdateState();
            }
        }

        public void ChangeState(EState stateKey)
        {
            if(!this.m_statesDict.ContainsKey(stateKey)) {Debug.LogWarning($"Key Not Exist {stateKey}"); return;}

            this.TransitionToNextState(stateKey);
        }


        protected virtual void TransitionToNextState(EState stateKey)
        {
            this.m_isTransitioningToNextState = true;
            this.m_currentState.ExitState();
            this.m_currentState = this.m_statesDict[stateKey];
            this.m_currentState.EnterState();
            this.m_isTransitioningToNextState = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            this.m_currentState.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other) 
        {
            this.m_currentState.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other) 
        {
            this.m_currentState.OnTriggerExit(other);
        }

        private void OnCollisionEnter(Collision other) 
        {
            this.m_currentState.OnCollisionEnter(other);
        }

        private void OnCollisionStay(Collision other) 
        {
            this.m_currentState.OnCollisionStay(other);
        }

        private void OnCollisionExit(Collision other) 
        {
            this.m_currentState.OnCollisionExit(other);
        }

        protected abstract void InitializeStateMachine();
    }
}