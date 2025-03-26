using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace RK
{
    /// <summary>
    /// NetworkBehaviour継承
    /// ネットワーク変数のイベントはここに書く
    /// サーバー、各種クライアントへの通知メソッド関連もここ
    /// </summary>
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player;
        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [Header("Equipment")]
        public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Power Ups")]
        public NetworkVariable<bool> isPowerUps = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        public void SetCharacterActionHand(bool rightHandedAction)
        {
            if (rightHandedAction)
            {
                isUsingLeftHand.Value = false;
                isUsingRightHand.Value = true;
            }
            else
            {
                isUsingRightHand.Value = false;
                isUsingLeftHand.Value = true;
            }
        }
        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            // ここでNullReferenceExceptionがおきてる
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
            currentHealth.Value = maxHealth.Value;
        }
        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurace)
        {
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurace);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
            currentStamina.Value = maxStamina.Value;
        }
        public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = WorldItemDatabase.instance.GetWeaponByID(newID);
            player.playerInventoryManager.currentRightHandWeapon = newWeapon;
            player.playerEquipmentManager.LoadRightWeapon();
            if (player.IsOwner)
            {
                PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon(newID);
            }
        }
        public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = WorldItemDatabase.instance.GetWeaponByID(newID);
            player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
            player.playerEquipmentManager.LoadLeftWeapon();
            if (player.IsOwner)
            {
                PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon(newID);
            }
        }
        public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = WorldItemDatabase.instance.GetWeaponByID(newID);
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon;

            if (player.IsOwner)
                return;

            if (player.playerCombatManager.currentWeaponBeingUsed != null)
                player.playerAnimatorManager.UpdateAnimatorController(player.playerCombatManager.currentWeaponBeingUsed.weaponAnimator);
        }
        public override void OnIsBlockingChanged(bool oldStatus, bool newStatus)
        {
            base.OnIsBlockingChanged(oldStatus, newStatus);

            if (IsOwner)
            {
                player.playerStatsManager.blockingPhysicalAbsorption = player.playerCombatManager.currentWeaponBeingUsed.physicalBaseDamageAbsorption;
                player.playerStatsManager.blockingMagicAbsorption = player.playerCombatManager.currentWeaponBeingUsed.magicBaseDamageAbsorption;
                player.playerStatsManager.blockingFireAbsorption = player.playerCombatManager.currentWeaponBeingUsed.fireBaseDamageAbsorption;
                player.playerStatsManager.blockingLightningAbsorption = player.playerCombatManager.currentWeaponBeingUsed.lightningBaseDamageAbsorption;
                player.playerStatsManager.blockingHolyAbsorption = player.playerCombatManager.currentWeaponBeingUsed.holyBaseDamageAbsorption;
                player.playerStatsManager.blockingStability = player.playerCombatManager.currentWeaponBeingUsed.stability;
            }
        }
        public void OnInPowerUpsChanged(bool oldStatus, bool newStatus)
        {
            if (!isPowerUps.Value)
            {
                player.playerEffectsManager.RemoveStaticEffect(WorldCharacterEffectsManager.instance.powerUpsEffect.staticEffectID);
                player.playerEffectsManager.RemoveStaticEffect(WorldCharacterEffectsManager.instance.absorptionEffect.staticEffectID);
            }
            else
            {
                StaticCharacterEffect powerUpsEffect = Instantiate(WorldCharacterEffectsManager.instance.powerUpsEffect);
                player.playerEffectsManager.AddStaticEffect(powerUpsEffect);

                StaticCharacterEffect absorptionEffect = Instantiate(WorldCharacterEffectsManager.instance.absorptionEffect);
                player.playerEffectsManager.AddStaticEffect(absorptionEffect);
            }
        }

        // item actions
        [ServerRpc]
        public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
        {
            if (IsServer)
            {
                NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
            }
        }
        [ClientRpc]
        private void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
        {
            // ローカルプレイヤーにも通知が来るが、ローカルプレイやーは既にアクションを行っているのでスルー
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformWeaponBasedAction(actionID, weaponID);
            }
        }
        private void PerformWeaponBasedAction(int actionID, int weaponID)
        {
            WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);
            if (weaponAction != null)
            {
                weaponAction.AttemptToPerformAction(player, WorldItemDatabase.instance.GetWeaponByID(weaponID));
            }
            else
            {
                Debug.LogError("ACTION IS NULL, CANNOT BE PERFORMED");
            }
        }

        public void OnCurrentScoreChange(int oldID, int newID)
        {
            if (player.IsOwner)
            {
                PlayerUIManager.instance.playerUIHudManager.SetScore(newID);
            }
        }
    }
}