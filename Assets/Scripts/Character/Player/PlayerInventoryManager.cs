using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 現在装備中の武器関連の管理を行う
    /// </summary>
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public WeaponItem currentRightHandWeapon;
        public WeaponItem currentLeftHandWeapon;
        [Header("Quick Slots")]
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
        public int rightHandWeaponIndex = 0;
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
        public int leftHandWeaponIndex = 0;

        [Header("Outfit")]
        public HatOutfitItem hatOutfitItem;
        public JacketOutfitItem jacketOutfitItem;
        public TopsOutfitItem topsOutfitItem;
        public BottomsOutfitItem bottomsOutfitItem;
        public ShoesOutfitItem shoesOutfitItem;

        public void ChangeCurrentWeapon()
        {
            if (rightHandWeaponIndex != -1)
                currentRightHandWeapon = weaponsInRightHandSlots[rightHandWeaponIndex];
            else
                currentRightHandWeapon = WorldItemDatabase.instance.unarmedWeapon;

            if (leftHandWeaponIndex != -1)
                currentLeftHandWeapon = weaponsInLeftHandSlots[leftHandWeaponIndex];
            else
                currentLeftHandWeapon = WorldItemDatabase.instance.unarmedWeapon;

            rightHandWeaponIndex = -1;
            leftHandWeaponIndex = -1;
            currentRightHandWeapon = WorldItemDatabase.instance.unarmedWeapon;
            currentLeftHandWeapon = WorldItemDatabase.instance.unarmedWeapon;

        }
    }
}
