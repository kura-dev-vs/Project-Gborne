using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Critical Damage Effect")]
    public class TakeCriticalDamageEffect : TakeDamageEffect
    {
        public override void ProcessEffect(CharacterManager character)
        {
            if (character.characterNetworkManager.isInvulnerable.Value)
                return;


            if (character.isDead.Value)
                return;

            CalculateDamage(character);

            character.characterCombatManager.pendingCriticalDamage = finalDamageDealt;
            // 追加効果がある場合はここで計算

        }
        protected override void CalculateDamage(CharacterManager character)
        {
            if (characterCausingDamage != null)
            {
                // ダメージ修正をチェック
            }
            // 最終的なダメージを計算する前にダメージの計算方式を考える
            // 防御力, アーマー, バフ, デバフなど
            physicalDamage -= (physicalDamage * (character.characterStatsManager.outfitPhysicalDamageAbsorption / 100));
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }
            if (character.IsOwner)
            {
                // poise蓄積を計算
                character.characterStatsManager.totalPoiseDamage -= poiseDamage;
                character.characterCombatManager.previousPoiseDmageTaken = poiseDamage;

                float remainingPoise = character.characterStatsManager.basePoiseDefense +
                character.characterStatsManager.offensivePoiseBonus +
                character.characterStatsManager.totalPoiseDamage;

                if (remainingPoise <= 0)
                    poiseIsBroken = true;

                character.characterStatsManager.poiseResetTimer = character.characterStatsManager.defaultPoiseResetTime;
            }

            // 最終的なダメージをUIに表示
            //var marker = Instantiate(healthChangeText, _markerPanel);
            //marker.Initialize(character.characterCombatManager.lockOnTransform.position, finalDamageDealt.ToString());

            if (character.characterGroup != PlayerCamera.instance.player.characterGroup)
            {
                // ダメージを負ったのが視点側のプレイヤーでない場合のみダメージ表示したいとき
            }
        }
    }
}
