using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ボスキャラクター用のHPと体力UI
    /// </summary> <summary>
    /// 
    /// </summary>
    public class UI_Boss_HP_Bar : UI_StatBar
    {
        [SerializeField] AIBossCharacterManager bossCharacter;
        private void LateUpdate()
        {
            if (bossCharacter == null)
                Destroy(gameObject);
        }

        public void EnableBossHPBar(AIBossCharacterManager boss)
        {
            bossCharacter = boss;
            bossCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged += OnBossHPChanged;
            SetMaxStat(bossCharacter.characterNetworkManager.maxHealth.Value);
            SetStat(bossCharacter.aiCharacterNetworkManager.currentHealth.Value);
            GetComponentInChildren<TextMeshProUGUI>().text = bossCharacter.characterName;
        }

        private void OnDestroy()
        {
            if (bossCharacter)
                bossCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged -= OnBossHPChanged;
        }
        private void OnBossHPChanged(int oldValue, int newValue)
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
