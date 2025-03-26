using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Block Damage")]
    public class TakeBlockDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; // ダメージの発生元 ("受けた"ではなく"与えた"キャラクター)
        [Header("Damage")]
        public float physicalDamage = 0;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;
        [Header("Final Damage")]
        private int finalDamageDealt = 0;   // 最終的に与えるダメージ
        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;    // SAを崩すための蓄積ダメージ
        [Header("Stamina")]
        public float staminaDamage = 0;
        public float finalStaminaDamage = 0;

        // 追加効果はここで受け取るための変数を用意

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = true;
        public string damageAnimation;
        [Header("Sound FX")]
        public bool willDamageSFX = true;
        public AudioClip elementalDamageSoundSFX;   // 特別な属性の攻撃 (fireとか) が当たったときのsfxを弄りたい場合は指定
        [Header("Direction Damage Taken From")]
        public float angleHitFrom;  // どの角度から攻撃が当たったか
        public Vector3 contactPoint;    // 攻撃が当たった場所
        Transform _markerPanel;
        [SerializeField] private HealthChangeTextController healthChangeText;
        private void Awake()
        {
            if (PlayerUIManager.instance != null)
                _markerPanel = PlayerUIManager.instance.playerUIHudManager.damageTextParent;
        }
        public override void ProcessEffect(CharacterManager character)
        {
            if (character.characterNetworkManager.isInvulnerable.Value)
                return;

            base.ProcessEffect(character);

            if (character.isDead.Value)
                return;

            CalculateDamage(character);
            CalculateStaminaDamage(character);
            PlayDirectionalBasedDamageAnimation(character);

            // 追加効果がある場合はここで計算

            PlayDamageSFX(character);
            PlayDamageVFX(character);

            CheckForGuardBreak(character);
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (characterCausingDamage != null)
            {
                // ダメージ修正をチェック
            }
            // 最終的なダメージを計算する前にダメージの計算方式を考える
            // 防御力, アーマー, バフ, デバフなど

            physicalDamage -= (physicalDamage * (character.characterStatsManager.blockingPhysicalAbsorption / 100));
            fireDamage -= (fireDamage * (character.characterStatsManager.blockingFireAbsorption / 100));
            magicDamage -= (magicDamage * (character.characterStatsManager.blockingMagicAbsorption / 100));
            lightningDamage -= (lightningDamage * (character.characterStatsManager.blockingLightningAbsorption / 100));
            holyDamage -= (holyDamage * (character.characterStatsManager.blockingHolyAbsorption / 100));

            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            if (character.IsOwner)
            {

                character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

                // poise蓄積を計算
            }

            // 最終的なダメージをUIに表示
            var marker = Instantiate(healthChangeText, _markerPanel);
            marker.Initialize(contactPoint, finalDamageDealt.ToString());

            if (character.characterGroup != PlayerCamera.instance.player.characterGroup)
            {
                // ダメージを負ったのが視点側のプレイヤーでない場合のみダメージ表示したいとき
            }
        }
        private void CalculateStaminaDamage(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            Debug.Log("staminaDamage: " + staminaDamage);
            finalStaminaDamage = staminaDamage;

            float staminaDamageAbsorption = finalStaminaDamage * (character.characterStatsManager.blockingStability / 100);
            float staminaDamageAfterAbsorption = finalStaminaDamage - staminaDamageAbsorption;

            Debug.Log("staminaDamageAfterAbsorption: " + staminaDamageAfterAbsorption);

            character.characterNetworkManager.currentStamina.Value -= staminaDamageAfterAbsorption;

        }

        private void CheckForGuardBreak(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            if (character.characterNetworkManager.currentStamina.Value <= 0)
            {
                character.characterAnimatorManager.PlayTargetActionAnimation("Guard_Break_01", true);
                character.characterNetworkManager.isBlocking.Value = false;
            }
        }
        /// <summary>
        /// ダメージが発生したとき用のvfx
        /// </summary>
        /// <param name="character"></param>
        private void PlayDamageVFX(CharacterManager character)
        {
            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);

        }
        /// <summary>
        /// ダメージが発生したとき用のsfx
        /// </summary>
        /// <param name="character"></param>
        private void PlayDamageSFX(CharacterManager character)
        {
            //AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

            //character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
            character.characterSoundFXManager.PlayBlockSoundFX();
            //character.characterSoundFXManager.PlayDamageGruntSoundFX();

        }
        private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner)
                return;
            if (character.isDead.Value)
                return;
            if (character.characterNetworkManager.isToughBody.Value)
                return;

            Debug.Log("poiseDamage: " + poiseDamage);
            DamageIntensity damageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(poiseDamage);

            switch (damageIntensity)
            {
                case DamageIntensity.Ping:
                    damageAnimation = "Block_Ping_01";
                    break;
                case DamageIntensity.Light:
                    damageAnimation = "Block_Light_01";
                    break;
                case DamageIntensity.Medium:
                    damageAnimation = "Block_Medium_01";
                    break;
                case DamageIntensity.Heavy:
                    damageAnimation = "Block_Heavy_01";
                    break;
                case DamageIntensity.Colossal:
                    damageAnimation = "Block_Colossal_01";
                    break;
                default:
                    break;
            }

            // SAが崩れたことを示す
            poiseIsBroken = true;

            // SAが崩れた場合ダメージアニメーションを再生
            if (poiseIsBroken)
            {
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }
        }

    }
}
