using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// idleステート
    /// ターゲットを探し続け、見つかったらpurshueStateに移行
    /// </summary>
    [CreateAssetMenu(menuName = "A.I/States/Idle")]
    public class IdleState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.characterCombatManager.currentTarget != null)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            }
            else
            {
                // return this state, to continually search for a target (keep the state here, until a target is found)
                aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
                return this;
            }
        }
    }
}
