using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

namespace RK
{
    /// <summary>
    /// プレイヤーの武器アクション関連
    /// </summary>
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;
        public WeaponItem currentWeaponBeingUsed;
        [Header("Flags")]
        public bool canComboWithMainHandWeapon = false;
        // public bool canComboWithOffHandWeapon=false;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        /// <summary>
        /// InputManagerから入力を受け取りその際持っていた武器とその武器に搭載されたweaponActionを実行する
        /// </summary>
        /// <param name="weaponAction"></param>搭載されたweaponAction
        /// <param name="weaponPerformingAction"></param>対応する武器
        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            if (weaponAction == null)
            {
                Debug.Log("DON'T SET WEAPON ACTION");
                return;
            }
            // アクションを実行
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

            // サーバーにアクションを実行したことを通知
            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }
        public override void CloseAllDamageColliders()
        {
            base.CloseAllDamageColliders();

            player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
            player.playerEquipmentManager.leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }

        /// <summary>
        /// スタミナ消費時のメソッド
        /// 基本的に攻撃モーションのアニメーションイベントから呼び出される
        /// </summary> 
        public virtual void DrainStaminaBasedOnAttack()
        {
            if (!player.IsOwner)
                return;
            if (currentWeaponBeingUsed == null)
                return;

            float staminaDeducted = 0;
            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack03:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack04:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargedAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargedAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                    break;
                case AttackType.RunningAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningAttackStaminaCostMultiplier;
                    break;
                case AttackType.RollingAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.rollingAttackStaminaCostMultiplier;
                    break;
                case AttackType.BackstepAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backstepAttackStaminaCostMultiplier;
                    break;
                default:
                    break;
            }
            Debug.Log("Stamina deducted" + staminaDeducted);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

        public override void AttemptRiposte(RaycastHit hit)
        {
            CharacterManager targetCharacter = hit.transform.gameObject.GetComponent<CharacterManager>();

            if (targetCharacter == null)
                return;
            if (!targetCharacter.characterNetworkManager.isRipostable.Value)
                return;
            // 対象が致命の一撃を食らっている状態ならreturn
            if (targetCharacter.characterNetworkManager.isBeingCriticallyDamaged.Value)
                return;

            // 近接武器のみ有効
            PlayableCharacter pc = player.entry.playableCharacterInventoryManager.currentCharacter;

            // 武器のoverride animator controllerによってアニメーションが変化
            character.characterAnimatorManager.PlayTargetActionAnimationInstantly("Riposte_01", true);

            // 致命の一撃中は無敵
            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = true;

            TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeCriticalDamageEffect);

            damageEffect.physicalDamage = pc.physicalDamage;
            damageEffect.holyDamage = pc.holyDamage;
            damageEffect.fireDamage = pc.fireDamage;
            damageEffect.lightningDamage = pc.lightningDamage;
            damageEffect.magicDamage = pc.magicDamage;
            damageEffect.poiseDamage = pc.poiseDamage;
            damageEffect.stanceDamage = pc.stanceDamage;

            damageEffect.physicalDamage *= pc.riposte_Attack_01_Modifier;
            damageEffect.holyDamage *= pc.riposte_Attack_01_Modifier;
            damageEffect.fireDamage *= pc.riposte_Attack_01_Modifier;
            damageEffect.lightningDamage *= pc.riposte_Attack_01_Modifier;
            damageEffect.magicDamage *= pc.riposte_Attack_01_Modifier;
            damageEffect.poiseDamage *= pc.riposte_Attack_01_Modifier;
            damageEffect.stanceDamage *= pc.riposte_Attack_01_Modifier;

            targetCharacter.characterNetworkManager.NotifyTheServerOfRiposteServerRpc(
                targetCharacter.NetworkObjectId,
                character.NetworkObjectId,
                "Riposted_01",
                pc.pcID,
                damageEffect.physicalDamage,
                damageEffect.magicDamage,
                damageEffect.fireDamage,
                damageEffect.holyDamage,
                damageEffect.poiseDamage,
                damageEffect.stanceDamage);
        }

        public override void AttemptBackstab(RaycastHit hit)
        {
            CharacterManager targetCharacter = hit.transform.gameObject.GetComponent<CharacterManager>();

            if (targetCharacter == null)
                return;
            if (!targetCharacter.characterCombatManager.canBeBackstabbed)
                return;
            // 対象が致命の一撃を食らっている状態ならreturn
            if (targetCharacter.characterNetworkManager.isBeingCriticallyDamaged.Value)
                return;

            PlayableCharacter pc = player.entry.playableCharacterInventoryManager.currentCharacter;

            // 武器のoverride animator controllerによってアニメーションが変化
            character.characterAnimatorManager.PlayTargetActionAnimationInstantly("Backstab_01", true);

            // 致命の一撃中は無敵
            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = true;

            TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeCriticalDamageEffect);

            damageEffect.physicalDamage = pc.physicalDamage;
            damageEffect.holyDamage = pc.holyDamage;
            damageEffect.fireDamage = pc.fireDamage;
            damageEffect.lightningDamage = pc.lightningDamage;
            damageEffect.magicDamage = pc.magicDamage;
            damageEffect.poiseDamage = pc.poiseDamage;
            damageEffect.stanceDamage = pc.stanceDamage;

            damageEffect.physicalDamage *= pc.backstab_Attack_01_Modifier;
            damageEffect.holyDamage *= pc.backstab_Attack_01_Modifier;
            damageEffect.fireDamage *= pc.backstab_Attack_01_Modifier;
            damageEffect.lightningDamage *= pc.backstab_Attack_01_Modifier;
            damageEffect.magicDamage *= pc.backstab_Attack_01_Modifier;
            damageEffect.poiseDamage *= pc.backstab_Attack_01_Modifier;
            damageEffect.stanceDamage *= pc.backstab_Attack_01_Modifier;

            targetCharacter.characterNetworkManager.NotifyTheServerOfBackstabServerRpc(
                targetCharacter.NetworkObjectId,
                character.NetworkObjectId,
                "Backstabed_01",
                pc.pcID,
                damageEffect.physicalDamage,
                damageEffect.magicDamage,
                damageEffect.fireDamage,
                damageEffect.holyDamage,
                damageEffect.poiseDamage,
                damageEffect.stanceDamage);
        }

    }
}
