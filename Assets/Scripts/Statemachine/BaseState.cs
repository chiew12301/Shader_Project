using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STATEMACHINE
{
    [System.Serializable]
    public abstract class BaseState<EState> where EState : Enum
    {
        public EState StateKey {get; private set;}
        protected StateManager<EState> m_mySM;

        public BaseState(EState key, StateManager<EState> sm){
            this.StateKey = key;
            this.m_mySM = sm;
        }

        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
        public abstract EState GetNextState();
        public abstract void OnTriggerEnter(Collider other);
        public abstract void OnTriggerStay(Collider other);
        public abstract void OnTriggerExit(Collider other);
        public abstract void OnCollisionEnter(Collision other);
        public abstract void OnCollisionStay(Collision other);
        public abstract void OnCollisionExit(Collision other);
    }    
}