using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// バースト (必殺技) の管理に使用する
    /// 現在開発中
    /// </summary> <summary>
    /// 
    /// </summary>
    public class PlayerBurstManager : MonoBehaviour
    {
        PlayerManager player;
        EntryManager entry;
        PlayableCharacterManager playableCharacterManager;
        public PlayableCharacter playableCharacter;
        [HideInInspector] public PCBurst burst;
        float recastTimeBurst;
        public float currentRecastBurst;
        public bool nowRecasting = false;
        public float rechargeEnergy;
        public float currentEnergy;
        public bool chargedBurst = false;
        private void Start()
        {
            player = GetComponentInParent<PlayerManager>();
            entry = GetComponentInParent<EntryManager>();
            playableCharacterManager = GetComponent<PlayableCharacterManager>();
            playableCharacter = playableCharacterManager.playableCharacter;
            burst = playableCharacter.burst;
            recastTimeBurst = burst.recastTime;
            currentRecastBurst = recastTimeBurst;
            rechargeEnergy = burst.rechargeNeedEnergy;
            currentEnergy = rechargeEnergy / 2;
        }
        private void Update()
        {
            if (currentEnergy >= rechargeEnergy)
            {
                chargedBurst = true;
            }
            else
            {
                chargedBurst = false;
            }
        }
        private void FixedUpdate()
        {
            if (nowRecasting)
            {
                currentRecastBurst -= 1f * Time.deltaTime;
            }
            if (currentRecastBurst <= 0)
            {
                nowRecasting = false;
                currentRecastBurst = recastTimeBurst;
            }
        }
    }
}
