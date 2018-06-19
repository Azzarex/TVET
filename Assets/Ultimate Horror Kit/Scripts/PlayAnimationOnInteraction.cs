using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxlPlay
{
    public class PlayAnimationOnInteraction : MonoBehaviour
    {

        public string OnAnim = "Open";
        public string OffAnim = "Close";

        private Animation animationC;
        private Animator animator;
        private bool opened;
        private void Awake()
        {
            animationC = GetComponent<Animation>();
            animator = GetComponent<Animator>();

        }
        void Interaction()
        {
            if (!opened)
                animator.Play(OnAnim);
            else
                animator.Play(OffAnim);
            opened = !opened;
            Debug.Log("interaction ");
        }
    }
}