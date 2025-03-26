using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// AICharacterのアタックアクションの雛型
    /// Animator Controller上のアタックアニメーションをstringで指定し、直接行動させる
    /// ターゲットとの角度、距離が合致したものをweightによって選択する
    /// </summary>
    [CreateAssetMenu(menuName = "A.I/Actions/Attack")]
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("Attack")]
        [SerializeField] private string attackAnimation;
        [SerializeField] bool isParryable = true;
        [Header("Combo Action")]
        public AICharacterAttackAction comboAction; // 現在のアクションから派生するコンボアクション
        [Header("Action Values")]
        [SerializeField] AttackType attackType;
        public int attackWeight = 50;
        public float actionRecoveryTime = 1.5f; // この攻撃を行った後、キャラクターが次の攻撃を行えるようになるまでの時間
        public float minimumAttackAngle = -35;
        public float maximumAttackAngle = 35;
        public float minimumAttackDistance = 0;
        public float maximumAttackDistance = 2;
        public void AttemptToPerformAction(AICharacterManager aICharacter)
        {
            // aiキャラクターの攻撃方式をどうするか再考すべきか

            // aICharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, this);
            aICharacter.characterAnimatorManager.PlayTargetActionAnimation(attackAnimation, this);
            aICharacter.aiCharacterNetworkManager.isParryable.Value = isParryable;
        }
    }
}
