using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RK
{
    /// <summary>
    ///  ヒール時のエフェクト
    /// 基本的な考え方はダメージ計算と一緒だが計算式が異なるため変数が異なる
    /// </summary>
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Heal")]
    public class TakeHealEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingHeal;
        [Header("Heal")]
        public float baseHeal = 0;
        [Header("Final Heal")]
        private int finalHealDealt = 0;

        [Header("Sound FX")]
        public bool willHealSFX = true;
        public AudioClip elementalHealSoundSFX;
        [Header("Direction Damage Taken From")]
        Transform _markerPanel;
        [SerializeField] private HealthChangeTextController healthChangeText;
        [SerializeField] private Color textColor;
        private void Awake()
        {
            if (PlayerUIManager.instance != null)
                _markerPanel = PlayerUIManager.instance.playerUIHudManager.damageTextParent;
        }

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            if (character.isDead.Value)
                return;

            CalculateHeal(character);

            //PlayHealSFX(character);
            //PlayHealVFX(character);

        }

        private void CalculateHeal(CharacterManager character)
        {
            if (character.IsOwner)
            {
                if (characterCausingHeal != null)
                {

                }

                finalHealDealt = Mathf.RoundToInt(baseHeal);

                if (finalHealDealt <= 0)
                {
                    finalHealDealt = 1;
                }
                character.characterNetworkManager.currentHealth.Value += finalHealDealt;


            }
            var marker = Instantiate(healthChangeText, _markerPanel);
            marker.Initialize(character.characterCombatManager.lockOnTransform.transform.position, "+" + finalHealDealt);
            marker.SetColor(textColor);
        }
        private void PlayHealVFX(CharacterManager character)
        {
            character.characterEffectsManager.PlayHealVFX();
        }
        private void PlayHealSFX(CharacterManager character)
        {
            AudioClip healSFX = WorldSoundFXManager.instance.healSFX;

            character.characterSoundFXManager.PlaySoundFX(healSFX);
        }
    }
}
