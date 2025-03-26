using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// PCの装備選択画面で現在所持しているOutfitを表示する際に出現するPrefabに付与されるScript
    /// 生成時に個々のOutfitとそのOutfitを装備しているオーナーPCが変数として格納される
    /// </summary>
    public class UI_EquipmentInventorySlot : MonoBehaviour
    {
        public Image itemIcon;
        public Image highlightedIcon;
        [SerializeField] GameObject ownerPCSymbol;
        [HideInInspector] public EquipmentType equipmentType;   // 現在この装備が装備されているとき、どの部位に装備されているか (装備自体の部位を指しているわけではない)
        [SerializeField] Image ownerPCImage;
        [SerializeField] public Item currentItem;
        [SerializeField] TextMeshProUGUI equipText;
        [SerializeField] PlayableCharacter ownerPC = null;

        public void AddItem(Item item)
        {
            if (item == null)
            {
                itemIcon.enabled = false;
                return;
            }

            itemIcon.enabled = true;
            currentItem = item;
            itemIcon.sprite = item.itemIcon;
        }
        public void SelectSlot()
        {
            highlightedIcon.enabled = true;
            EquipmentWindow ep = PlayerUIManager.instance.playerUIPCInfoManager.equipmentWindow;
            ep.selectingItem = currentItem;
            ep.selectingItemImage.sprite = itemIcon.sprite;
            if (currentItem as WeaponItem != null)
            {

            }
            else if (currentItem as OutfitItem != null)
            {
                ep.selectingOutfitName.SetText(currentItem.itemName);
            }

            if (ownerPC == null)
            {
                ep.leftButtonText.SetText("装備する");
                ep.leftButtonWeaponText.SetText("装備する");
                ep.selectingItemOwner = null;
                ep.selectingItemEquipmentType = EquipmentType.Null;
                return;
            }

            if (ownerPC == PlayerUIManager.instance.playerUIPCInfoManager.currentSelectingPC)
            {
                ep.leftButtonText.SetText("外す");
                ep.leftButtonWeaponText.SetText("外す");
            }
            else
            {
                ep.leftButtonText.SetText("切り替え");
                ep.leftButtonWeaponText.SetText("切り替え");
            }
            ep.selectingItemOwner = ownerPC;
            ep.selectingItemEquipmentType = equipmentType;
        }
        public void DeselectedSlot()
        {
            highlightedIcon.enabled = false;
        }
        public void ClickIcon()
        {
            PlayerUIManager.instance.playerUIPCInfoManager.equipmentWindow.ChangedCurrentSelectingItem(currentItem);
        }
        public void CheckItemWhoHave(PlayableCharacter pc, EquipmentType type)
        {
            ownerPC = pc;
            ownerPCSymbol.SetActive(true);
            ownerPCImage.sprite = ownerPC.faceIcon;
            equipmentType = type;
        }
    }
}
