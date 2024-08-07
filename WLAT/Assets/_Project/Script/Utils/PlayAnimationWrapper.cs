using UnityEngine;

namespace Utils
{
    public class PlayAnimationWrapper : MonoBehaviour
    {
        public Animator animator;
        public string animationName;

        public void Play()
        {
            animator.Play(animationName);
        }
    }
}
