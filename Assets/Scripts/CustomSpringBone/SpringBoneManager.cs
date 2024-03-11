using System.Collections.Generic;
using UnityEngine;

namespace KC_2D
{
    public class SpringBoneManager : MonoBehaviour
    {
        [Tooltip("If the list contain bones, it will not look for bone anymore!")]
        [SerializeField] private List<SpringBone> m_springBones = new List<SpringBone>();

        [Header("Spring bone settings")]
        [SerializeField] private bool m_overrideSpringBoneSetting = false;
        [Header("Spring bone Collision Settings")]
        [SerializeField] private LayerMask m_collisionLayer;
        [SerializeField] private float m_minDistance = 0.1f;
        //=======================================================================

        private void Start()
        {
            this.FindSpringBones();
            this.InitializeSpringBones();
        }

        private void Update()
        {
            this.UpdateSpringBones();
        }

        //=======================================================================
        private void FindSpringBones()
        {
            if(this.m_springBones.Count > 0) return;

            SpringBone[] foundBones = FindObjectsOfType<SpringBone>();

            this.m_springBones.AddRange(foundBones);
        }

        private void InitializeSpringBones()
        {
            if(!this.m_overrideSpringBoneSetting) return;

            if(this.m_springBones.Count > 0) return;

            foreach (SpringBone bone in this.m_springBones)
            {
                bone.InitializeSpringBone(this.m_collisionLayer, this.m_minDistance);
            }
        }

        private void UpdateSpringBones()
        {
            foreach (SpringBone bone in this.m_springBones)
            {
                bone.UpdateSpringBone();
            }
        }

        //=======================================================================

    }
}