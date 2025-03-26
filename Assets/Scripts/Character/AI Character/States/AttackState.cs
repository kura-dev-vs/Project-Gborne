using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// アタックステートのscriptableobject
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "A.I/States/Attack")]
    public class AttackState : AIState
    {
        [Header("Current Attack")]
        [HideInInspector] public AICharacterAttackAction currentAttack;
        [HideInInspector] public bool willPerformCombo = false;
        [Header("State Flags")]
        protected bool hasPerformedAttack = false;
        protected bool hasPerformedCombo = false;
        [Header("Pivot After Attack")]
        [SerializeField] protected bool pivotAfterAttack = false;
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);
            if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // ターゲットに向かって回転しながら攻撃
            aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhilstAttacking(aiCharacter);
            // movement parametersを0に
            aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);
            // コンボを行う場合
            if (willPerformCombo && !hasPerformedCombo)
            {
                if (currentAttack.comboAction != null)
                {
                    // AICharacterがコンボを行える場合、アタックアクションに設定したアクションに派生する
                    //hasPerformedCombo = true;
                    //currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
                }
            }

            if (aiCharacter.isPerformingAction)
                return this;

            if (!hasPerformedAttack)
            {
                // actionRecoveryTimerがまだなら他の行動をする前にreturnしてTickをやり直す
                if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
                    return this;

                PerformAttack(aiCharacter);

                // コンボが可能な場合用に上に戻ってコンボアクションの処理をする
                return this;
            }
            if (pivotAfterAttack)
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            return SwitchState(aiCharacter, aiCharacter.combatStance);
        }
        protected void PerformAttack(AICharacterManager aICharacter)
        {
            hasPerformedAttack = true;
            currentAttack.AttemptToPerformAction(aICharacter);
            aICharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
        }
        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasPerformedAttack = false;
            hasPerformedCombo = false;
        }
    }
}
