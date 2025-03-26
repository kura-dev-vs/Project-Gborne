using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RK
{
    /// <summary>
    /// aicharacterのspawner
    /// このコンポーネントを持つオブジェクトを配置するとstartした際にaicharacterもinstantiateされる
    /// </summary>
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject characterGameObject;
        [SerializeField] GameObject instantiatedGameObject;
        [SerializeField] GameObject spawnVFX;
        private void Awake()
        {

        }
        private void Start()
        {
            WorldAIManager.instance.SpawnCharacter(this);
            gameObject.SetActive(false);
        }
        public void AttemptToSpawnCharacter()
        {
            if (characterGameObject != null)
            {
                instantiatedGameObject = Instantiate(characterGameObject);

                NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
                int vertexIndex = Random.Range(0, triangulation.vertices.Length);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2f, 0))
                {
                    var navMesh = instantiatedGameObject.GetComponentInChildren<NavMeshAgent>();
                    navMesh.Warp(hit.position);
                    navMesh.enabled = true;
                }

                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
                WorldAIManager.instance.AddCharacterToSpawnedCharactersList(instantiatedGameObject.GetComponent<AICharacterManager>());
                if (spawnVFX != null)
                {
                    var vfx = Instantiate(spawnVFX, instantiatedGameObject.transform.position, Quaternion.identity);
                    Destroy(vfx, 2.0f);
                }

            }
        }
        public void SetCharacter(GameObject characterObject)
        {
            characterGameObject = characterObject;
        }
    }
}
