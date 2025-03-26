using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// Attack Aniamationで使用する用のコンポ―ネント
    /// 今は攻撃ステートから抜けた際にchaseを切るために使用
    /// 
    /// </summary> <summary>
    /// 
    /// </summary>
    public class ResetAttackFlag : StateMachineBehaviour
    {
        CharacterManager character;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                // PlayerManagerが親、Animatorが子オブジェクトにあるためInParent
                character = animator.GetComponentInParent<CharacterManager>();
            }

            // 攻撃のチェイスを停止
            character.characterCombatManager.isChase = false;

            // キャラクターが持つダメージコライダーをすべて閉じる
            DamageCollider[] damageColliders = character.GetComponentsInChildren<DamageCollider>();
            foreach (var collider in damageColliders)
            {
                Debug.Log("tojiru");
                collider.DisableDamageCollider();
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
