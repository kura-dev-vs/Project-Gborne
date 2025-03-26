using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RK
{
    /// <summary>
    /// 雑魚敵のHPUI
    /// </summary>
    public class UI_Enemy_Health_Bar : UI_StatBar
    {
        [SerializeField] AICharacterManager enemyCharacter;

        public void EnableEnemyHPBar(AICharacterManager enemy)
        {
            enemyCharacter = enemy;
            enemyCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged += OnEnemyHPChanged;
            if (PlayerUIManager.instance.playerUIHudManager.gameObject.activeSelf)
            {
                SetMaxStat(enemyCharacter.characterNetworkManager.maxHealth.Value);
                SetStat(enemyCharacter.aiCharacterNetworkManager.currentHealth.Value);
            }
            //GetComponentInChildren<TextMeshProUGUI>().text = enemyCharacter.characterName;
        }

        private void OnDestroy()
        {
            if (enemyCharacter)
            {
                enemyCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged -= OnEnemyHPChanged;
            }
        }
        private void OnEnemyHPChanged(int oldValue, int newValue)
        {
            SetStat(newValue);

            if (newValue <= 0)
            {
                RemoveHPBar(2.5f);
            }
        }
        public void RemoveHPBar(float time)
        {
            Destroy(gameObject, time);
        }
    }
}
