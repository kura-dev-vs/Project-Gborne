using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Off Hand Melee Action")]
    public class OffHandMeleeAction : WeaponItemAction
    {
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

            // ブロック可能かチェック
            if (!playerPerformingAction.playerCombatManager.canBlock)
                return;

            // 攻撃の動作をしているか
            if (playerPerformingAction.playerNetworkManager.isAttacking.Value)
            {
                if (playerPerformingAction.IsOwner)
                    playerPerformingAction.playerNetworkManager.isBlocking.Value = false;
                return;
            }

            if (playerPerformingAction.playerNetworkManager.isBlocking.Value)
                return;

            if (playerPerformingAction.IsOwner)
            {
                playerPerformingAction.playerNetworkManager.isBlocking.Value = true;
            }
        }

    }
}
