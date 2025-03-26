using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ボスキャラクター以外のエネミーキャラクターに必要なコンポーネント
    /// HPバーUIをUIHudに生成する
    /// 生成基準はプレイヤーからの距離に依存
    /// </summary>
    public class Health_Bar_Instantiation : MonoBehaviour
    {
        private Transform _markerPanel;
        [SerializeField] private ObjectMarker _markerPrefab;
        bool bar_Instantiate;
        [HideInInspector] public bool isActive = false;
        private void Awake()
        {
            _markerPanel = PlayerUIManager.instance.playerUIHudManager.enemyHealthBarParent;
        }
        private void Update()
        {
            if (!PlayerUIManager.instance.playerUIHudManager.gameObject.activeSelf)
                return;
            float dist = Vector3.Distance(PlayerCamera.instance.mainCamera.transform.position, transform.position);

            if (dist < PlayerCamera.instance.targetIsActiveDistance)
            {
                bar_Instantiate = true;
            }
            else
            {
                bar_Instantiate = false;
            }


            if (bar_Instantiate)
            {
                if (!isActive)
                {
                    bar_Instantiate = false;

                    var marker = Instantiate(_markerPrefab, _markerPanel);
                    marker.Initialize(transform, gameObject);
                    marker.gameObject.GetComponent<UI_Enemy_Health_Bar>().EnableEnemyHPBar(marker._target.gameObject.GetComponent<AICharacterManager>());
                    isActive = true;
                }
            }
        }
    }
}
