using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 強攻撃
    /// </summary>
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";
        [SerializeField] string heavy_Attack_02 = "Main_Heavy_Attack_02";
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

            PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
        }
        private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // モーション中且つコンボが可能な状態
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                // 最後の攻撃モーションに応じたモーションを行う
                if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack02, heavy_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
                }
            }
            // 非モーション中
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
        }
    }
}
