using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 元素エネルギーを受け取ったときのエフェクト
    /// 現在はvfx, sfxなど未設定
    /// </summary>
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Energy")]
    public class TakeEnergyEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public PlayerManager characterCausingEnergy;
        [Header("Energy")]
        public float baseEnergy;
        [Header("Final Damage")]
        private float finalEnergyDealt = 0;
        [Header("Sound FX")]
        public bool willEnergySFX = true;
        public AudioClip elementalEnergySoundSFX;
        public void ProcessEnergyEffect(EntryManager entry)
        {
            if (entry.player.playerNetworkManager.isInvulnerable.Value)
                return;
            if (entry.player.isDead.Value)
                return;

            CalculateEnergy(entry);
            //PlayEnergySFX(entry);
        }
        private void CalculateEnergy(EntryManager entry)
        {
            if (entry.IsOwner)
            {
                finalEnergyDealt = baseEnergy;
                Debug.Log(finalEnergyDealt);
                entry.player.playerBurstManager.currentEnergy += finalEnergyDealt;
            }
        }
        private void PlayEnergySFX(EntryManager entry)
        {
            AudioClip energySFX = WorldSoundFXManager.instance.energySFX;

            entry.player.characterSoundFXManager.PlaySoundFX(energySFX);
        }
    }
}
