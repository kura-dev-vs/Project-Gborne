using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace RK
{
    /// <summary>
    /// キャラクターに影響するエフェクトの処理を行うコンポーネント
    /// ・ダメージやヒールなど実数値に即座に影響が出るもの
    /// ・時限式の効果 (毒とか)
    /// ・バフ・デバフ関連
    /// </summary>
    public class CharacterEffectsManager : MonoBehaviour
    {
        [HideInInspector] public CharacterManager character;
        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;
        [SerializeField] GameObject criticalBloodSplatterVFX;
        [SerializeField] GameObject healVFX;
        [Header("Static Effects")]
        public List<StaticCharacterEffect> staticEffects = new List<StaticCharacterEffect>();
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }
        /// <summary>
        /// 被ダメージのVFX
        /// 専用のものを追加する場合はbloodSplatterVFXに設定しておき、なかったら共通のものが実行される
        /// </summary>
        /// <param name="contactPoint"></param>
        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }

        public void PlayCriticalBloodSplatterVFX(Vector3 contactPoint)
        {
            if (criticalBloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(criticalBloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.criticalBloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }

        public void AddStaticEffect(StaticCharacterEffect effect)
        {
            // if you want to sync effects across network, if you are the owner launch a server rpc here to process the effect on all other clients

            // 1. add a static effect to the character
            staticEffects.Add(effect);

            // 2. process its effect
            effect.ProcessStaticEffect(character);

            // 3. check for null entries in your list and remove them
            for (int i = staticEffects.Count - 1; i > -1; i--)
            {
                if (staticEffects[i] == null)
                    staticEffects.RemoveAt(i);
            }
        }
        public void RemoveStaticEffect(int effectID)
        {
            // if you want to sync effects across network, if you are the owner launch a server rpc here to process the effect on all other clients
            StaticCharacterEffect effect;

            for (int i = 0; i < staticEffects.Count; i++)
            {
                if (staticEffects[i] != null)
                {
                    if (staticEffects[i].staticEffectID == effectID)
                    {
                        effect = staticEffects[i];
                        // 1. remove static effect from character
                        effect.RemoveStaticEffect(character);
                        // 2. remove static effect from list
                        staticEffects.Remove(effect);
                    }
                }
            }
            // 3. check for null entries in your list and remove them
            for (int i = staticEffects.Count - 1; i > -1; i--)
            {
                if (staticEffects[i] == null)
                    staticEffects.RemoveAt(i);
            }
        }
        public void RemoveAllStaticEffects()
        {
            StaticCharacterEffect effect;
            for (int i = 0; i < staticEffects.Count; i++)
            {
                if (staticEffects[i] != null)
                {
                    effect = staticEffects[i];
                    // 1. remove static effect from character
                    effect.RemoveStaticEffect(character);
                }
            }
            staticEffects.Clear();
        }

        /// <summary>
        /// 被ヒールのVFX
        /// 専用のものを追加する場合はhealVFXに設定しておき、なかったら共通のものが実行される
        /// </summary>
        /// <param name="contactPoint"></param>
        public void PlayHealVFX()
        {
            if (bloodSplatterVFX != null)
            {
                GameObject heal = Instantiate(healVFX);
            }
            else
            {
                GameObject heal = Instantiate(WorldCharacterEffectsManager.instance.healVFX);
            }
        }

    }
}
