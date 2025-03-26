using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RK
{
    /// <summary>
    /// ターゲットを補足し、追いかけている状態
    /// </summary>
    [CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // アクションを実行しているかチェック（実行している場合は、アクションが完了するまで何もしない）
            if (aiCharacter.isPerformingAction)
                return this;

            // ターゲットがNULLかどうかをチェックし、ターゲットがない場合はidle stateに戻る. 
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);
            // nabmeshAgentがアクティブになっているか確認する。
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;
            if (aiCharacter.aiCharacterCombatManager.enablePivot)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV
                    || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }

            //aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);
            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);
            // ターゲットとの戦闘範囲内にいる場合、戦闘態勢に切り替える
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
                return SwitchState(aiCharacter, aiCharacter.combatStance);

            // ターゲットが遠い場合は初期位置に戻る処理を書きたい


            // ターゲットの追跡 (navMeshAgentを用いる)
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
    }
}
