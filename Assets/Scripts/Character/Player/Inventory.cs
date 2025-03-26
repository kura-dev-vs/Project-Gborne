using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 所持しているアイテムの保管場所
    /// アイテムを個々のPCに装備させる場合、 itemsInInventoryから該当するcurrentEquipped{任意}ByPCIDに移動する
    /// </summary> 
    public class Inventory : MonoBehaviour
    {
        [Header("Inventory")]
        public List<Item> itemsInInventory;
        public List<Item> currentRightByPCID, currentLeftByPCID;
        public List<Item> currentEquippedHatByPCID, currentEquippedJacketByPCID, currentEquippedTopsByPCID, currentEquippedBottomsByPCID, currentEquippedShoesByPCID;
        private void Awake()
        {
            for (int i = 0; i < WorldPlayableCharacterDatabase.instance.GetPlayableCharacterCount(); i++)
            {
                currentRightByPCID.Add(null);
                currentLeftByPCID.Add(null);

                currentEquippedHatByPCID.Add(null);
                currentEquippedJacketByPCID.Add(null);
                currentEquippedTopsByPCID.Add(null);
                currentEquippedBottomsByPCID.Add(null);
                currentEquippedShoesByPCID.Add(null);
            }
        }
        public void AddItemToInventory(Item item)
        {
            itemsInInventory.Add(item);
        }
        public void RemoveItemFromInventory(Item item)
        {
            itemsInInventory.Remove(item);

            for (int i = itemsInInventory.Count - 1; i > -1; i--)
            {
                if (itemsInInventory[i] == null)
                {
                    itemsInInventory.RemoveAt(i);
                }
            }
        }
        public void ChangeCurrentEquipmentList(int pcID, Item item, EquipmentType equipmentType)
        {
            PlayableCharacter pc = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(pcID);
            switch (equipmentType)
            {
                case EquipmentType.RightWeapon01:
                    currentRightByPCID[pcID] = item;
                    break;
                case EquipmentType.LeftWeapon01:
                    currentLeftByPCID[pcID] = item;
                    break;

                case EquipmentType.Hat:
                    currentEquippedHatByPCID[pcID] = item;
                    break;
                case EquipmentType.Jacket:
                    currentEquippedJacketByPCID[pcID] = item;
                    break;
                case EquipmentType.Tops:
                    currentEquippedTopsByPCID[pcID] = item;
                    break;
                case EquipmentType.Bottoms:
                    currentEquippedBottomsByPCID[pcID] = item;
                    break;
                case EquipmentType.Shoes:
                    currentEquippedShoesByPCID[pcID] = item;
                    break;
                default:
                    break;
            }
        }
    }
}
