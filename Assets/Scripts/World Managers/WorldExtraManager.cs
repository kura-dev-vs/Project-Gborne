using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace RK
{
    /// <summary>
    /// Extra Sceneでのみ使用される
    /// 敵が10体単位で無限に生成され続ける。bossCountの区切りでボスエネミーが生成される
    /// </summary>
    public class WorldExtraManager : MonoBehaviour
    {
        public static WorldExtraManager instance;
        [SerializeField] GameObject spawner;
        [SerializeField] GameObject enemyObject;
        [SerializeField] GameObject bossObject;
        [SerializeField] int bossCount = 50;
        public int spawnCountEachWave = 10;
        [SerializeField] float spawnAreaRange = 10.0f;
        [SerializeField] float waveInterval = 3;

        [SerializeField] float timeLimit = 60.0f;
        float currentTime = 0f;
        [SerializeField] GameObject timeUI;
        GameObject time;
        public bool startFlag = false;
        public bool timeCount = false;
        public FogWallInteractable fogwall;
        public int currentWave = 0;
        bool isWaveBuff = false;
        private void Awake()
        {
            // このスクリプトのオブジェクトは一度に一つのみ存在
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            PlayerUIManager.instance.OpenMenuUI();
            time = Instantiate(timeUI, PlayerUIManager.instance.playerUIHudManager.scoreParent);
            timeLimit = WorldSaveGameManager.instance.timeLimit;
            time.GetComponentInChildren<TextMeshProUGUI>().SetText((timeLimit - currentTime).ToString("N2"));
            spawnCountEachWave = WorldSaveGameManager.instance.waveEnemy;
        }
        private void OnDestroy()
        {
            Destroy(time);
        }

        private void Update()
        {
            if (startFlag)
            {
                if (fogwall != null)
                {
                    fogwall.isActive.Value = true;
                }

                if (timeCount)
                    TimeCount();

                if (WorldAIManager.instance.spawnedInCharacters.Count == 0)
                {
                    if (isWaveBuff)
                    {
                        isWaveBuff = false;
                        //wave毎にバフを選択させる
                        PlayerUIManager.instance.playerUIPopUpManager.SendBuffListPopUp();
                    }
                }
            }
        }
        private void TimeCount()
        {
            currentTime += Time.deltaTime;
            time.GetComponentInChildren<TextMeshProUGUI>().SetText((timeLimit - currentTime).ToString("N2"));

            if (timeLimit < currentTime)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendScorePopUp();
                Time.timeScale = 0f;
                currentTime = 0;
            }
        }
        public IEnumerator SpawnedCharacter()
        {
            yield return new WaitForSeconds(waveInterval);

            for (int i = 0; i < spawnCountEachWave; i++)
            {
                Vector3 point;
                if (RandomPoint(transform.position, spawnAreaRange, out point))
                {
                    var spawnmanager = spawner;
                    if ((WorldAIManager.instance.aiCharacterSpawners.Count % bossCount) == 0 && WorldAIManager.instance.aiCharacterSpawners.Count != 0)
                    {
                        spawnmanager.GetComponent<AICharacterSpawner>().SetCharacter(bossObject);
                        Instantiate(spawnmanager, point, Quaternion.identity);
                        bossObject.GetComponent<AIBossCharacterManager>().WakeBoss();
                    }
                    else
                    {
                        spawnmanager.GetComponent<AICharacterSpawner>().SetCharacter(enemyObject);
                        Instantiate(spawnmanager, point, Quaternion.identity);
                    }
                    yield return new WaitForSeconds(0.2f);
                }
            }
            isWaveBuff = true;
        }
        public void RestartExtraScene()
        {
            startFlag = false;
            isWaveBuff = false;
            timeCount = false;

        }

        /// <summary>
        /// navMesh上からランダムに座標を提示する
        /// </summary>
        /// <param name="center"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        /// <returns></returns> 
        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    Debug.Log(hit);
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}
