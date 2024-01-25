using UnityEngine;

namespace KC_Custom
{
    public abstract class AAnimatorMenu<T> : AMenu<T> where T : AAnimatorMenu<T>
    {
        [Header("Animator")]
        [SerializeField] protected Animator m_animator;

        public static Animator ANIMATOR { get { return GetInstance().m_animator; } }
    }
}