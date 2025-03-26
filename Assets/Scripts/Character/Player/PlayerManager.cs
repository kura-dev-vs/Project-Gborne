using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace RK
{
    /// <summary>
    /// プレイヤー操作キャラクターの大元マネージャー
    /// 基本ここから各種マネージャーアクセスする
    /// NetworkBehaviour継承
    /// </summary>
    public class PlayerManager : CharacterManager
    {
        // PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        [Header("DEBUG MENU")]
        [HideInInspector] public EntryManager entry;
        public bool respawnCharacter = false;
        [SerializeField] bool switchRightWeapon = false;
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        public PlayerStatsManager playerStatsManager;
        public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public RightSkillManager rightSkillManager;
        [HideInInspector] public LeftSkillManager leftSkillManager;
        [HideInInspector] public PlayerBurstManager playerBurstManager;
        [HideInInspector] public PlayerInteractionManager playerInteractionManager;
        public PlayerEffectsManager playerEffectsManager;
        public PlayerCombatManager playerCombatManager;
        public Inventory inventory;
        protected override void Awake()
        {
            base.Awake();
            entry = GetComponent<EntryManager>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            rightSkillManager = GetComponent<RightSkillManager>();
            leftSkillManager = GetComponent<LeftSkillManager>();
            playerBurstManager = GetComponent<PlayerBurstManager>();
            playerInteractionManager = GetComponent<PlayerInteractionManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            inventory = GetComponent<Inventory>();
        }
        protected override void Update()
        {
            base.Update();
            if (!IsOwner)
                return;

            // isowner (ローカルプレイヤーのオブジェクト) でのみ実行

            // handle movement
            playerLocomotionManager.HandleAllMovement();

            // スタミナの回復
            playerLocomotionManager.RegenerateStamina();

            // スキル、爆発関連のUIHud表示を実行している (UI管理だけなのでリキャストは別のマネージャーで行っている)
            SetRecastSkillBurst();
            SetEnergyValue();

            // 機能の試運転デバッグ用
            DebugMenu();
        }
        protected override void LateUpdate()
        {
            if (!IsOwner)
                return;

            base.LateUpdate();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // "クライアントが接続すると呼び出されるコールバック。このコールバックはサーバと接続したローカルクライアントでのみ実行される。"
            // クライアントがサーバーと接続した瞬間に発生するコールバックに対し、"OnClientConnectedCallback"を実行するようスポーン時に設定しておく
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

            // isowner (ローカルプレイヤーのオブジェクト) でのみ実行
            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                // ネットワーク変数の対応
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                // スタミナや体力の変化時にローカルプレイヤーuiも変化させる
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerLocomotionManager.ResetStaminaRegenTimer;
            }

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
            // Lock On
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;
            // Equipment
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            playerNetworkManager.isBlocking.OnValueChanged += playerNetworkManager.OnIsBlockingChanged;
            playerNetworkManager.isPowerUps.OnValueChanged += playerNetworkManager.OnInPowerUpsChanged;
            // Flags
            playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;

            // 非サーバーのクライアントとして接続時にキャラクターデータをロードする
            // サーバーのホストはセーブデータ選択時にロード済であるため必要無い
            if (IsOwner && !IsServer)
            {
                LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }

            playerNetworkManager.currentScore.OnValueChanged += playerNetworkManager.OnCurrentScoreChange;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            // isowner (ローカルプレイヤーのオブジェクト) でのみ実行
            if (IsOwner)
            {
                playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

                playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged -= playerLocomotionManager.ResetStaminaRegenTimer;
            }

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;
            // Lock On
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;
            // Equipment
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            playerNetworkManager.isBlocking.OnValueChanged -= playerNetworkManager.OnIsBlockingChanged;
            // Flags
            playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;

            playerNetworkManager.currentScore.OnValueChanged -= playerNetworkManager.OnCurrentScoreChange;
        }

        /// <summary>
        /// クライアントがサーバーにコネクトした際に呼び出される
        /// ゲームセッションにプレイヤーを追加し、他のプレイヤーの情報を読み込む
        /// </summary>
        /// <param name="clientID"></param>
        private void OnClientConnectedCallback(ulong clientID)
        {
            WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

            // 他のキャラクターの情報をロードする
            // 非サーバーかつローカルプレイヤーの場合のみ実行
            // 同期のためにゲームセッションへの参加時に必要
            if (!IsServer && IsOwner)
            {
                foreach (var player in WorldGameSessionManager.instance.players)
                {
                    if (player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDamageAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }
            return base.ProcessDeathEvent(manuallySelectDamageAnimation);

            // 回生を設定するならこのあたりに実行するか？
        }

        /// <summary>
        /// 復活時のメソッド。現在はなんの演出もなく復活するだけ
        /// </summary>
        public override void ReviveCharacter()
        {
            base.ReviveCharacter();
            if (IsOwner)
            {
                isDead.Value = false;
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

                playerAnimatorManager.PlayTargetActionAnimation("Empty", false, true, true, true);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                currentCharacterData.sceneIndex = WorldSaveGameManager.instance.GetWorldSceneIndex();
            }
            else
            {
                currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            }
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;

            for (int i = 0; i < currentCharacterData.ptID.Length; i++)
            {
                currentCharacterData.ptID[i] = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<EntryManager>().playableCharacterEntryNetworkManager.currentPTIDForSaveAndLoad[i];
            }

            for (int i = 1; i < WorldPlayableCharacterDatabase.instance.GetPlayableCharacterCount(); i++)
            {
                // セーブデータにpcの情報が入っていない場合、デフォルト値で保存
                if (!currentCharacterData.pcRightWeapon.ContainsKey(i))
                {
                    currentCharacterData.pcRightWeapon.Add(i, WorldItemDatabase.instance.unarmedWeapon.itemID);
                    currentCharacterData.pcLeftWeapon.Add(i, WorldItemDatabase.instance.unarmedWeapon.itemID);

                    currentCharacterData.pcHatOutfit.Add(i, -1);
                    currentCharacterData.pcJacketOutfit.Add(i, -1);
                    currentCharacterData.pcTopsOutfit.Add(i, -1);
                    currentCharacterData.pcBottomsOutfit.Add(i, -1);
                    currentCharacterData.pcShoesOutfit.Add(i, -1);

                }
                // セーブデータにpcの情報が入っている場合、現在の情報を保存
                else
                {
                    currentCharacterData.pcRightWeapon.Remove(i);
                    currentCharacterData.pcLeftWeapon.Remove(i);

                    currentCharacterData.pcHatOutfit.Remove(i);
                    currentCharacterData.pcJacketOutfit.Remove(i);
                    currentCharacterData.pcTopsOutfit.Remove(i);
                    currentCharacterData.pcBottomsOutfit.Remove(i);
                    currentCharacterData.pcShoesOutfit.Remove(i);


                    if (WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).rightWeapon != null)
                        currentCharacterData.pcRightWeapon.Add(i, WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).rightWeapon.itemID);
                    else
                        currentCharacterData.pcRightWeapon.Add(i, WorldItemDatabase.instance.unarmedWeapon.itemID);

                    if (WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).leftWeapon != null)
                        currentCharacterData.pcLeftWeapon.Add(i, WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).leftWeapon.itemID);
                    else
                        currentCharacterData.pcLeftWeapon.Add(i, WorldItemDatabase.instance.unarmedWeapon.itemID);


                    if (WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).hatOutfit != null)
                        currentCharacterData.pcHatOutfit.Add(i, WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).hatOutfit.itemID);
                    else
                        currentCharacterData.pcHatOutfit.Add(i, -1);

                    if (WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).jacketOutfit != null)
                        currentCharacterData.pcJacketOutfit.Add(i, WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).jacketOutfit.itemID);
                    else
                        currentCharacterData.pcJacketOutfit.Add(i, -1);

                    if (WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).topsOutfit != null)
                        currentCharacterData.pcTopsOutfit.Add(i, WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).topsOutfit.itemID);
                    else
                        currentCharacterData.pcTopsOutfit.Add(i, -1);

                    if (WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).bottomsOutfit != null)
                        currentCharacterData.pcBottomsOutfit.Add(i, WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).bottomsOutfit.itemID);
                    else
                        currentCharacterData.pcBottomsOutfit.Add(i, -1);

                    if (WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).shoesOutfit != null)
                        currentCharacterData.pcShoesOutfit.Add(i, WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i).shoesOutfit.itemID);
                    else
                        currentCharacterData.pcShoesOutfit.Add(i, -1);

                }
            }

            if (currentCharacterData.itemsInInventory != null)
                currentCharacterData.itemsInInventory.Clear();

            for (int i = 0; i < inventory.itemsInInventory.Count; i++)
            {
                currentCharacterData.itemsInInventory.Add(i, inventory.itemsInInventory[i].itemID);
            }
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition + 3f, currentCharacterData.zPosition);
            transform.position = myPosition;

            playerNetworkManager.vitality.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;

            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);

            for (int i = 0; i < currentCharacterData.ptID.Length; i++)
            {
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<EntryManager>().playableCharacterEntryNetworkManager.currentPTIDForSaveAndLoad[i] = currentCharacterData.ptID[i];
            }

            for (int i = 1; i < WorldPlayableCharacterDatabase.instance.GetPlayableCharacterCount(); i++)
            {
                PlayableCharacter pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i);
                pc.rightWeapon = WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.pcRightWeapon[i]);
                pc.leftWeapon = WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.pcLeftWeapon[i]);

                inventory.ChangeCurrentEquipmentList(i, WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.pcRightWeapon[i]), EquipmentType.RightWeapon01);
                inventory.ChangeCurrentEquipmentList(i, WorldItemDatabase.instance.GetWeaponByID(currentCharacterData.pcLeftWeapon[i]), EquipmentType.LeftWeapon01);


                pc.hatOutfit = WorldItemDatabase.instance.GetHatOutfitByID(currentCharacterData.pcHatOutfit[i]);
                pc.jacketOutfit = WorldItemDatabase.instance.GetJacketOutfitByID(currentCharacterData.pcJacketOutfit[i]);
                pc.topsOutfit = WorldItemDatabase.instance.GetTopsOutfitByID(currentCharacterData.pcTopsOutfit[i]);
                pc.bottomsOutfit = WorldItemDatabase.instance.GetBottomsOutfitByID(currentCharacterData.pcBottomsOutfit[i]);
                pc.shoesOutfit = WorldItemDatabase.instance.GetShoesOutfitByID(currentCharacterData.pcShoesOutfit[i]);
                pc.CalculateCharacterStats();

                inventory.ChangeCurrentEquipmentList(i, WorldItemDatabase.instance.GetHatOutfitByID(currentCharacterData.pcHatOutfit[i]), EquipmentType.Hat);
                inventory.ChangeCurrentEquipmentList(i, WorldItemDatabase.instance.GetJacketOutfitByID(currentCharacterData.pcJacketOutfit[i]), EquipmentType.Jacket);
                inventory.ChangeCurrentEquipmentList(i, WorldItemDatabase.instance.GetTopsOutfitByID(currentCharacterData.pcTopsOutfit[i]), EquipmentType.Tops);
                inventory.ChangeCurrentEquipmentList(i, WorldItemDatabase.instance.GetBottomsOutfitByID(currentCharacterData.pcBottomsOutfit[i]), EquipmentType.Bottoms);
                inventory.ChangeCurrentEquipmentList(i, WorldItemDatabase.instance.GetShoesOutfitByID(currentCharacterData.pcShoesOutfit[i]), EquipmentType.Shoes);

                WorldPlayableCharacterDatabase.instance.SetEquipment(pc);
            }
            if (currentCharacterData.itemsInInventory != null)
            {
                for (int i = 0; i < currentCharacterData.itemsInInventory.Count; i++)
                {
                    int id = currentCharacterData.itemsInInventory[i];
                    inventory.AddItemToInventory(WorldItemDatabase.instance.GetItemByID(id));
                }
            }
        }


        public void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            // 武器の同期
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
            playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

            // ブロック状態の同期
            playerNetworkManager.OnIsBlockingChanged(false, playerNetworkManager.isBlocking.Value);

            if (playerNetworkManager.isLockedOn.Value)
            {
                playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
            }
        }
        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }
        }
        /// <summary>
        /// アクティブキャラクター変更時に親オブジェクトのcharactercontrollerのコライダー (厳密にはcolliderではなくcharactercontroller固有の当たり判定) をキャラクター固有のものに変更
        /// </summary>
        /// <param name="playableCharacter"></param>
        public void SetController(GameObject playableCharacter)
        {
            characterController.height = playableCharacter.GetComponent<CharacterController>().height;
        }

        /// <summary>
        /// 爆発とスキルのUI関連。後々別のコンポーネントに移したほうがいいかも
        /// </summary>
        public void SetRecastSkillBurst()
        {
            if (playerBurstManager.nowRecasting)
            {
                PlayerUIManager.instance.playerUIHudManager.SetEnabledBurstRecast(playerBurstManager.currentRecastBurst);
            }
            else
            {
                PlayerUIManager.instance.playerUIHudManager.SetDisableBurstRecast();
            }
            if (rightSkillManager.nowRecasting)
            {
                PlayerUIManager.instance.playerUIHudManager.SetEnabledRightRecast(rightSkillManager.currentRecast);
            }
            else
            {
                PlayerUIManager.instance.playerUIHudManager.SetDisableRightRecast();
            }

            if (leftSkillManager.nowRecasting)
            {
                PlayerUIManager.instance.playerUIHudManager.SetEnabledLeftRecast(leftSkillManager.currentRecast);
            }
            else
            {
                PlayerUIManager.instance.playerUIHudManager.SetDisableLeftRecast();
            }
        }
        private void SetEnergyValue()
        {
            float value = playerBurstManager.currentEnergy / playerBurstManager.rechargeEnergy;
            PlayerUIManager.instance.playerUIHudManager.SetEnergyValue(value);
        }
    }
}
