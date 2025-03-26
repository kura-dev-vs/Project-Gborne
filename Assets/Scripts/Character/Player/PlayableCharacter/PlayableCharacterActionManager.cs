using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace RK
{
    /// <summary>
    /// スキルやバーストのアクションを受け取ったら可能か判定し、可能であればアクションを始めサーバーへアクションid等を通知させる
    /// </summary>
    public class PlayableCharacterActionManager : NetworkBehaviour
    {
        EntryManager entry;
        private void Awake()
        {
            entry = GetComponent<EntryManager>();
        }
        public void PerformPCBasedSkillAction(PCSkillAction skillAction, PlayerSkillManager pcSkill, string direction)
        {
            if (skillAction == null)
            {
                Debug.Log("DON'T SET SKILL ACTION");
                return;
            }
            // owner側がアクションを起こす
            skillAction.AttemptToPerformSkill(entry.player, pcSkill);

            // サーバーにアクションを実行したことを通知
            entry.playableCharacterEntryNetworkManager.NotifyTheServerOfSkillActionServerRpc(NetworkManager.Singleton.LocalClientId, skillAction.skillActionID, pcSkill.skill.skillID, direction);
        }

        public void PerformPCBasedBurstAction(PCBurstAction burstAction, PlayerBurstManager pcBurst)
        {
            if (burstAction == null)
            {
                Debug.Log("DON'T SET Burst ACTION");
                return;
            }
            // owner側がアクションを起こす
            burstAction.AttemptToPerformBurst(entry.player, pcBurst);

            // サーバーにアクションを実行したことを通知
            entry.playableCharacterEntryNetworkManager.NotifyTheServerOfBurstActionServerRpc(NetworkManager.Singleton.LocalClientId, burstAction.burstActionID, pcBurst.burst.burstID);
        }
    }
}
