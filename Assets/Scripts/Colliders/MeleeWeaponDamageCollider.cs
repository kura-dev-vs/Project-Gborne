using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// characterの持つ近距離武器用コライダー
    /// </summary>
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        private PlayerManager player;
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage; // 武器の持ち主(ダメージ計算の際、相手のダメージ修正値や効果などをチェックするために使用される）
        [Header("Weapon Attack Modifiers")]
        public float light_Attack_01_Modifier;
        public float light_Attack_02_Modifier;
        public float light_Attack_03_Modifier;
        public float light_Attack_04_Modifier;
        public float heavy_Attack_01_Modifier;
        public float heavy_Attack_02_Modifier;
        public float charge_Attack_01_Modifier;
        public float charge_Attack_02_Modifier;
        public float running_Attack_01_Modifier;
        public float rolling_Attack_01_Modifier;
        public float backstep_Attack_01_Modifier;

        protected override void Awake()
        {
            base.Awake();
            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
            }
            damageCollider.enabled = false; // 開始時はdisable

        }
        protected override void Start()
        {
            base.Start();
            if (player == null)
            {
                player = GetComponentInParent<PlayerManager>();
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

                CheckForParry(damageTarget);
                CheckForBlock(damageTarget);
                if (!damageTarget.characterNetworkManager.isInvulnerable.Value)
                    DamageTarget(damageTarget);
            }
        }
        protected override void CheckForParry(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget))
                return;
            if (!characterCausingDamage.characterNetworkManager.isParryable.Value)
                return;
            if (!damageTarget.IsOwner)
                return;
            if (damageTarget.characterNetworkManager.isParrying.Value)
            {
                charactersDamaged.Add(damageTarget);
                damageTarget.characterNetworkManager.NotifyServerOfParryServerRpc(characterCausingDamage.NetworkObjectId);
                // parry_land: パリィ成功時のモーション
                //damageTarget.characterAnimatorManager.PlayTargetActionAnimationInstantly("Parry_Land_01", true);
            }
        }
        protected override void GetBlockingDotValue(CharacterManager damageTarget)
        {
            directionFromAttackToDamageTarget = characterCausingDamage.transform.position - damageTarget.transform.position;
            dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
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

            // 素のダメージ計算後、各種modifierを計算

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
                case AttackType.RunningAttack01:
                    ApplyAttackDamageModifiers(running_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.RollingAttack01:
                    ApplyAttackDamageModifiers(rolling_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.BackstepAttack01:
                    ApplyAttackDamageModifiers(backstep_Attack_01_Modifier, damageEffect);
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
            player.OnAttackHitStop();
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
