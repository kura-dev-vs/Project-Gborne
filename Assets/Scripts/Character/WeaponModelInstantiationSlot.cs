using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 武器モデルをinstantiateするためのスクリプト
    /// このコンポーネントを持つゲームオブジェクトの位置に武器が生成される
    /// </summary>
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        public WeaponModelSlot weaponSlot;
        public GameObject currentWeaponModel;
        public void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }
        public void LoadWeapon(GameObject weaponModel)
        {
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.localRotation = Quaternion.identity;
            //weaponModel.transform.localScale = Vector3.one;
        }
        public void PlaceWeaponModelInUnequippedSlot(GameObject weaponModel, WeaponClass weaponClass, PlayerManager player)
        {
            // to do, move weapon on back closer or more outward on chest equipment (so it doesnt apper to float)
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent.parent = transform;

            switch (weaponClass)
            {
                case WeaponClass.StraightSword:
                    weaponModel.transform.localPosition = new Vector3(0.0064f, 0f, -0.06f);
                    weaponModel.transform.localRotation = Quaternion.Euler(194, 90, -0.22f);
                    break;
                case WeaponClass.Spear:
                    weaponModel.transform.localPosition = new Vector3(0.0064f, 0f, -0.06f);
                    weaponModel.transform.localRotation = Quaternion.Euler(194, 90, -0.22f);
                    break;
                case WeaponClass.Gun:
                    weaponModel.transform.localPosition = new Vector3(0.0064f, 0f, -0.06f);
                    weaponModel.transform.localRotation = Quaternion.Euler(194, 90, -0.22f);
                    break;
                case WeaponClass.MediumShield:
                    weaponModel.transform.localPosition = new Vector3(0.09f, -0.292f, -0.1f);
                    weaponModel.transform.localRotation = Quaternion.Euler(0, -193.2f, 0);
                    break;
                default:
                    break;
            }
        }
    }
}
