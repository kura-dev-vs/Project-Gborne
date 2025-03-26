using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace RK
{
    /// <summary>
    /// 武器アイテムに付けるコンポーネント
    /// 同オブジェクトの子部分に付与されているはずの武器コライダーを起動時に取得
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        public MeleeWeaponDamageCollider meleeDamageCollider;
        public RangedWeaponDamageCollider rangedDamageCollider;
        public GameObject bullet;
        private void Awake()
        {
            if (GetComponentInChildren<MeleeWeaponDamageCollider>() != null)
            {
                meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
            }
            else if (GetComponentInChildren<RangedWeaponDamageCollider>() != null)
            {
                rangedDamageCollider = GetComponentInChildren<RangedWeaponDamageCollider>();
            }
        }
        /// <summary>
        /// 固有の数値関連をweaponItemオブジェクトから取得し, Colliderに付与する
        /// 遠距離武器の場合はここでcolliderに付与された数値を、弾丸発射時に生成した弾丸が持つcolliderにコピーする
        /// </summary>
        /// <param name="characterWieldWeapon"></param>
        /// <param name="weapon"></param>
        public void SetWeaponDamage(CharacterManager characterWieldWeapon, WeaponItem weapon)
        {
            if (weapon.bullet != null)
            {
                SetRangedDamageCollider(rangedDamageCollider, characterWieldWeapon, weapon);
            }
            else
            {
                meleeDamageCollider.characterCausingDamage = characterWieldWeapon;
                meleeDamageCollider.physicalDamage = weapon.physicalDamage;
                meleeDamageCollider.magicDamage = weapon.magicDamage;
                meleeDamageCollider.fireDamage = weapon.fireDamage;
                meleeDamageCollider.lightningDamage = weapon.lightningDamage;
                meleeDamageCollider.holyDamage = weapon.holyDamage;
                meleeDamageCollider.poiseDamage = weapon.poiseDamage;
                meleeDamageCollider.stanceDamage = weapon.stanceDamage;

                meleeDamageCollider.light_Attack_01_Modifier = weapon.light_Attack_01_Modifier;
                meleeDamageCollider.light_Attack_02_Modifier = weapon.light_Attack_02_Modifier;
                meleeDamageCollider.light_Attack_03_Modifier = weapon.light_Attack_03_Modifier;
                meleeDamageCollider.light_Attack_04_Modifier = weapon.light_Attack_04_Modifier;
                meleeDamageCollider.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_Modifier;
                meleeDamageCollider.heavy_Attack_02_Modifier = weapon.heavy_Attack_02_Modifier;
                meleeDamageCollider.charge_Attack_01_Modifier = weapon.charge_Attack_01_Modifier;
                meleeDamageCollider.charge_Attack_02_Modifier = weapon.charge_Attack_02_Modifier;
                meleeDamageCollider.running_Attack_01_Modifier = weapon.running_Attack_01_Modifier;
                meleeDamageCollider.rolling_Attack_01_Modifier = weapon.rolling_Attack_01_Modifier;
                meleeDamageCollider.backstep_Attack_01_Modifier = weapon.backstep_Attack_01_Modifier;

            }
        }

        public void SetRangedDamageCollider(RangedWeaponDamageCollider collider, CharacterManager characterWieldWeapon, WeaponItem weapon)
        {
            collider.characterCausingDamage = characterWieldWeapon;
            collider.physicalDamage = weapon.physicalDamage;
            collider.magicDamage = weapon.magicDamage;
            collider.fireDamage = weapon.fireDamage;
            collider.lightningDamage = weapon.lightningDamage;
            collider.holyDamage = weapon.holyDamage;
            collider.poiseDamage = weapon.poiseDamage;
            collider.stanceDamage = weapon.stanceDamage;

            collider.light_Attack_01_Modifier = weapon.light_Attack_01_Modifier;
            collider.light_Attack_02_Modifier = weapon.light_Attack_02_Modifier;
            collider.light_Attack_03_Modifier = weapon.light_Attack_03_Modifier;
            collider.light_Attack_04_Modifier = weapon.light_Attack_04_Modifier;
            collider.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_Modifier;
            collider.heavy_Attack_02_Modifier = weapon.heavy_Attack_02_Modifier;
            collider.charge_Attack_01_Modifier = weapon.charge_Attack_01_Modifier;
            collider.charge_Attack_02_Modifier = weapon.charge_Attack_02_Modifier;
        }
    }
}

