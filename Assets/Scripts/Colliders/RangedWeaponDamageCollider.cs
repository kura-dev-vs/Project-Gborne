using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// characterの持つ遠距離武器用コライダー
    /// 基本弾丸と武器本体
    /// </summary>
    public class RangedWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage;
        [Header("Weapon Attack Modifiers")]
        public float light_Attack_01_Modifier;
        public float light_Attack_02_Modifier;
        public float light_Attack_03_Modifier;
        public float light_Attack_04_Modifier;
        public float heavy_Attack_01_Modifier;
        public float heavy_Attack_02_Modifier;
        public float charge_Attack_01_Modifier;
        public float charge_Attack_02_Modifier;

        protected override void Awake()
        {
            base.Awake();
            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
            }
        }
        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                if (damageTarget == characterCausingDamage)
                    return;
                if (damageTarget.characterGroup == characterCausingDamage.characterGroup)
                    return;
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                CheckForBlock(damageTarget);
                if (!damageTarget.characterNetworkManager.isInvulnerable.Value)
                    DamageTarget(damageTarget);
            }

            if (transform.GetComponent<BulletVFXManager>())
            {
                transform.GetComponent<BulletVFXManager>().ImpactVFX();
            }
            else
            {
                Destroy(gameObject, 1f);
            }
        }
        protected override void DamageTarget(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget))
                return;
            charactersDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.stanceDamage = stanceDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack02:
                    ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack03:
                    ApplyAttackDamageModifiers(light_Attack_03_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack04:
                    ApplyAttackDamageModifiers(light_Attack_04_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack02:
                    ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack01:
                    ApplyAttackDamageModifiers(charge_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack02:
                    ApplyAttackDamageModifiers(charge_Attack_02_Modifier, damageEffect);
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
        }
        private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
        {
            damage.physicalDamage *= modifier;
            damage.magicDamage *= modifier;
            damage.fireDamage *= modifier;
            damage.holyDamage *= modifier;
            damage.poiseDamage *= modifier;
            damage.stanceDamage *= modifier;
        }
    }
}
