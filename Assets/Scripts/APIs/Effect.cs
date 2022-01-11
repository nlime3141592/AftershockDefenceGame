using System.Collections;

using UnityEngine;

namespace DefenceGameSystem.OS.API
{
    public class Effect : MonoBehaviour
    {
        public AnimationClip clip;
        private Animator animator;
        private AnimatorOverrideController overrides;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            overrides = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrides;

            overrides["effect"] = clip;
        }

        private void Start()
        {
            StartCoroutine(starter());
        }

        private IEnumerator starter()
        {
            animator.Play("effect");
            yield return new WaitForSeconds(3.0f);
            Destroy(this.gameObject);
        }
    }
}