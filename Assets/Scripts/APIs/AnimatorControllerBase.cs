using UnityEngine;

namespace DefenceGameSystem.OS.API
{
    [DisallowMultipleComponent]
    public class AnimatorControllerBase : MonoBehaviour
    {
        public Animator animator => m_animator;
        
        protected AnimationClip this[string key]
        {
            get
            {
                return m_animatorOverrideController[key] ?? null;
            }
            set
            {
                m_animatorOverrideController[key] = value;
            }
        }

        protected Animator m_animator { get; private set; }
        private AnimatorOverrideController m_animatorOverrideController;

        protected virtual void Awake()
        {
            m_animator = GetComponent<Animator>();
            
            m_animatorOverrideController = new AnimatorOverrideController(m_animator.runtimeAnimatorController);
            m_animator.runtimeAnimatorController = m_animatorOverrideController;
        }
    }
}