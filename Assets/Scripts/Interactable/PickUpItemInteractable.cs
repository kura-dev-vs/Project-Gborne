using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;


namespace RK
{
    /// <summary>
    /// ドロップアイテムが個々に持つインタラクトスクリプト
    /// 
    /// </summary>
    public class PickUpItemInteractable : Interactable
    {

        public ItemPickUpType pickUpType;
        [Header("Item")]
        [SerializeField] Item item;
        [Header("Creature Lost Pick Up")]
        public NetworkVariable<int> itemID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [Header("World Spawn Pick Up")]
        [SerializeField] int worldSpawnInteractableID;
        [SerializeField] bool hasBeenLooted = false;

        [Header("Drop SFX")]
        [SerializeField] AudioClip itemDropSFX;
        private AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();

            audioSource = GetComponent<AudioSource>();
        }
        protected override void Start()
        {
            base.Start();
            if (pickUpType == ItemPickUpType.worldSpawn)
                CheckIfWorldItemWasAlreadyLooted();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            itemID.OnValueChanged += OnItemIDChanged;
            networkPosition.OnValueChanged += OnNetworkPositionChanged;
            if (pickUpType == ItemPickUpType.CharacterDrop)
                audioSource.PlayOneShot(itemDropSFX, 0.1f);

            if (!IsOwner)
            {
                OnItemIDChanged(0, itemID.Value);
                OnNetworkPositionChanged(Vector3.zero, networkPosition.Value);
            }
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            itemID.OnValueChanged -= OnItemIDChanged;
            networkPosition.OnValueChanged -= OnNetworkPositionChanged;
        }
        private void CheckIfWorldItemWasAlreadyLooted()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(worldSpawnInteractableID))
            {
                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, false);
            }

            hasBeenLooted = WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted[worldSpawnInteractableID];

            if (hasBeenLooted)
                gameObject.SetActive(false);
        }
        public override void Interact(PlayerManager player)
        {
            base.Interact(player);
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickUpItemSFX, 0.1f);
            player.inventory.AddItemToInventory(item);
            PlayerUIManager.instance.playerUIHudManager.SetJustGotItemSlotUI(item, 0);

            if (pickUpType == ItemPickUpType.worldSpawn)
            {
                if (WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey((int)worldSpawnInteractableID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Remove(worldSpawnInteractableID);
                }
                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, true);
            }

            DestroyThisNetworkObjectServerRpc();
        }
        protected void OnItemIDChanged(int oldValue, int newValue)
        {
            if (pickUpType != ItemPickUpType.CharacterDrop)
                return;

            item = WorldItemDatabase.instance.GetItemByID(itemID.Value);

        }
        protected void OnNetworkPositionChanged(Vector3 oldPosition, Vector3 newPosition)
        {
            if (pickUpType != ItemPickUpType.CharacterDrop)
                return;

            transform.position = networkPosition.Value;
        }
        public Item GetItemInfo()
        {
            return item;
        }
        [ServerRpc(RequireOwnership = false)]
        protected void DestroyThisNetworkObjectServerRpc()
        {
            if (IsServer)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
