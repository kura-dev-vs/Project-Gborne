using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 進行状況によってオン/オフを行う必要のあるオブジェクトのspawner
    /// </summary>
    public class NetworkObjectSpawner : MonoBehaviour
    {
        [Header("Object")]
        [SerializeField] GameObject networkGameObject;
        [SerializeField] GameObject instantiatedGameObject;
        private void Awake()
        {

        }
        private void Start()
        {
            WorldObjectManager.instance.SpawnObject(this);
            gameObject.SetActive(false);
        }
        public void AttemptToSpawnCharacter()
        {
            if (networkGameObject != null)
            {
                instantiatedGameObject = Instantiate(networkGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
