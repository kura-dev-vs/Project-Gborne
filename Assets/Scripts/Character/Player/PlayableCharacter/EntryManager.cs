using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using RK;
using UnityEngine.InputSystem;
using System;

namespace RK
{
    /// <summary>
    /// 各playablecharactermanagerの管理に必要。基本playerからここを通って各componentへアクセスさせる
    /// </summary>
    public class EntryManager : NetworkBehaviour
    {
        [HideInInspector] public PlayerManager player;
        [HideInInspector] public PlayableCharacterEntryNetworkManager playableCharacterEntryNetworkManager;
        [HideInInspector] public PlayableCharacterEntryManager playableCharacterEntryManager;
        [HideInInspector] public PlayableCharacterInventoryManager playableCharacterInventoryManager;
        [HideInInspector] public PlayableCharacterActionManager playableCharacterActionManager;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
            player = GetComponent<PlayerManager>();
            playableCharacterEntryNetworkManager = GetComponent<PlayableCharacterEntryNetworkManager>();
            playableCharacterEntryManager = GetComponent<PlayableCharacterEntryManager>();
            playableCharacterInventoryManager = GetComponent<PlayableCharacterInventoryManager>();
            playableCharacterActionManager = GetComponent<PlayableCharacterActionManager>();
        }
        protected virtual void Update()
        {
            if (!IsOwner)
                return;
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                PlayerInputManager.instance.entry = this;
            }
            playableCharacterEntryNetworkManager.currentPlayableCharacterID.OnValueChanged += playableCharacterEntryNetworkManager.OnCurrentPlayableCharacterIDChange;
            playableCharacterEntryNetworkManager.resetPTFire.OnValueChanged += playableCharacterEntryNetworkManager.OnCurrentResetPTFireChange;
            playableCharacterEntryNetworkManager.currentPTIDNetworkList.OnListChanged += playableCharacterEntryNetworkManager.OnCurrentPTIDChanged;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            playableCharacterEntryNetworkManager.currentPlayableCharacterID.OnValueChanged -= playableCharacterEntryNetworkManager.OnCurrentPlayableCharacterIDChange;
            playableCharacterEntryNetworkManager.resetPTFire.OnValueChanged -= playableCharacterEntryNetworkManager.OnCurrentResetPTFireChange;
            playableCharacterEntryNetworkManager.currentPTIDNetworkList.OnListChanged -= playableCharacterEntryNetworkManager.OnCurrentPTIDChanged;
        }
    }
}
