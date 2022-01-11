using System.Collections;

using UnityEngine;

namespace DefenceGameSystem.OS.API
{
    public class UnitAnimatorController : AnimatorControllerBase
    {
        protected readonly static string KEY_READY = "ready";
        protected readonly static string KEY_GENERATE = "generate";
        protected readonly static string KEY_IDLE = "idle";
        protected readonly static string KEY_MOVE = "move";
        protected readonly static string KEY_DIE = "die";
        protected readonly static string KEY_ACTION = "action";

        public bool isRestricted => m_isRestricted;

        public AnimationClip readyClip;
        public AnimationClip generateClip;
        public AnimationClip idleClip;
        public AnimationClip moveClip;
        public AnimationClip dieClip;

        private bool m_isRestricted = false;

        private IEnumerator m_actionEnumerator;

        protected override void Awake()
        {
            base.Awake();

            this[KEY_READY] = readyClip;
            this[KEY_GENERATE] = generateClip;
            this[KEY_IDLE] = idleClip;
            this[KEY_MOVE] = moveClip;
            this[KEY_DIE] = dieClip;
        }

        public void Act(AnimationClip actionClip)
        {
            if(m_actionEnumerator == null)
            {
                m_actionEnumerator = m_i_Act(actionClip);

                StartCoroutine(m_actionEnumerator);
            }
            else if (!m_isRestricted)
            {
                StopCoroutine(m_actionEnumerator);

                m_actionEnumerator = m_i_Act(actionClip);

                StartCoroutine(m_actionEnumerator);
            }
            else
            {
                // 아무 일도 발생하지 않음.
            }
        }

        private IEnumerator m_i_Act(AnimationClip actionClip)
        {
            this[KEY_ACTION] = actionClip;

            m_isRestricted = true;

            yield return new WaitUntil(
                () =>
                {
                    AnimatorStateInfo stateInfo;

                    stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.normalizedTime < 1.0f;
                }
            );

            m_isRestricted = false;

            m_actionEnumerator = null;
        }
    }
}