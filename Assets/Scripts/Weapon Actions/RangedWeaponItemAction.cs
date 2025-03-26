using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 遠距離武器用弱攻撃
    /// 遠距離武器に強攻撃を作成するかは未定
    /// 遠距離武器でコンボを行うかも未定なので
    /// </summary>
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Ranged Weapon Action")]
    public class RangedWeaponItemAction : WeaponItemAction
    {
        [Header("Light Attacks")]
        [SerializeField] string[] light_Attack = { "Left_Ranged_Attack_01", "Left_Ranged_Attack_02", "Left_Ranged_Attack_03", "Left_Ranged_Attack_04" };
        [SerializeField] AttackType[] attackType = { AttackType.LightAttack01, AttackType.LightAttack02, AttackType.LightAttack03, AttackType.LightAttack04 };

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

                string lastAnimation = playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed;

                if (light_Attack.Contains(lastAnimation) || light_Attack.LastOrDefault() == lastAnimation)
                {
                    for (int i = 0; i < light_Attack.Length - 1; i++)
                    {
                        if (lastAnimation == light_Attack[i])
                        {
                            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, attackType[i + 1], light_Attack[i + 1], true, true, false, false);
                            break;
                        }
                    }
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, attackType[0], light_Attack[0], true, true, false, false);
                }

            }
            // 非モーション中
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, attackType[0], light_Attack[0], true, true, false, false);
            }
            else
            {
                // playerPerformingAction.isPerformingAction == true && playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon == false

            }
            // ロックオン中はターゲットに向き直る
            if (playerPerformingAction.playerNetworkManager.isLockedOn.Value)
            {
                playerPerformingAction.playerLocomotionManager.RotationToTarget();
            }
        }
    }
}
