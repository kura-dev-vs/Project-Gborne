using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// PT関連のnetworkvariableとonvaluechanged、アクティブキャラの変更時やスキルアクションを行った時のサーバーへの通知などネットワーク関連を管理
    /// </summary>
    public class PlayableCharacterEntryNetworkManager : NetworkBehaviour
    {
        [HideInInspector] public EntryManager entry;
        [HideInInspector] public PlayerManager player;
        public NetworkVariable<bool> resetPTFire = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentPlayableCharacterID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkList<int> currentPTIDNetworkList = new NetworkList<int>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [HideInInspector] public int[] currentPTIDForSaveAndLoad = { 1, 2, 0, 0 };
        [HideInInspector] public int count = 0;
        bool pcChangeAllow = true;
        protected virtual void Awake()
        {
            entry = GetComponent<EntryManager>();
            player = GetComponent<PlayerManager>();
        }

        public void OnCurrentResetPTFireChange(bool oldStatus, bool newStatus)
        {
            if (oldStatus)
                return;

            if (player.IsOwner)
            {
                resetPTFire.Value = false;
            }
            else
            {
                pcChangeAllow = false;
            }
        }
        /// <summary>
        /// listのonvaluechangedはlist内の一つが変更されると呼び出される
        /// PTIDが変更時、 currentPTIDが4回呼び出されてから非オーナー側もPTをリフレッシュし、アクティブキャラの変更を許す
        /// </summary>
        /// <param name="changeEvent"></param>
        public void OnCurrentPTIDChanged(NetworkListEvent<int> changeEvent)
        {
            if (!player.IsOwner)
            {
                count++;
                if (count == 4)
                {
                    Debug.Log("Count: 4");
                    entry.playableCharacterInventoryManager.RefreshDeployedPT();
                    count = 0;
                    pcChangeAllow = true;
                }
            }
        }
        /// <summary>
        /// アクティブキャラのidが変わった場合を通知
        /// pt変更時には途中で呼び出されてしまうため、pcChangeAllowを用いてtrueの時のみ変更を許す
        /// </summary>
        /// <param name="oldID"></param>
        /// <param name="newID"></param>
        public void OnCurrentPlayableCharacterIDChange(int oldID, int newID)
        {
            if (player.IsOwner)
            {
                entry.playableCharacterEntryManager.ChangeCharacterFromID(newID);
            }
            else
            {
                var task = WaitChangeCheck(newID);
            }
        }

        /// <summary>
        /// pcChangeAllowがtrueでない場合はtrueになるまで待機し続ける (非同期処理)
        /// </summary>
        /// <param name="newID"></param>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <param name="newID"></param>
        /// <returns></returns>
        private async Task WaitChangeCheck(int newID)
        {
            while (!pcChangeAllow)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            entry.playableCharacterEntryManager.ChangeCharacterFromID(newID);
        }

        /// <summary>
        /// skillAction関連サーバーに通知、クライアントに通知、それを通って非オーナー側が同様のskillActionを行う
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="skillActionID"></param>
        /// <param name="skillID"></param>
        /// <param name="direction"></param>
        [ServerRpc]
        public void NotifyTheServerOfSkillActionServerRpc(ulong clientID, int skillActionID, int skillID, string direction)
        {
            if (IsServer)
            {
                NotifyTheServerOfSkillActionClientRpc(clientID, skillActionID, skillID, direction);
            }
        }
        [ClientRpc]
        private void NotifyTheServerOfSkillActionClientRpc(ulong clientID, int skillActionID, int skillID, string direction)
        {
            // owner側ではアクションを実行しているのでそれ以外で行わせる
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                if (direction == "left")
                {
                    PerformPCBasedSkillAction(skillActionID, skillID, player.leftSkillManager);
                }
                else
                {
                    PerformPCBasedSkillAction(skillActionID, skillID, player.rightSkillManager);
                }
            }
        }
        private void PerformPCBasedSkillAction(int skillActionID, int skillID, PlayerSkillManager pcSkill)
        {
            PCSkillAction pcSkillAction = WorldActionManager.instance.GetPCSkillActionByID(skillActionID);
            if (pcSkillAction != null)
            {
                pcSkillAction.AttemptToPerformSkill(player, pcSkill);
            }
            else
            {
                Debug.LogError("ACTION IS NULL, CANNOT BE PERFORMED");
            }
        }


        /// <summary>
        /// burstAction関連サーバーに通知、クライアントに通知、それを通って非オーナー側が同様のburstActionを行う
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="burstActionID"></param>
        /// <param name="burstID"></param>
        [ServerRpc]
        public void NotifyTheServerOfBurstActionServerRpc(ulong clientID, int burstActionID, int burstID)
        {
            if (IsServer)
            {
                NotifyTheServerOfBurstActionClientRpc(clientID, burstActionID, burstID);
            }
        }
        [ClientRpc]
        private void NotifyTheServerOfBurstActionClientRpc(ulong clientID, int burstActionID, int burstID)
        {
            // owner側ではアクションを実行しているのでそれ以外で行わせる
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformPCBasedBurstAction(burstActionID, burstID, player.playerBurstManager);
            }
        }
        private void PerformPCBasedBurstAction(int burstActionID, int burstID, PlayerBurstManager pcBurst)
        {
            PCBurstAction pcBurstAction = WorldActionManager.instance.GetPCBurstActionByID(burstActionID);
            if (pcBurstAction != null)
            {
                pcBurstAction.AttemptToPerformBurst(player, pcBurst);
            }
            else
            {
                Debug.LogError("ACTION IS NULL, CANNOT BE PERFORMED");
            }
        }
    }
}
