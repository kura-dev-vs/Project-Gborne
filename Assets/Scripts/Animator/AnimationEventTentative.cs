using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 共通のアニメーションイベントを管理する時に使用するかも
    /// 今はアタック時のスタミナ消費のみ使用
    /// </summary>
    public class AnimationEventTentative : MonoBehaviour
    {
        PlayerManager player;
        WeaponItem currentWeaponBeingUsed;
        AttackType currentAttackType;

        public void SetManger()
        {
            player = GetComponentInParent<PlayerManager>();
        }
        public void DrainStaminaBasedOnAttack()
        {
            if (!player.IsOwner)
                return;
            currentWeaponBeingUsed = player.playerCombatManager.currentWeaponBeingUsed;
            currentAttackType = player.playerCombatManager.currentAttackType;
            if (currentWeaponBeingUsed == null)
                return;

            float staminaDeducted = 0;
            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                default:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
            }
            Debug.Log("Stamina deducted" + staminaDeducted);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }
    }
}
