using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UIElements;

namespace KC_2D
{
    public class SpringBone : MonoBehaviour
    {
        [SerializeField] private float m_stiffness = 1.0f;
        [SerializeField, Range(0.0f, 0.9f)] private float m_damping = 0.1f;
        [SerializeField] private float m_bounciness = 40.0f;
        [SerializeField] private Vector3 m_customRotiation = Vector3.zero;
        [SerializeField] private bool m_useSpecifiedRotation = false;
        [SerializeField] private Vector3 m_springEnd = Vector3.left;

        private float m_springLength => this.m_springEnd.magnitude;
        private Vector3 m_currentTipPosition = Vector3.zero;
        private SpringBone m_parBone = null;
        private bool m_updated = false;

        [Tooltip("Please do not set to false if you wish manager to control the bone, this settings is for customasation purposes.")]
        [SerializeField] private bool m_allowOverride = true;
        private Vector3 m_velocity = Vector3.zero;

        //=======================================================================

        private void Start()
        {
            this.m_currentTipPosition = this.transform.TransformPoint(this.m_springEnd);

            if(this.transform.parent != null)
            {
                this.m_parBone = this.transform.parent.GetComponentInParent<SpringBone>();
            }
        }

        private void Update()
        {
            this.m_updated = false;
        }

        private void LateUpdate()
        {
            this.UpdateSpringBone();
        }

        //=======================================================================

        public void InitializeSpringBone(float stiffness, float damping, float bounciness, Vector3 specRotate, Vector3 springEnd, bool useSpecifcRotatiion = false)
        {
            if(!this.m_allowOverride) return;
            this.m_stiffness = stiffness;
            this.m_damping = damping;
            this.m_bounciness = bounciness;
            this.m_customRotiation = specRotate;
            this.m_springEnd = springEnd;
            this.m_useSpecifiedRotation = useSpecifcRotatiion;
        }

        public void UpdateSpringBone()
        {
            if(this.m_updated)
            {
                return;
            }

            if (this.m_parBone != null)
            {
                this.m_parBone.UpdateSpringBone();
            }

            this.m_updated = true;

            var lastFrameTip = this.m_currentTipPosition;

            if(this.m_useSpecifiedRotation)
            {
                this.transform.localRotation = Quaternion.Euler(this.m_customRotiation);
            }

            this.m_currentTipPosition = this.transform.TransformPoint(this.m_springEnd);

            var force = this.m_bounciness * (this.m_currentTipPosition - lastFrameTip);

            force += this.m_stiffness * (this.m_currentTipPosition - this.transform.position).normalized;

            force -= this.m_damping * this.m_velocity;

            this.m_velocity = this.m_velocity + force * Time.deltaTime;

            this.m_currentTipPosition = lastFrameTip + this.m_velocity * Time.deltaTime;

            this.m_currentTipPosition = this.m_springLength * (this.m_currentTipPosition - this.transform.position).normalized + this.transform.position;

            this.transform.rotation = Quaternion.FromToRotation(this.transform.TransformDirection(this.m_springEnd), (this.m_currentTipPosition - this.transform.position).normalized) * this.transform.rotation;
        }

        //=======================================================================
    }
}