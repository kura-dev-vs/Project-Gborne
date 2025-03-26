using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ステータス関連
    /// </summary>
    public class CharacterStatsManager : MonoBehaviour
    {
        [HideInInspector] public CharacterManager character;

        [Header("Blocking Absorptions")]
        public float blockingPhysicalAbsorption;
        public float blockingFireAbsorption;
        public float blockingMagicAbsorption;
        public float blockingLightningAbsorption;
        public float blockingHolyAbsorption;
        public float blockingStability;

        [Header("Outfit Absorption Bonus")]
        public float outfitPhysicalDamageAbsorption;
        public float outfitMagicDamageAbsorption;
        public float outfitFireDamageAbsorption;
        public float outfitLightningDamageAbsorption;
        public float outfitHolyDamageAbsorption;

        [Header("Outfit Resistance Bonus")]
        public float outfitImmunity;  // 毒耐性
        public float outfitRobustness;    // 出血凍結耐性
        public float outfitFocus;     // 睡眠、発狂耐性
        public float outfitVitality;  // 呪い耐性

        [Header("Poise: 強靭")]
        public float totalPoiseDamage;              // 現在の総poise値
        public float offensivePoiseBonus;           // 攻撃動作時の武器種による強靭ボーナス値
        public float basePoiseDefense;              // ベースの強靭耐性
        public float defaultPoiseResetTime = 8;     // リセット時間
        public float poiseResetTimer = 0;           // タイマー


        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        protected virtual void Start()
        {
        }
        protected virtual void Update()
        {
            HandlePoiseResetTimer();
            //ResetStaminaRegenTimer();

        }
        /// <summary>
        /// 体力の計算
        /// </summary>
        /// <param name="vitality"></param>
        /// <returns></returns>
        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            health = vitality * 10;

            return Mathf.RoundToInt(health);
        }
        /// <summary>
        /// スタミナの計算
        /// </summary>
        /// <param name="endurance"></param>
        /// <returns></returns>
        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        protected virtual void HandlePoiseResetTimer()
        {
            if (poiseResetTimer > 0)
            {
                poiseResetTimer -= Time.deltaTime;
            }
            else
            {
                totalPoiseDamage = 0;
            }
        }


    }
}
