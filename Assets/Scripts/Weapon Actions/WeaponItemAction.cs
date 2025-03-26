using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 武器を用いたアクションをまとめたもの
    /// 一つの武器が持つアクション (RB, LB) に対しこのWeaponItemActionを継承したspriptableObjectをそれぞれ設定する
    /// </summary>
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
    public class WeaponItemAction : ScriptableObject
    {
        public int actionID;

        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // すべての武器に共通して必要なこととして、アクションを起こす事前に武器のIDの整合性を取る必要がある
            if (playerPerformingAction.IsOwner)
            {
                playerPerformingAction.playerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformingAction.itemID;
            }
        }
    }
}
