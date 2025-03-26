using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class SkillRangedCollider : SkillColliderBase
    {
        [Header("Collider")]
        [SerializeField] protected Collider damageCollider;
        [Header("Damage")]
        //public float baseDamage = 0;
        [Header("Characters Damaged")]
        [SerializeField] protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();
        [Header("Damaging Character")]
        public EntryManager characterCausingDamage; // ダメージの発生元 ("受けた"ではなく"与えた"キャラクター)
        [Header("Weapon Attack Modifiers")]
        public float skillModifier;
        Vector3 contactPoint;
        public int energy = 2;  // 発生するエネルギー

        protected virtual void Awake()
        {
            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
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
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                if (damageTarget.isDead.Value)
                    return;
                if (damageTarget.characterGroup == characterCausingDamage.player.characterGroup)
                    return;
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                DamageTarget(damageTarget);
            }
            if (transform.GetComponent<BulletVFXManager>())
            {
                transform.GetComponent<BulletVFXManager>().ImpactVFX();
            }
            else
            {
                if (triggerForDestroy)
                    Destroy(gameObject, 1f);
            }
        }
        protected virtual void DamageTarget(CharacterManager damageTarget)
        {

            if (charactersDamaged.Contains(damageTarget))
                return;
            charactersDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

            switch (characterCausingDamage.player.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageModifiers(skillModifier, damageEffect);
                    break;
                default:
                    break;
            }

            if (characterCausingDamage.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    characterCausingDamage.NetworkObjectId,
                    damageEffect.physicalDamage,
                    damageEffect.magicDamage,
                    damageEffect.fireDamage,
                    damageEffect.holyDamage,
                    damageEffect.poiseDamage,
                    damageEffect.stanceDamage,
                    damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x,
                    damageEffect.contactPoint.y,
                    damageEffect.contactPoint.z);
            }
            GenerateEnergy(energy, characterCausingDamage, contactPoint);
        }
        private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
        {
            damage.physicalDamage *= modifier;
            damage.magicDamage *= modifier;
            damage.fireDamage *= modifier;
            damage.holyDamage *= modifier;
            damage.poiseDamage *= modifier;
        }
        public void ClearCharacterList()
        {
            charactersDamaged.Clear();
        }
    }
}
