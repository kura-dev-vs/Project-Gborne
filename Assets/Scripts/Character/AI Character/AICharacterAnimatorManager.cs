using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// aicharacter用のanimatormanager
    /// MonoBehaviourのOnAnimatorMoveをaicharacter用に調整する
    /// </summary>
    public class AICharacterAnimatorManager : CharacterAnimatorManager
    {
        AICharacterManager aiCharacter;
        protected override void Awake()
        {
            base.Awake();
            aiCharacter = GetComponent<AICharacterManager>();
        }
        private void OnAnimatorMove()
        {
            if (!aiCharacter.IsOwner)
            {
                if (!aiCharacter.aiCharacterNetworkManager.isGrounded.Value)
                    return;
                Vector3 velocity = aiCharacter.animator.deltaPosition;

                aiCharacter.characterController.Move(velocity);
                aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
            }
            else
            {
                if (!aiCharacter.aiCharacterNetworkManager.isGrounded.Value)
                    return;
                Vector3 velocity = aiCharacter.animator.deltaPosition;

                aiCharacter.characterController.Move(velocity);
                aiCharacter.transform.position = Vector3.SmoothDamp(transform.position,
                    aiCharacter.characterNetworkManager.networkPosition.Value,
                    ref aiCharacter.characterNetworkManager.networkPositionVelocity,
                    aiCharacter.characterNetworkManager.networkPositionSmoothTime);
                aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;

            }
        }
    }
}
