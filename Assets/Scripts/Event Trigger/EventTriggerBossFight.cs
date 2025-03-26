using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// トリガーに触れた場合に該当のボスをWakeさせる
    /// </summary>
    public class EventTriggerBossFight : MonoBehaviour
    {
        [SerializeField] int bossID;

        private void OnTriggerEnter(Collider other)
        {
            AIBossCharacterManager boss = WorldAIManager.instance.GetBossCharacterByID(bossID);
            if (boss.hasBeenDefeated.Value)
                return;

            if (other.GetComponent<PlayerManager>() == null)
                return;

            if (boss != null)
            {
                boss.WakeBoss();
            }
        }
    }
}
