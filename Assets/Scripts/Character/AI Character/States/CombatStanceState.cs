using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RK
{
    /// <summary>
    /// 戦闘ステート
    /// </summary>
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
    public class CombatStanceState : AIState
    {
        // 1. キャラクターに対するターゲットの距離と角度に応じて、攻撃状態を選択
        // 2. 攻撃を待つ間、戦闘ロジックを処理（ブロッキング、ストレフィング、ドッジなど）
        // 3.ターゲットが戦闘範囲外に移動したら、ターゲットを追跡するように切り替える
        // 4. ターゲットがいなくなったら、アイドル状態に切り替える
        // どこかのタイミングでターゲットを切り替える処理を書く予定 (攻撃後とか?)
        [Header("Attacks")]
        public List<AICharacterAttackAction> aiCharacterAttacks;    // このキャラクターが可能なアタックアクションの全て. inspectorで設定
        public List<AICharacterAttackAction> potentialAttacks;   // 上のリストから角度, 距離に応じて現在可能な攻撃のリスト. 逐次変更されていく
        public AICharacterAttackAction choosenAttack;
        public AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;   // 次に行う攻撃が選択されていた場合true
        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false;    // キャラクターがコンボアクションを行えるかどうか. アタックアクションによって変化
        [SerializeField] protected int chanceToPerformCombo = 25;   // コンボが可能な場合, コンボを行う確率 (%)
        [SerializeField] bool hasRolledForComboChance = false;      // コンボを狙っているか

        [Header("Engagement Distance")]
        [SerializeField] public float maximumEngagementDistance = 5; // ターゲットとの距離がこの距離より大きいときpursue target stateに移行する
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;
            // ターゲットが前方+-30°の範囲からいなくなった場合向き直す
            if (aiCharacter.aiCharacterCombatManager.enablePivot)
            {
                if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
                {
                    if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                        aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }

            // 上記の範囲内にいる場合navmeshagentの角速度で正面に捉えさせる
            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);
            // hasAttackがfalseの場合、次に行う攻撃を選ぶ
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = choosenAttack;
                // コンボを狙うか
                //hasRolledForComboChance = RollForOutcomeChance(chanceToPerformCombo);

                return SwitchState(aiCharacter, aiCharacter.attack);
            }
            // maximumEngagementDistanceの距離より遠かったらpursueTargetStateに移行
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
        /// <summary>
        /// 距離と角度からpotentialAttacksに可能な攻撃をリストし、そこからweightに応じてランダムに選択
        /// </summary>
        /// <param name="aiCharacter"></param>
        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();
            foreach (var potentialAttack in aiCharacterAttacks)
            {
                if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;
                potentialAttacks.Add(potentialAttack);
            }
            if (potentialAttacks.Count <= 0)
                return;
            var totalWeight = 0;
            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }
            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;
            foreach (var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight;
                if (randomWeightValue <= processedWeight)
                {
                    choosenAttack = attack;
                    previousAttack = choosenAttack;
                    hasAttack = true;
                    return;
                }
            }
        }
        /// <summary>
        /// コンボが可能な場合ランダムに行うか判定する
        /// </summary>
        /// <param name="outcomeChance"></param>
        /// <returns></returns>
        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            bool outcomeWillBePerformed = false;
            int randomPercentage = Random.Range(0, 100);
            if (randomPercentage < outcomeChance)
                outcomeWillBePerformed = true;
            return outcomeWillBePerformed;
        }
        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);
            hasAttack = false;
            hasRolledForComboChance = false;
        }

    }
}
