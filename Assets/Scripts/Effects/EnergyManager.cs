using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// スキル等元素攻撃をヒットさせたときに生成される元素エネルギーの挙動や付与するエネルギー関連コンポーネント
    /// 生成時はランダムな方向に力が加わって勢いよく投げ出され、periodTimeで設定した時間に発生元のキャラクターの元へ回収される
    /// 発生元のキャラクターが元素エネルギーに触れると爆発を打つためのエネルギーがチャージされる
    /// </summary>
    public class EnergyManager : MonoBehaviour
    {
        [SerializeField] EntryManager characterCausing;
        List<EntryManager> charactersEnergied = new List<EntryManager>();
        Vector3 offset = new Vector3(0f, 1f, 0f);
        Vector3 velocity, position;
        [SerializeField] float periodTime = 1.5f;
        float currentTime;
        [SerializeField] float firstVelocityRangeMaximum = 5f;
        [Header("Take Energy")]
        public float baseEnergy = 10f;
        private void Start()
        {
            var x_force = Random.Range(-firstVelocityRangeMaximum, firstVelocityRangeMaximum);
            var z_force = Random.Range(-firstVelocityRangeMaximum, firstVelocityRangeMaximum);
            var y_force = Random.Range(1.0f, firstVelocityRangeMaximum);
            velocity = new Vector3(x_force, y_force, z_force);
            currentTime = periodTime;
            Destroy(gameObject, periodTime + 2f);
            position = transform.position;
        }
        private void Update()
        {
            if (characterCausing != null)
            {
                Homing();
            }
        }
        public void InstantiationEnergy(EntryManager entry)
        {
            characterCausing = entry;
        }
        private void OnTriggerEnter(Collider other)
        {
            EntryManager entry = other.GetComponentInParent<EntryManager>();
            if (entry != null)
            {
                if (entry != characterCausing)
                    return;
                if (charactersEnergied.Contains(entry))
                    return;
                charactersEnergied.Add(entry);
                TakeEnergyEffect energyEffect = Instantiate(WorldCharacterEffectsManager.instance.takeEnergyEffect);
                energyEffect.baseEnergy = baseEnergy;

                energyEffect.ProcessEnergyEffect(entry);
                Destroy(gameObject);
            }

        }
        private void Homing()
        {
            var acceleration = Vector3.zero;

            var diff = characterCausing.transform.position + offset - position;
            acceleration += (diff - velocity * currentTime) * 2f / (currentTime * currentTime);

            currentTime -= Time.deltaTime;
            if (currentTime < 0f)
            {
                currentTime = 0.1f;
            }

            velocity += acceleration * Time.deltaTime;
            position += velocity * Time.deltaTime;
            transform.position = position;
        }
    }
}
