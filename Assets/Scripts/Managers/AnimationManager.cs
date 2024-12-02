    using UnityEngine;

    public class AnimationManager
    {
        private Animator animator;

        public AnimationManager(Animator animator)
        {
            this.animator = animator;
        }

        // Play a single trigger animation
        public void PlayTrigger(string triggerName)
        {
            animator?.SetTrigger(triggerName);
        }

        // Set a boolean parameter
        public void SetBool(string parameterName, bool value)
        {
            animator?.SetBool(parameterName, value);
        }

        // Set an integer parameter
        public void SetInteger(string parameterName, int value)
        {
            animator?.SetInteger(parameterName, value);
        }

        // Reset a trigger
        public void ResetTrigger(string triggerName)
        {
            animator?.ResetTrigger(triggerName);
        }

        public void SetActionState(float actionState)
        {
            animator?.SetFloat("ActionState", actionState);
        }

        // Play parry reaction using Blend Tree
        public void SetParryIndex(int parryIndex)
        {
            SetInteger("ParryIndex", parryIndex);
        }

        public void TriggerParryReaction()
        {
            SetActionState(3);
            animator.SetTrigger("ParrySuccess");
        }

        // Play defense hit animation
        public void PlayDefenseHit()
        {
            PlayTrigger("DefenseHit");
        }

        public void TriggerDefenseStart()
        {
            SetActionState(0);
            animator.SetTrigger("DefenseStart");
        }

        // Play defense loop
        public void TriggerDefenseLoop()
        {
            SetActionState(1);
        }

    // Stop defense loop
        public void StopDefense()
        {
            SetActionState(2);
            ResetTrigger("DefenseLoop");
            SetBool("IsDefending", false);
        }

    }
