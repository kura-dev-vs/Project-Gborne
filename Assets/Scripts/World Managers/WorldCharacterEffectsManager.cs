using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 使用されるエフェクトの登録を行う
    /// ダメージやヒール等々
    /// </summary>
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        public static WorldCharacterEffectsManager instance;
        [Header("VFX")]
        public GameObject bloodSplatterVFX;
        public GameObject criticalBloodSplatterVFX;
        public GameObject healVFX;
        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;
        public TakeBlockDamageEffect takeBlockDamageEffect;
        public TakeCriticalDamageEffect takeCriticalDamageEffect;
        [Header("PowerUps")]
        public PowerUpsEffect powerUpsEffect;
        public AbsorptionEffect absorptionEffect;
        [Header("Instant Effects")]
        [SerializeField] List<InstantCharacterEffect> instantEffects;

        [Header("StaticEffects")]
        [SerializeField] List<StaticCharacterEffect> staticEffects;
        [Header("Heal")]
        public TakeHealEffect takeHealEffect;
        [Header("Energy")]
        public TakeEnergyEffect takeEnergyEffect;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            GenerateEffectIDs();
        }
        private void GenerateEffectIDs()
        {
            for (int i = 0; i < instantEffects.Count; i++)
            {
                instantEffects[i].instantEffectID = i;
            }
            for (int i = 0; i < staticEffects.Count; i++)
            {
                staticEffects[i].staticEffectID = i;
            }
        }

        public StaticCharacterEffect GetStaticEffectByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return staticEffects.FirstOrDefault(staticEffect => staticEffect.staticEffectID == ID);
        }
    }
}
