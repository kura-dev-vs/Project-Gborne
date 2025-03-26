using System.Collections;
using System.Collections.Generic;
using Mebiustos.MMD4MecanimFaciem;
using UnityEngine;

namespace RK
{
    public class UI3DIdleFlag : StateMachineBehaviour
    {
        UI3DManager manager;
        BlendShapeBlink blink;
        FaciemController faciemController;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            manager = animator.GetComponentInParent<UI3DManager>();
            if (manager != null)
            {
                manager.idleAnimation = true;
            }
            blink = animator.GetComponentInParent<BlendShapeBlink>();
            if (blink != null)
            {
                blink.canBlink = true;
            }
            faciemController = animator.GetComponentInParent<FaciemController>();
            if (faciemController != null)
            {
                faciemController.SetFace("Default");
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (manager != null)
            {
                manager.idleAnimation = false;
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
