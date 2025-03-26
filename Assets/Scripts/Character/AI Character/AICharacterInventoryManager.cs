using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace RK
{
    public class AICharacterInventoryManager : CharacterInventoryManager
    {
        AICharacterManager aICharacter;
        [Header("Loot Chance")]
        public int dropItemChance = 10;
        [SerializeField] Item[] droppableItems;
        [SerializeField] int[] droppableItemMinAmount, droppableItemMaxAmount;
        protected override void Awake()
        {
            base.Awake();
            aICharacter = GetComponent<AICharacterManager>();
        }

        public void DropItem()
        {
            if (!aICharacter.IsOwner)
                return;
            if (droppableItems.Length == 0)
                return;
            bool willDropItem = false;
            int itemChanceRoll = Random.Range(0, 100);
            if (itemChanceRoll <= dropItemChance)
                willDropItem = true;

            if (!willDropItem)
                return;
            Item generatedItem = droppableItems[Random.Range(0, droppableItems.Length)];

            if (generatedItem == null)
                return;

            GameObject itemPickUpInteractableGameObject = Instantiate(WorldItemDatabase.instance.pickUpItemPrefab);
            itemPickUpInteractableGameObject.gameObject.transform.position = transform.position;
            PickUpItemInteractable pickUpInteractable = itemPickUpInteractableGameObject.GetComponent<PickUpItemInteractable>();
            itemPickUpInteractableGameObject.GetComponent<NetworkObject>().Spawn();
            pickUpInteractable.itemID.Value = generatedItem.itemID;
            pickUpInteractable.networkPosition.Value = transform.position;
            itemPickUpInteractableGameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.Range(2, 5), Random.Range(-1, 1)), ForceMode.Impulse);
        }

    }
}
