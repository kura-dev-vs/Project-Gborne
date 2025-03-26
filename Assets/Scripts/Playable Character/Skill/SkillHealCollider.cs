using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ヒールスキルのコライダー
    /// </summary>
    public class SkillHealCollider : SkillColliderBase
    {
        [Header("Collider")]
        [SerializeField] protected Collider healCollider;
        [Header("Heal")]
        public float baseHeal = 0;
        [Header("Characters Healed")]
        [SerializeField] protected List<CharacterManager> charactersHealed = new List<CharacterManager>();
        [Header("Healing Character")]
        public CharacterManager characterCausingHeal;   // ヒールを行ったキャラクター
        [Header("Weapon Attack Modifiers")]
        public float light_Attack_01_Modifier;

        protected virtual void Awake()
        {
            if (healCollider == null)
            {
                healCollider = GetComponent<Collider>();
            }
        }
        private void Update()
        {
            // durationTimeの間隔でリストをリセットし、回復を行う
            if (resetListPermission == false)
                return;
            if (durationTime != 0)
            {
                timeCount += Time.deltaTime;
                if (timeCount > durationTime)
                {
                    ClearCharacterList();
                    timeCount = 0;
                }
            }
        }
        protected virtual void OnTriggerStay(Collider other)
        {
            CharacterManager healTarget = other.GetComponentInParent<CharacterManager>();

            if (healTarget != null)
            {
                if (healTarget.isDead.Value)
                    return;
                // キャラクターと別グループのキャラクターがコライダーに触れても回復させない
                if (healTarget.characterGroup != characterCausingHeal.characterGroup)
                    return;

                HealTarget(healTarget);
            }
        }
        protected virtual void HealTarget(CharacterManager healTarget)
        {
            if (charactersHealed.Contains(healTarget))
                return;
            charactersHealed.Add(healTarget);

            TakeHealEffect healEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealEffect);
            healEffect.baseHeal = baseHeal;

            switch (characterCausingHeal.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackHealModifiers(light_Attack_01_Modifier, healEffect);
                    break;
                default:
                    break;
            }

            if (characterCausingHeal.IsOwner)
            {
                healTarget.characterNetworkManager.NotifyTheServerOfCharacterHealServerRpc(
                    healTarget.NetworkObjectId,
                    characterCausingHeal.NetworkObjectId,
                    healEffect.baseHeal);
            }
        }
        private void ApplyAttackHealModifiers(float modifier, TakeHealEffect heal)
        {
            heal.baseHeal *= modifier;
        }
        public void ClearCharacterList()
        {
            charactersHealed.Clear();
        }
    }
}
