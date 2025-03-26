using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace RK
{
    /// <summary>
    /// 右手武器、左手武器等装備の管理と武器の切り替えメソッド
    /// 武器アクションで呼び出されるevent管理
    /// 1. メイン武器以外に別の武器を持っているかチェックし、持っている場合は決して非武装に切り替えず、武器1と武器2の間でローテーションする。
    /// 2.メインの武器に戻る前に、両方の空きスロットを処理しない。
    /// </summary>
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        public PlayerManager player;
        [Header("Weapon Model Instantiation Slots")]
        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandWeaponSlot;
        public WeaponModelInstantiationSlot leftHandShieldSlot;
        public WeaponModelInstantiationSlot backRightSlot;
        public WeaponModelInstantiationSlot backLeftSlot;

        [Header("Weapon Managers")]
        public WeaponManager rightWeaponManager;
        public WeaponManager leftWeaponManager;
        [Header("Weapon Models")]
        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
            InitializeWeaponSlots();
        }
        protected override void Start()
        {
            base.Start();
            LoadWeaponsOnBothHands();
        }
        public void SetManger()
        {
            player = GetComponentInParent<PlayerManager>();
        }

        private void InitializeWeaponSlots()
        {
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();
            foreach (var weaponSlot in weaponSlots)
            {
                if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandWeaponSlot)
                {
                    leftHandWeaponSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandShieldSlot)
                {
                    leftHandShieldSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.backRightSlot)
                {
                    backRightSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.backLeftSlot)
                {
                    backLeftSlot = weaponSlot;
                }
            }
        }
        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();
            LoadLeftWeapon();
        }
        // 右手武器
        public void SwitchRightWeapon()
        {
            if (!player.IsOwner)
                return;
            if (player.isPerformingAction)
                return;
            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            // 両手持ちなら両手持ちを無効にする

            // rightHandWeaponIndexを一つ進めて、次の可能性のある武器に切り替える。
            player.playerInventoryManager.rightHandWeaponIndex += 1;

            // indexが範囲外になったら0に戻る
            if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
            {
                player.playerInventoryManager.rightHandWeaponIndex = 0;

                // 武器を1個以上持ってるかどうかチェックして持ってた場合一番最初に出てきた武器をfirstWeaponに入れる
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;
                for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.rightHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
                }
                return;
            }

            foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
            {
                // 次の候補武器がunarmed weaponではない場合
                if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                    // ネットワーク変数のcurrentRightHandWeaponIDを切り替える
                    player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                    Debug.Log("right" + player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID);
                    return;
                }
            }

            if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
            {
                SwitchRightWeapon();
            }
            else
            {
                /*
                // 現状以下が動作することはない
                // 武器を1個以上持ってるかどうかチェックして持ってた場合一番最初に出てきた武器をfirstWeaponに入れる
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;
                for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.rightHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
                }
                */
            }
        }
        public void LoadRightWeapon()
        {
            if (player.playerInventoryManager.currentRightHandWeapon != null)
            {
                // 切り替え前の武器をremove
                rightHandSlot.UnloadWeapon();
                // 切り替え後の武器をinstantiateして各種マネージャーに設定
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
                player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
            }
        }
        // 左手武器
        public void SwitchLeftWeapon()
        {
            if (!player.IsOwner)
                return;
            if (player.isPerformingAction)
                return;
            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            // 両手持ちなら両手持ちを無効にする

            // leftHandWeaponIndexを一つ進めて、次の可能性のある武器に切り替える。
            player.playerInventoryManager.leftHandWeaponIndex += 1;

            // indexが範囲外になったら0に戻る
            if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 2)
            {
                player.playerInventoryManager.leftHandWeaponIndex = 0;

                // 武器を1個以上持ってるかどうかチェックして持ってた場合一番最初に出てきた武器をfirstWeaponに入れる
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;
                for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.leftHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = firstWeapon.itemID;
                }
                return;
            }

            foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInLeftHandSlots)
            {
                // 次の候補武器がunarmed weaponではない場合
                if (player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];
                    // ネットワーク変数のcurrentRightHandWeaponIDを切り替える
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID;
                    Debug.Log("left" + player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID);
                    return;
                }
            }

            if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <= 2)
            {
                SwitchLeftWeapon();
            }
            else
            {
                // 現状以下が動作することはない
                /*
                // 武器を1個以上持ってるかどうかチェックして持ってた場合一番最初に出てきた武器をfirstWeaponに入れる
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;
                for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                // 所有武器が一個以下だと以下は使われない

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.leftHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = firstWeapon.itemID;
                }
                */
            }
        }
        public void LoadLeftWeapon()
        {
            if (player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                // 切り替え前の武器をremove
                if (leftHandWeaponSlot.currentWeaponModel != null)
                    leftHandWeaponSlot.UnloadWeapon();

                if (leftHandShieldSlot.currentWeaponModel != null)
                    leftHandShieldSlot.UnloadWeapon();

                // 切り替え後の武器をinstantiateして各種マネージャーに設定
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);

                switch (player.playerInventoryManager.currentLeftHandWeapon.weaponModelType)
                {
                    case WeaponModelType.Weapon:
                        leftHandWeaponSlot.LoadWeapon(leftHandWeaponModel);
                        break;
                    case WeaponModelType.Shield:
                        leftHandShieldSlot.LoadWeapon(leftHandWeaponModel);
                        break;
                    default:
                        break;
                }

                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
                player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentLeftHandWeapon.weaponAnimator);
            }
        }
        // damage colliders
        public void OpenDamageCollider()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightWeaponManager.meleeDamageCollider.EnableDamageCollider();
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightHandWeapon.whooshes));
            }
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.meleeDamageCollider.EnableDamageCollider();
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftHandWeapon.whooshes));
            }

            // play whoosh sfx
        }
        public void CloseDamageCollider()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
        }
    }
}
