using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIRECABLE
{
    [RequireComponent(typeof(Connector))]
    public class CableGrabHandler : MonoBehaviour
    {
        private Connector m_connector = null;

        //==================================================

        private void Awake() 
        {
            this.m_connector = this.gameObject.GetComponent<Connector>();
        }

        //==================================================


        //==================================================
    }
}