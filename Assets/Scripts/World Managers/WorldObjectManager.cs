using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ワールドに存在する特殊なオブジェクトの管理
    /// ボス戦を意味するfog wall等
    /// ゲームの進行状況によってオン/オフが可能 
    /// </summary>
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager instance;

        [Header("Characters")]
        [SerializeField] List<NetworkObjectSpawner> networkObjectSpawners;
        [SerializeField] List<GameObject> spawnedInCharacters;

        [Header("Fog Walls")]
        public List<FogWallInteractable> fogWalls;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                networkObjectSpawners.Add(networkObjectSpawner);
                networkObjectSpawner.AttemptToSpawnCharacter();
            }
        }
        public void AddFogWallToList(FogWallInteractable fogWall)
        {
            if (!fogWalls.Contains(fogWall))
            {
                fogWalls.Add(fogWall);
            }
        }
        public void RemoveFogWallFromList(FogWallInteractable fogWall)
        {
            if (fogWalls.Contains(fogWall))
            {
                fogWalls.Remove(fogWall);
            }
        }
    }
}
