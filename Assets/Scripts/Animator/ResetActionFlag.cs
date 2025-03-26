using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// Animator Controller上のEmptyで使用するコンポ―ネント
    /// </summary>
    public class ResetActionFlag : StateMachineBehaviour
    {
        CharacterManager character;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                // PlayerManagerが親、Animatorが子オブジェクトにあるためInParent
                character = animator.GetComponentInParent<CharacterManager>();
            }
            if (character == null)
                return;
            // アクションが終了し、ステートが "EMPTY "に戻ったときに呼び出される。
            if (character.IsOwner)
            {
                character.isPerformingAction = false;
                character.characterAnimatorManager.applyRootMotion = false;
                character.characterLocomotionManager.canRotate = true;
                character.characterLocomotionManager.canMove = true;
                character.characterLocomotionManager.isRolling = false;
                character.characterAnimatorManager.DisableCanDoCombo();
                character.characterAnimatorManager.DisableCanDoRollingAttack();
                character.characterAnimatorManager.DisableCanDoBackstepAttack();

                character.canDodge = true;
                character.characterNetworkManager.isJumping.Value = false;
                character.characterNetworkManager.isInvulnerable.Value = false;
                character.characterNetworkManager.isAttacking.Value = false;
                character.characterNetworkManager.isParrying.Value = false;
                character.characterNetworkManager.isParryable.Value = false;
                character.characterNetworkManager.isRipostable.Value = false;
                character.characterNetworkManager.isBeingCriticallyDamaged.Value = false;
            }

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{

        //}

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