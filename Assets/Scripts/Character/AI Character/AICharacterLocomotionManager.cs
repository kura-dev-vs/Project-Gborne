using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 回頭用に用意したがcombatmanagerに同じものを書いたので現在は使わない
    /// </summary>
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        /*
        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
        */
    }
}
