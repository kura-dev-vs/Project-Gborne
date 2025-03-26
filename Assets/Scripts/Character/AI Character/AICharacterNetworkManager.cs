using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// aicharacter用のnetworkmanager
    /// bosscharacter等固有のonvaluechanged用に必要なので削除禁止
    /// </summary>
    public class AICharacterNetworkManager : CharacterNetworkManager
    {
        //public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        AICharacterManager aICharacter;
        protected override void Awake()
        {
            base.Awake();

            aICharacter = GetComponent<AICharacterManager>();
        }

        public override void OnIsDeadChanged(bool oldStatus, bool newStatus)
        {
            base.OnIsDeadChanged(oldStatus, newStatus);

            aICharacter.aICharacterInventoryManager.DropItem();
        }
    }
}
