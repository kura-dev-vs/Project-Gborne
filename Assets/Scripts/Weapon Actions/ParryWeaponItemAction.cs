using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// パリィ用
    /// </summary>
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Parry Action")]
    public class ParryWeaponItemAction : WeaponItemAction
    {
        [Header("Parry Action")]
        [SerializeField] string parryAction;
        [SerializeField] AttackType[] attackType;
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            if (!playerPerformingAction.IsOwner)
                return;
            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;
            if (!playerPerformingAction.playerNetworkManager.isGrounded.Value)
                return;
            if (playerPerformingAction.IsOwner)
                playerPerformingAction.playerNetworkManager.isAttacking.Value = true;

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);
        }
        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // モーション中且つコンボが可能な状態
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, parryAction, true, true, false);
                playerPerformingAction.playerCombatManager.DrainStaminaBasedOnAttack();
            }
            // 非モーション中
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, parryAction, true, true, false);
                playerPerformingAction.playerCombatManager.DrainStaminaBasedOnAttack();
            }
            else
            {
                // playerPerformingAction.isPerformingAction == true && playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon == false
            }

        }
    }
}
