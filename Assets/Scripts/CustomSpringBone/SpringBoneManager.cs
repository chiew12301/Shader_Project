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
        [SerializeField] private bool m_contanstUpdateSpringBoneSettings = true;
        [SerializeField] private float m_stiffness = 100.0f;
        [SerializeField, Range(0.0f, 0.9f)] private float m_damping = 5.0f;
        [SerializeField] private float m_bounciness = 40.0f;
        [SerializeField] private Vector3 m_customRotiation = Vector3.zero;
        [SerializeField] private bool m_useSpecifiedRotation = false;
        [SerializeField] private Vector3 m_springEnd = Vector3.left;
        //=======================================================================

        private void Start()
        {
            this.FindSpringBones();
            this.InitializeSpringBones();
        }

        private void Update()
        {

        }

        private void LateUpdate()
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

            if(this.m_springBones.Count <= 0) return;

            foreach (SpringBone bone in this.m_springBones)
            {
                bone.InitializeSpringBone(this.m_stiffness, this.m_damping, this.m_bounciness, this.m_customRotiation, this.m_springEnd, this.m_useSpecifiedRotation);
            }
        }

        private void UpdateSpringBones()
        {
            if(this.m_contanstUpdateSpringBoneSettings)
            {
                foreach (SpringBone bone in this.m_springBones)
                {
                    bone.InitializeSpringBone(this.m_stiffness, this.m_damping, this.m_bounciness, this.m_customRotiation, this.m_springEnd, this.m_useSpecifiedRotation);
                }
                return;
            }
        }

        //=======================================================================

    }
}