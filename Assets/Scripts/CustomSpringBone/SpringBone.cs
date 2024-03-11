using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

namespace KC_2D
{
    public class SpringBone : MonoBehaviour
    {
        [SerializeField] private Transform m_target = null;
        [SerializeField] private float m_springConstant = 100.0f;
        [SerializeField] private float m_damping = 5.0f;

        [Tooltip("Please do not set to false if you wish manager to control the bone, this settings is for customasation purposes.")]
        [SerializeField] private bool m_allowOverride = true;

        [Header("Collision")]
        [SerializeField] private LayerMask m_collisionLayer;
        [SerializeField] private float m_minDistance = 0.1f;
        [Header("Constrain")]
        [SerializeField] private Vector3 m_minBounds = Vector3.zero;
        [SerializeField] private Vector3 m_maxBounds = Vector3.zero;
        [SerializeField] private bool m_constrainXRotation = false;
        [SerializeField] private bool m_constrainYRotation = false;
        [SerializeField] private bool m_constrainZRotation = false; 
        private Vector3 m_velocity = Vector3.zero;

        //=======================================================================

        //=======================================================================

        public void InitializeSpringBone(LayerMask layerMask, float detectionDistance)
        {
            if(!this.m_allowOverride) return;

            this.m_collisionLayer = layerMask;
            this.m_minDistance = detectionDistance;
        }

        public void UpdateSpringBone()
        {
            if(this.m_target == null)
            {
                Debug.LogWarning("[SPRINGBONE]: The target is empty!");
                return;
            }   

            Vector3 displacement = this.m_target.position - this.transform.position;

            Vector3 springForce = this.m_springConstant * displacement;

            Vector3 dampingForce = -this.m_damping * this.m_velocity;

            Vector3 totalForce = springForce + dampingForce;

            this.ApplyForce(totalForce);

            this.m_velocity += totalForce * Time.deltaTime;

            this.transform.position += this.m_velocity * Time.deltaTime;

            this.HandleCollisionAndConstraints();
        }

        private void ApplyForce(Vector3 force)
        {
            this.transform.position += force * Time.deltaTime;
        }

        private void HandleCollisionAndConstraints()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, this.m_target.position - transform.position,
                                                Vector3.Distance(transform.position, this.m_target.position), this.m_collisionLayer);
            if (hit.collider != null)
            {
                Vector3 convertHitPointToVector3 = new Vector3(hit.point.x, hit.point.y, 0.0f);
                Vector3 awayFromObstacle = (this.transform.position - convertHitPointToVector3).normalized;
                this.transform.position = convertHitPointToVector3 + awayFromObstacle * this.m_minDistance;
            }

            this.transform.position = new Vector3(
                Mathf.Clamp(this.transform.position.x, this.m_minBounds.x, this.m_maxBounds.x),
                Mathf.Clamp(this.transform.position.y, this.m_minBounds.x, this.m_maxBounds.x),
                Mathf.Clamp(this.transform.position.z, this.m_minBounds.x, this.m_maxBounds.x)
            );

            if (this.m_constrainXRotation)
                this.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
            if (this.m_constrainYRotation)
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, 0f, this.transform.rotation.eulerAngles.z);
            if (this.m_constrainZRotation)
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, 0f);
        }

        //=======================================================================
    }
}