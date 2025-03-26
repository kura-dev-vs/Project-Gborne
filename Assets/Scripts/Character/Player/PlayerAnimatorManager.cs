using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

namespace RK
{
    /// <summary>
    /// player用のanimatorManager
    /// animatation中に呼ばれる共通メソッドはここに書く
    /// </summary>
    public class PlayerAnimatorManager : CharacterAnimatorManager
    {
        [HideInInspector] public PlayerManager player;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        public void SetManager()
        {
            character = GetComponentInParent<CharacterManager>();
            player = GetComponentInParent<PlayerManager>();
        }

        private void OnAnimatorMove()
        {
            if (applyRootMotion)
            {
                Vector3 velocity = player.animator.deltaPosition;
                player.characterController.Move(velocity * 1);
                player.transform.rotation *= player.animator.deltaRotation;
            }
            /*
            var deltaPosition = this.animator.deltaPosition;
            var deltaRotation = this.animator.deltaRotation;

            this.parentTransform.localPosition += deltaPosition;
            this.parentTransform.localRotation = deltaRotation * this.parentTransform.localRotation;
            */

        }
        // animation event calls
        public override void EnableCanDoCombo()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerCombatManager.canComboWithMainHandWeapon = true;
                EableCanDodge();
            }
            else
            {

            }
        }
        public override void DisableCanDoCombo()
        {
            player.playerCombatManager.canComboWithMainHandWeapon = false;
        }
        public override void EableCanDodge()
        {
            player.canDodge = true;
        }
        public override void InstantiationBullet()
        {
            Transform weaponTransform;
            WeaponItem rangedWeapon;
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                weaponTransform = player.playerEquipmentManager.rightHandWeaponModel.transform;
                rangedWeapon = player.playerInventoryManager.currentRightHandWeapon;
            }
            else
            {
                weaponTransform = player.playerEquipmentManager.leftHandWeaponModel.transform;
                rangedWeapon = player.playerInventoryManager.currentLeftHandWeapon;
            }

            Vector3 bulletDirection;
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                Vector3 targetPosition = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position;
                bulletDirection = targetPosition - weaponTransform.position;
            }
            else
            {
                bulletDirection = player.transform.forward;
            }
            Quaternion rot = player.transform.rotation;
            rot = Quaternion.Euler(90, 90, 0);
            GameObject bullet = Instantiate(rangedWeapon.bullet, weaponTransform.position, rot);
            RangedWeaponDamageCollider bulletCllider = bullet.GetComponent<RangedWeaponDamageCollider>();
            player.playerEquipmentManager.leftWeaponManager.SetRangedDamageCollider(bulletCllider, player, rangedWeapon);

            bullet.GetComponent<BulletPrefabController>().bulletDirection = bulletDirection;
            if (player.IsOwner)
            {
                int maxHealth = player.playerNetworkManager.maxHealth.Value;
                float cp = maxHealth * rangedWeapon.baseHPCostPct / 100;
                if (player.playerNetworkManager.currentHealth.Value < (int)Mathf.Ceil(cp))
                {
                    player.playerNetworkManager.currentHealth.Value -= player.playerNetworkManager.currentHealth.Value - 1;
                }
                else
                {
                    player.playerNetworkManager.currentHealth.Value -= (int)Mathf.Ceil(cp);
                }
            }
        }
    }
}
