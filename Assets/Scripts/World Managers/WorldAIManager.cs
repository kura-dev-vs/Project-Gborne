using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Linq;

namespace RK
{
    /// <summary>
    /// ワールドに存在するAIキャラクターを管理するマネージャー
    /// AIキャラクターはそれぞれspawnerを通して出現するため、それらのspawnerとキャラクターオブジェクトの両方を持ったものが登録される
    /// </summary>
    public class WorldAIManager : MonoBehaviour
    {
        public static WorldAIManager instance;

        [Header("Characters")]
        public List<AICharacterSpawner> aiCharacterSpawners;
        public List<AICharacterManager> spawnedInCharacters;

        [Header("Bosses")]
        [SerializeField] List<AIBossCharacterManager> spawnedInBosses;
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

        public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                aiCharacterSpawners.Add(aiCharacterSpawner);
                aiCharacterSpawner.AttemptToSpawnCharacter();
            }
        }
        public void AddCharacterToSpawnedCharactersList(AICharacterManager character)
        {
            if (spawnedInCharacters.Contains(character))
                return;

            spawnedInCharacters.Add(character);

            AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

            if (bossCharacter != null)
            {
                if (spawnedInBosses.Contains(bossCharacter))
                    return;
                spawnedInBosses.Add(bossCharacter);
            }
        }
        public AIBossCharacterManager GetBossCharacterByID(int ID)
        {
            return spawnedInBosses.FirstOrDefault(boss => boss.bossID == ID);
        }
        public void ResetAllCharacters()
        {
            DespawnALLCharacters();

            foreach (var spawner in aiCharacterSpawners)
            {
                spawner.AttemptToSpawnCharacter();
            }
        }
        public void DespawnALLCharacters()
        {
            DisableAllCharacters();
            foreach (var character in spawnedInCharacters)
            {
                if (character == null)
                    continue;
                character.GetComponent<NetworkObject>().Despawn();
            }
            spawnedInCharacters.Clear();
            spawnedInBosses.Clear();
        }
        private void DisableAllCharacters()
        {
            // キャラクタのゲームオブジェクトを無効化し、無効化ステータスをネットワーク上で同期させる。
            // 無効化ステータスがtrueの場合、接続時にクライアントのゲームオブジェクトを無効にする。
            // メモリを節約するために、プレイヤーから遠いキャラクターを無効にすることができる。
            // キャラクターをエリア（area_00_、area_01、area_02）などに分割できる。
        }
    }
}
