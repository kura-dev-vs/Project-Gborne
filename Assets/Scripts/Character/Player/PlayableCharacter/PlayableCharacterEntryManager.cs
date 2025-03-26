using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RK
{
    /// <summary>
    /// playablecharacterのアクティブキャラやPT管理に使用する
    /// </summary> <summary>
    /// 
    /// </summary>
    public class PlayableCharacterEntryManager : MonoBehaviour
    {
        PlayerManager player;
        EntryManager entry;
        public PlayableCharacterManager playableCharacterManager;
        public GameObject playableCharacterModel;
        protected void Awake()
        {
            player = GetComponent<PlayerManager>();
            entry = GetComponent<EntryManager>();
        }
        protected void Start()
        {
            if (player.IsOwner)
            {
                SetCurrentPTIDList();
            }
            else
            {
                if (entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList.Count == WorldPlayableCharacterDatabase.MaxPTCount)
                {
                    for (int i = 0; i < WorldPlayableCharacterDatabase.MaxPTCount; i++)
                    {
                        entry.playableCharacterInventoryManager.currentPCPT[i] = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList[i]);
                    }
                }
            }

            SetFirstCharacter();
        }
        /// <summary>
        /// アクティブキャラの変更時に呼び出される
        /// </summary>
        /// <param name="newManager"></param>
        /// <param name="newCharacterModel"></param> 
        private void LoadPlayableCharacter(PlayableCharacterManager newManager, GameObject newCharacterModel)
        {
            if (entry.playableCharacterInventoryManager.currentCharacter != null)
            {
                playableCharacterManager = newManager;
                playableCharacterModel = newCharacterModel;
                entry.playableCharacterInventoryManager.PositioningCharacterModel(playableCharacterModel);
            }
        }

        /// <summary>
        /// PT出現、変更時に呼び出される
        /// PT配列の中から一番最初に出てきたキャラクターをアクティブキャラにセットし、それ以外はinstantiateしてから各種コンポ―ネントをfalseにする
        /// </summary>
        public void SetFirstCharacter()
        {
            float characterCount = 0;
            PlayableCharacter playableCharacter;
            GameObject loadNewCharacter;
            PlayableCharacterManager loadNewCharacterManager;

            for (int i = 0; i < WorldPlayableCharacterDatabase.MaxPTCount; i++)
            {
                if (entry.playableCharacterInventoryManager.currentPCPT[i].pcID != WorldPlayableCharacterDatabase.instance.NoCharacter.pcID)
                {
                    playableCharacter = entry.playableCharacterInventoryManager.currentPCPT[i];
                    loadNewCharacter = Instantiate(playableCharacter.characterModel);
                    entry.playableCharacterInventoryManager.PositioningCharacterModel(loadNewCharacter);
                    loadNewCharacterManager = loadNewCharacter.GetComponent<PlayableCharacterManager>();
                    loadNewCharacterManager.SetCharacterInformation(entry, playableCharacter);

                    if (characterCount == 0)
                    {
                        entry.playableCharacterInventoryManager.currentCharacterIndex = i;
                        entry.playableCharacterInventoryManager.currentCharacter = playableCharacter;
                        playableCharacterModel = loadNewCharacter;
                        playableCharacterManager = loadNewCharacterManager;
                        if (entry.IsOwner)
                        {
                            entry.playableCharacterEntryNetworkManager.currentPlayableCharacterID.Value = playableCharacter.pcID;
                            PlayerUIManager.instance.playerUIHudManager.SetCharacterSlotUI(i, entry.playableCharacterInventoryManager.currentPCPT[i].pcID);
                        }
                    }
                    else
                    {
                        entry.playableCharacterInventoryManager.SetActivePlayableCharacter(false, loadNewCharacter);
                        if (entry.IsOwner)
                        {
                            PlayerUIManager.instance.playerUIHudManager.SetCharacterSlotUI(i, entry.playableCharacterInventoryManager.currentPCPT[i].pcID);
                        }
                    }
                    characterCount += 1;
                    entry.playableCharacterInventoryManager.pcObjects[i] = loadNewCharacter;
                }
                else
                {
                    entry.playableCharacterInventoryManager.pcObjects[i] = null;
                }
            }
            SetCurrentCharacterInfo(entry.playableCharacterEntryNetworkManager.currentPlayableCharacterID.Value);
        }

        /// <summary>
        /// currentCharacterのコンポーネントを紐づけてアクティブにする
        /// </summary>
        /// <param name="newID"></param>
        private void SetCurrentCharacterInfo(int newID)
        {
            if (!player.IsOwner)
            {
                FindCharacterByIDFromChildren(newID);
            }

            //SpawnEffect sE = GetComponent<SpawnEffect>();
            //sE.PlayEffect();
            WorldPlayableCharacterDatabase.instance.SpawnVFX(WorldPlayableCharacterDatabase.instance.spawnEffect, player.gameObject);
            player.characterSoundFXManager.PlaySpawnSoundFX();
            player.animator = playableCharacterModel.GetComponent<Animator>();
            player.characterAnimatorManager = playableCharacterModel.GetComponent<CharacterAnimatorManager>();
            player.playerAnimatorManager = playableCharacterModel.GetComponent<PlayerAnimatorManager>();
            player.playerEquipmentManager = playableCharacterModel.GetComponent<PlayerEquipmentManager>();
            //player.playerEffectsManager = playableCharacterEntryManager.playableCharacterModel.GetComponent<PlayerEffectsManager>();
            player.playerInventoryManager = playableCharacterModel.GetComponent<PlayerInventoryManager>();
            player.SetController(playableCharacterModel);
            player.myLockOnTransform = playableCharacterModel.GetComponentInChildren<LockOnTransform>();
            player.playerCombatManager.lockOnTransform = player.myLockOnTransform.transform;
            player.rightSkillManager = playableCharacterModel.GetComponent<RightSkillManager>();
            player.leftSkillManager = playableCharacterModel.GetComponent<LeftSkillManager>();
            player.playerBurstManager = playableCharacterModel.GetComponent<PlayerBurstManager>();
            player.characterStatsManager = playableCharacterModel.GetComponent<PlayerStatsManager>();
            player.playerStatsManager = playableCharacterModel.GetComponent<PlayerStatsManager>();
            player.pelvisOffset = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(newID).IKPelvisOffset;

            if (player.IsOwner)
            {
                if (player.playerInventoryManager.rightHandWeaponIndex == -1)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = 0;
                }
                else
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                }
                if (player.playerInventoryManager.leftHandWeaponIndex == -1)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = 0;
                }
                else
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID;
                }
                PlayerUIManager.instance.playerUIHudManager.SetSkillBurstSlotIcon(newID);
            }
        }

        /// <summary>
        /// currentPCIDが変更された際に呼び出される処理
        /// </summary>
        /// <param name="newID"></param>
        public void ChangeCharacterFromID(int newID)
        {
            entry.playableCharacterEntryManager.SetCurrentCharacterInfo(newID);

            PlayableCharacter newPlayableCharacter = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(newID);
            entry.playableCharacterInventoryManager.currentCharacter = newPlayableCharacter;
        }

        /// <summary>
        /// idを用いてキャラクターをcurrentPTから探し出しそのモデルを現在のアクティブキャラにする
        /// </summary>
        /// <param name="newCharacterID"></param>
        public void FindCharacterByIDFromChildren(int newCharacterID)
        {
            var children = new Transform[transform.childCount];
            PlayableCharacterManager newCharacter;
            GameObject newCharacterModel;
            for (var i = 0; i < children.Length; ++i)
            {
                newCharacterModel = transform.GetChild(i).gameObject;
                newCharacter = newCharacterModel.GetComponent<PlayableCharacterManager>();
                if (newCharacter == null)
                    continue;
                if (newCharacterID == newCharacter.playableCharacterID)
                {
                    entry.playableCharacterInventoryManager.SetActivePlayableCharacter(true, newCharacterModel);
                    LoadPlayableCharacter(newCharacter, newCharacterModel);
                    if (player.IsOwner)
                    {
                        int slotsIndex = Array.IndexOf(entry.playableCharacterInventoryManager.currentPCPT, newCharacter.playableCharacter);
                        entry.playableCharacterInventoryManager.currentCharacterIndex = slotsIndex;
                        entry.playableCharacterEntryNetworkManager.currentPlayableCharacterID.Value = newCharacter.playableCharacterID;
                    }
                }
                else
                {
                    entry.playableCharacterInventoryManager.SetActivePlayableCharacter(false, newCharacterModel);
                }
            }
        }
        /// <summary>
        /// 起動時にPT情報をセットする際に使用する
        /// </summary>
        private void SetCurrentPTIDList()
        {
            for (int i = 0; i < WorldPlayableCharacterDatabase.MaxPTCount; i++)
            {
                if (entry.playableCharacterInventoryManager.currentPCPT[i] == null)
                {
                    entry.playableCharacterInventoryManager.currentPCPT[i] = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(0);
                }
                entry.playableCharacterInventoryManager.currentPCPT[i] = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(entry.playableCharacterEntryNetworkManager.currentPTIDForSaveAndLoad[i]);
                entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList.Add(entry.playableCharacterInventoryManager.currentPCPT[i].pcID);
            }
        }
    }
}