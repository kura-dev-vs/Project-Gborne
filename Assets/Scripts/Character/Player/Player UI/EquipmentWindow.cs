using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// PCInfoUIの装備変更UIマネージャー
    /// </summary> 
    public class EquipmentWindow : MonoBehaviour
    {
        [SerializeField] GameObject equipmentInventorySlotPrefab;
        public Item selectingItem;
        public PlayableCharacter selectingItemOwner;
        public EquipmentType selectingItemEquipmentType;
        [SerializeField] EquipmentType currentType;
        public Image selectingItemImage;

        [Header("Weapon Windoe")]
        [SerializeField] GameObject weaponWindow;
        [SerializeField] GameObject possessionWeaponListContent;
        public TextMeshProUGUI selectingWeaponName;
        public Toggle[] toggleWeapons;
        public TextMeshProUGUI leftButtonWeaponText;

        [Header("Outfit Window")]
        [SerializeField] GameObject outfitWindow;
        [SerializeField] GameObject possessionOutfitListContent;
        [SerializeField] Image selectingOutfitImage;
        public TextMeshProUGUI selectingOutfitName;
        [SerializeField] TextMeshProUGUI selectingItemType;
        [SerializeField] TextMeshProUGUI physicalAbs, magicAbs, fireAbs, lightAbs, holyAbs;
        public Toggle[] toggleEquipments;
        public TextMeshProUGUI leftButtonText;

        /// <summary>
        /// 指定されたOutfitTypeと同一のOutfitをInventoryから抽出して選択可能リストに表示する
        /// </summary>
        /// <param name="player"></param>
        /// <param name="outfitType"></param>
        private void ExtractSpecifiedOutfitFromInventory(PlayerManager player, Outfit outfitType)
        {
            List<OutfitItem> outfitInInventory = new List<OutfitItem>();

            for (int i = 0; i < player.inventory.itemsInInventory.Count; i++)
            {
                OutfitItem outfit = player.inventory.itemsInInventory[i] as OutfitItem;

                if (outfit == null)
                    continue;

                if (outfit.outfitType == outfitType)
                    outfitInInventory.Add(outfit);
            }
            if (outfitInInventory.Count <= 0)
            {
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < outfitInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, possessionOutfitListContent.transform);
                UI_EquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(outfitInInventory[i]);

                if (!hasSelectedFirstInventorySlot)
                {
                    ChangedCurrentSelectingItem(outfitInInventory[i]);
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        private void ExtractSpecifiedWeaponFromInventory(PlayerManager player)
        {
            List<WeaponItem> weaponInInventory = new List<WeaponItem>();

            for (int i = 0; i < player.inventory.itemsInInventory.Count; i++)
            {
                WeaponItem weapon = player.inventory.itemsInInventory[i] as WeaponItem;

                if (weapon == null)
                    continue;
                weaponInInventory.Add(weapon);
            }
            if (weaponInInventory.Count <= 0)
            {
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < weaponInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, possessionWeaponListContent.transform);
                UI_EquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(weaponInInventory[i]);

                if (!hasSelectedFirstInventorySlot)
                {
                    ChangedCurrentSelectingItem(weaponInInventory[i]);
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        public void ToggleHat()
        {
            ChangedCurrentSelectingItem(null);
            DestroyChildAll(possessionOutfitListContent.transform);
            currentType = EquipmentType.Hat;
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            InstantiationCurrentOutfitItems(player.inventory.currentEquippedHatByPCID, EquipmentType.Hat);

            ExtractSpecifiedOutfitFromInventory(player, Outfit.Hat);
        }
        public void ToggleJacket()
        {
            ChangedCurrentSelectingItem(null);
            DestroyChildAll(possessionOutfitListContent.transform);
            currentType = EquipmentType.Jacket;
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            InstantiationCurrentOutfitItems(player.inventory.currentEquippedJacketByPCID, EquipmentType.Jacket);

            ExtractSpecifiedOutfitFromInventory(player, Outfit.Jacket);
        }
        public void ToggleTops()
        {
            ChangedCurrentSelectingItem(null);
            DestroyChildAll(possessionOutfitListContent.transform);
            currentType = EquipmentType.Tops;
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            InstantiationCurrentOutfitItems(player.inventory.currentEquippedTopsByPCID, EquipmentType.Tops);

            ExtractSpecifiedOutfitFromInventory(player, Outfit.Tops);
        }
        public void ToggleBottoms()
        {
            ChangedCurrentSelectingItem(null);
            DestroyChildAll(possessionOutfitListContent.transform);
            currentType = EquipmentType.Bottoms;
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            InstantiationCurrentOutfitItems(player.inventory.currentEquippedBottomsByPCID, EquipmentType.Bottoms);

            ExtractSpecifiedOutfitFromInventory(player, Outfit.Bottoms);
        }
        public void ToggleShoes()
        {
            ChangedCurrentSelectingItem(null);
            DestroyChildAll(possessionOutfitListContent.transform);
            currentType = EquipmentType.Shoes;
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            InstantiationCurrentOutfitItems(player.inventory.currentEquippedShoesByPCID, EquipmentType.Shoes);

            ExtractSpecifiedOutfitFromInventory(player, Outfit.Shoes);
        }

        public void ToggleRight()
        {
            ChangedCurrentSelectingItem(null);
            DestroyChildAll(possessionWeaponListContent.transform);
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            currentType = EquipmentType.RightWeapon01;
            InstantiationCurrentWeaponItems(player.inventory.currentLeftByPCID, player.inventory.currentRightByPCID);

            ExtractSpecifiedWeaponFromInventory(player);
        }
        public void ToggleLeft()
        {
            ChangedCurrentSelectingItem(null);
            DestroyChildAll(possessionWeaponListContent.transform);
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            currentType = EquipmentType.LeftWeapon01;
            InstantiationCurrentWeaponItems(player.inventory.currentLeftByPCID, player.inventory.currentRightByPCID);

            ExtractSpecifiedWeaponFromInventory(player);
        }
        private void DestroyChildAll(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
        public void Back()
        {
            CloseUI();
        }
        public void OpenUI(String type)
        {
            GetComponentInParent<PlayerUIPCInfoManager>().pcInfoUIGroup.alpha = 0;
            GetComponentInParent<PlayerUIPCInfoManager>().pcInfoUIGroup.interactable = false;
            GetComponentInParent<PlayerUIPCInfoManager>().pcInfoUIGroup.blocksRaycasts = false;
            selectingItemImage.gameObject.SetActive(true);
            if (type == "weapon")
            {
                weaponWindow.SetActive(true);
            }
            else
            {
                outfitWindow.SetActive(true);
            }
        }
        public void CloseUI()
        {
            DestroyChildAll(possessionOutfitListContent.transform);
            DestroyChildAll(possessionWeaponListContent.transform);
            selectingItem = null;
            selectingItemOwner = null;
            outfitWindow.SetActive(false);
            weaponWindow.SetActive(false);
            selectingItemImage.gameObject.SetActive(false);
            GetComponentInParent<PlayerUIPCInfoManager>().pcInfoUIGroup.alpha = 1;
            GetComponentInParent<PlayerUIPCInfoManager>().pcInfoUIGroup.interactable = true;
            GetComponentInParent<PlayerUIPCInfoManager>().pcInfoUIGroup.blocksRaycasts = true;
        }
        public void ChangedCurrentSelectingItem(Item newItem)
        {
            if (newItem != null)
            {
                selectingItem = newItem;
                selectingItemImage.sprite = newItem.itemIcon;
                selectingOutfitName.SetText(newItem.itemName);
                var item = newItem as OutfitItem;
                if (item != null)
                {
                    physicalAbs.SetText(item.physicalDamageAbsorption.ToString());
                    magicAbs.SetText(item.magicDamageAbsorption.ToString());
                    fireAbs.SetText(item.fireDamageAbsorption.ToString());
                    lightAbs.SetText(item.lightningDamageAbsorption.ToString());
                    holyAbs.SetText(item.holyDamageAbsorption.ToString());
                }
            }
            else
            {
                selectingItem = null;
                selectingItemOwner = null;
                selectingItemImage.sprite = WorldItemDatabase.instance.transparent;
                selectingOutfitName.SetText("");
                physicalAbs.SetText("0");
                magicAbs.SetText("0");
                fireAbs.SetText("0");
                lightAbs.SetText("0");
                holyAbs.SetText("0");
            }
        }

        /// <summary>
        /// RightUIのDownUIの中での左側ボタンの挙動
        /// 下記の誰にも装備されていない場合、既に選択PCが装備している場合、他のPCが装備している場合によって挙動を変化させる。
        /// </summary>
        public void LeftButtonOnClick()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            PlayableCharacter currentPC = PlayerUIManager.instance.playerUIPCInfoManager.currentSelectingPC;
            PlayableCharacter swapedOwnerPC = null;
            if (selectingItem == null)
                return;

            if (selectingItemOwner == null)
            {
                EquipWithSelectingItem(player, currentPC);
            }
            else
            {
                if (selectingItemOwner == PlayerUIManager.instance.playerUIPCInfoManager.currentSelectingPC)
                {
                    UnequipWithSelectingItem(player, currentPC);
                }
                else
                {
                    swapedOwnerPC = selectingItemOwner;
                    SwapWithSelectingItem(player, currentPC);
                }
            }

            PlayerUIManager.instance.playerUIPCInfoManager.ChangedMainDetails();
            EntryManager tempEntry = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>().entry;

            int[] ptID = new int[4];
            Array.Copy(tempEntry.playableCharacterEntryNetworkManager.currentPTIDForSaveAndLoad, ptID, 4);
            if (ptID.Contains(currentPC.pcID))
            {
                tempEntry.playableCharacterInventoryManager.RefreshDeployedPT();
            }
            else if (swapedOwnerPC != null)
            {
                if (ptID.Contains(swapedOwnerPC.pcID))
                {
                    tempEntry.playableCharacterInventoryManager.RefreshDeployedPT();
                }
            }
        }

        /// <summary>
        /// 選択中のアイテムのOwnerがいない場合
        /// 選択中のアイテムを装備する 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="currentPC"></param>
        /// <param name="outfitItem"></param>
        public void EquipWithSelectingItem(PlayerManager player, PlayableCharacter currentPC)
        {
            switch (currentType)
            {
                case EquipmentType.RightWeapon01:
                    if (currentPC.rightWeapon != WorldItemDatabase.instance.unarmedWeapon)
                        player.inventory.AddItemToInventory(currentPC.rightWeapon);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.RightWeapon01);
                    currentPC.rightWeapon = selectingItem as WeaponItem;
                    player.inventory.RemoveItemFromInventory(selectingItem);
                    ToggleRight();
                    break;
                case EquipmentType.LeftWeapon01:
                    if (currentPC.leftWeapon != WorldItemDatabase.instance.unarmedWeapon)
                        player.inventory.AddItemToInventory(currentPC.leftWeapon);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.LeftWeapon01);
                    currentPC.leftWeapon = selectingItem as WeaponItem;
                    player.inventory.RemoveItemFromInventory(selectingItem);
                    ToggleLeft();
                    break;
                case EquipmentType.Hat:
                    if (currentPC.hatOutfit != null)
                        player.inventory.AddItemToInventory(currentPC.hatOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Hat);
                    currentPC.hatOutfit = selectingItem as HatOutfitItem;
                    player.inventory.RemoveItemFromInventory(selectingItem);
                    ToggleHat();
                    break;
                case EquipmentType.Jacket:
                    if (currentPC.jacketOutfit != null)
                        player.inventory.AddItemToInventory(currentPC.jacketOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Jacket);
                    currentPC.jacketOutfit = selectingItem as JacketOutfitItem;
                    player.inventory.RemoveItemFromInventory(selectingItem);
                    ToggleJacket();
                    break;
                case EquipmentType.Tops:
                    if (currentPC.topsOutfit != null)
                        player.inventory.AddItemToInventory(currentPC.topsOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Tops);
                    currentPC.topsOutfit = selectingItem as TopsOutfitItem;
                    player.inventory.RemoveItemFromInventory(selectingItem);
                    ToggleTops();
                    break;
                case EquipmentType.Bottoms:
                    if (currentPC.bottomsOutfit != null)
                        player.inventory.AddItemToInventory(currentPC.bottomsOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Bottoms);
                    currentPC.bottomsOutfit = selectingItem as BottomsOutfitItem;
                    player.inventory.RemoveItemFromInventory(selectingItem);
                    ToggleBottoms();
                    break;
                case EquipmentType.Shoes:
                    if (currentPC.shoesOutfit != null)
                        player.inventory.AddItemToInventory(currentPC.shoesOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Shoes);
                    currentPC.shoesOutfit = selectingItem as ShoesOutfitItem;
                    player.inventory.RemoveItemFromInventory(selectingItem);
                    ToggleShoes();
                    break;
                default:
                    break;
            }
            WorldPlayableCharacterDatabase.instance.SetEquipment(currentPC);
            currentPC.CalculateCharacterStats();
        }

        /// <summary>
        /// 選択中のアイテムのOwnerが選択中のPCであった場合。
        /// 選択中のアイテムを外す
        /// </summary>
        /// <param name="player"></param>
        /// <param name="currentPC"></param>
        /// <param name="outfitItem"></param>
        public void UnequipWithSelectingItem(PlayerManager player, PlayableCharacter currentPC)
        {
            switch (currentType)
            {
                case EquipmentType.RightWeapon01:
                    if (currentPC.rightWeapon == WorldItemDatabase.instance.unarmedWeapon)
                        return;
                    player.inventory.AddItemToInventory(currentPC.rightWeapon);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, WorldItemDatabase.instance.unarmedWeapon, EquipmentType.RightWeapon01);
                    currentPC.rightWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    ToggleRight();
                    break;
                case EquipmentType.LeftWeapon01:
                    if (currentPC.leftWeapon == WorldItemDatabase.instance.unarmedWeapon)
                        return;
                    player.inventory.AddItemToInventory(currentPC.leftWeapon);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, WorldItemDatabase.instance.unarmedWeapon, EquipmentType.LeftWeapon01);
                    currentPC.leftWeapon = WorldItemDatabase.instance.unarmedWeapon;
                    ToggleLeft();
                    break;
                case EquipmentType.Hat:
                    if (currentPC.hatOutfit == null)
                        return;
                    player.inventory.AddItemToInventory(currentPC.hatOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, null, EquipmentType.Hat);
                    currentPC.hatOutfit = null;
                    ToggleHat();
                    break;
                case EquipmentType.Jacket:
                    if (currentPC.jacketOutfit == null)
                        return;
                    player.inventory.AddItemToInventory(currentPC.jacketOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, null, EquipmentType.Jacket);
                    currentPC.jacketOutfit = null;
                    ToggleJacket();
                    break;
                case EquipmentType.Tops:
                    if (currentPC.topsOutfit == null)
                        return;
                    player.inventory.AddItemToInventory(currentPC.topsOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, null, EquipmentType.Tops);
                    currentPC.topsOutfit = null;
                    ToggleTops();
                    break;
                case EquipmentType.Bottoms:
                    if (currentPC.bottomsOutfit == null)
                        return;
                    player.inventory.AddItemToInventory(currentPC.bottomsOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, null, EquipmentType.Bottoms);
                    currentPC.bottomsOutfit = null;
                    ToggleBottoms();
                    break;
                case EquipmentType.Shoes:
                    if (currentPC.shoesOutfit == null)
                        return;
                    player.inventory.AddItemToInventory(currentPC.shoesOutfit);
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, null, EquipmentType.Shoes);
                    currentPC.shoesOutfit = null;
                    ToggleShoes();
                    break;
                default:
                    break;
            }
            WorldPlayableCharacterDatabase.instance.SetEquipment(currentPC);
            currentPC.CalculateCharacterStats();
        }

        /// <summary>
        /// 選択中のアイテムのOwnerが既に他のPCだった場合。
        /// それぞれのアイテムを交換して付け替える。 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="currentPC"></param>
        /// <param name="outfitItem"></param>
        public void SwapWithSelectingItem(PlayerManager player, PlayableCharacter currentPC)
        {
            PlayableCharacter tempCharacter = selectingItemOwner;
            switch (currentType)
            {
                case EquipmentType.RightWeapon01:
                    if (selectingItemEquipmentType == EquipmentType.RightWeapon01)
                    {
                        selectingItemOwner.rightWeapon = currentPC.rightWeapon;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.rightWeapon, EquipmentType.RightWeapon01);
                    }
                    else if (selectingItemEquipmentType == EquipmentType.LeftWeapon01)
                    {
                        selectingItemOwner.leftWeapon = currentPC.rightWeapon;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.rightWeapon, EquipmentType.LeftWeapon01);
                    }

                    currentPC.rightWeapon = selectingItem as WeaponItem;
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.RightWeapon01);
                    ToggleRight();
                    break;
                case EquipmentType.LeftWeapon01:
                    if (selectingItemEquipmentType == EquipmentType.RightWeapon01)
                    {
                        selectingItemOwner.rightWeapon = currentPC.leftWeapon;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.leftWeapon, EquipmentType.RightWeapon01);
                    }
                    else if (selectingItemEquipmentType == EquipmentType.LeftWeapon01)
                    {
                        selectingItemOwner.leftWeapon = currentPC.leftWeapon;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.leftWeapon, EquipmentType.LeftWeapon01);
                    }

                    currentPC.leftWeapon = selectingItem as WeaponItem;
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.LeftWeapon01);
                    ToggleLeft();
                    break;
                case EquipmentType.Hat:
                    if (currentPC.hatOutfit != null)
                    {
                        selectingItemOwner.hatOutfit = currentPC.hatOutfit;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.hatOutfit, EquipmentType.Hat);
                    }
                    else
                    {
                        selectingItemOwner.hatOutfit = null;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, null, EquipmentType.Hat);
                    }
                    currentPC.hatOutfit = selectingItem as HatOutfitItem;
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Hat);
                    ToggleHat();
                    break;
                case EquipmentType.Jacket:
                    if (currentPC.jacketOutfit != null)
                    {
                        selectingItemOwner.jacketOutfit = currentPC.jacketOutfit;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.jacketOutfit, EquipmentType.Jacket);
                    }
                    else
                    {
                        selectingItemOwner.jacketOutfit = null;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, null, EquipmentType.Jacket);
                    }
                    currentPC.jacketOutfit = selectingItem as JacketOutfitItem;
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Jacket);
                    ToggleJacket();
                    break;
                case EquipmentType.Tops:
                    if (currentPC.topsOutfit != null)
                    {
                        selectingItemOwner.topsOutfit = currentPC.topsOutfit;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.topsOutfit, EquipmentType.Tops);
                    }
                    else
                    {
                        selectingItemOwner.topsOutfit = null;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, null, EquipmentType.Tops);
                    }
                    currentPC.topsOutfit = selectingItem as TopsOutfitItem;
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Tops);
                    ToggleTops();
                    break;
                case EquipmentType.Bottoms:
                    if (currentPC.bottomsOutfit != null)
                    {
                        selectingItemOwner.bottomsOutfit = currentPC.bottomsOutfit;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.bottomsOutfit, EquipmentType.Bottoms);
                    }
                    else
                    {
                        selectingItemOwner.bottomsOutfit = null;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, null, EquipmentType.Bottoms);
                    }
                    currentPC.bottomsOutfit = selectingItem as BottomsOutfitItem;
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Bottoms);
                    ToggleBottoms();
                    break;
                case EquipmentType.Shoes:
                    if (currentPC.shoesOutfit != null)
                    {
                        selectingItemOwner.shoesOutfit = currentPC.shoesOutfit;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, currentPC.shoesOutfit, EquipmentType.Shoes);
                    }
                    else
                    {
                        selectingItemOwner.shoesOutfit = null;
                        player.inventory.ChangeCurrentEquipmentList(selectingItemOwner.pcID, null, EquipmentType.Shoes);
                    }
                    currentPC.shoesOutfit = selectingItem as ShoesOutfitItem;
                    player.inventory.ChangeCurrentEquipmentList(currentPC.pcID, selectingItem, EquipmentType.Shoes);
                    ToggleShoes();
                    break;
                default:
                    break;
            }
            WorldPlayableCharacterDatabase.instance.SetEquipment(tempCharacter);
            WorldPlayableCharacterDatabase.instance.SetEquipment(currentPC);
            tempCharacter.CalculateCharacterStats();
            currentPC.CalculateCharacterStats();
        }

        /// <summary>
        /// 現在使用可能なPCが装備しているOutfitを最初に表示する
        /// </summary>
        /// <param name="items"></param>
        private void InstantiationCurrentOutfitItems(IReadOnlyCollection<Item> items, EquipmentType type)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items.ElementAt(i) == null)
                    continue;
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, possessionOutfitListContent.transform);
                UI_EquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(items.ElementAt(i));

                WhoesEquipment(i, equipmentInventorySlot, type);
            }
        }

        private void InstantiationCurrentWeaponItems(IReadOnlyCollection<Item> leftItems, IReadOnlyCollection<Item> rightItems)
        {
            for (int i = 0; i < leftItems.Count; i++)
            {
                if (leftItems.ElementAt(i) == null)
                    continue;
                if (leftItems.ElementAt(i) == WorldItemDatabase.instance.unarmedWeapon)
                    continue;

                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, possessionWeaponListContent.transform);
                UI_EquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(leftItems.ElementAt(i));

                WhoesEquipment(i, equipmentInventorySlot, EquipmentType.LeftWeapon01);
            }

            for (int i = 0; i < rightItems.Count; i++)
            {
                if (rightItems.ElementAt(i) == null)
                    continue;
                if (rightItems.ElementAt(i) == WorldItemDatabase.instance.unarmedWeapon)
                    continue;

                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, possessionWeaponListContent.transform);
                UI_EquipmentInventorySlot equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlot>();
                equipmentInventorySlot.AddItem(rightItems.ElementAt(i));

                WhoesEquipment(i, equipmentInventorySlot, EquipmentType.RightWeapon01);
            }
        }

        private void WhoesEquipment(int i, UI_EquipmentInventorySlot equipmentInventorySlot, EquipmentType type)
        {
            if (i == PlayerUIManager.instance.playerUIPCInfoManager.currentSelectingPC.pcID)
            {
                equipmentInventorySlot.CheckItemWhoHave(PlayerUIManager.instance.playerUIPCInfoManager.currentSelectingPC, type);
            }
            else
            {
                equipmentInventorySlot.CheckItemWhoHave(WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(i), type);
            }
        }
    }
}
