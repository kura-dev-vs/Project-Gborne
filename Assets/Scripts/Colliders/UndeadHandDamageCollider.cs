using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// undeadの手専用ダメージコライダー
    /// </summary>
    public class UndeadHandDamageCollider : DamageCollider
    {
        [SerializeField] AICharacterManager undeadCharacter;
        protected override void Awake()
        {
            base.Awake();
            damageCollider = GetComponent<Collider>();
            undeadCharacter = GetComponentInParent<AICharacterManager>();
        }
        protected override void GetBlockingDotValue(CharacterManager damageTarget)
        {
            directionFromAttackToDamageTarget = undeadCharacter.transform.position - damageTarget.transform.position;
            dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
        }
        protected override void DamageTarget(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget))
                return;
            if (damageTarget.characterGroup == undeadCharacter.characterGroup)
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
            damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

            if (damageTarget.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    undeadCharacter.NetworkObjectId,
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
            undeadCharacter.OnAttackHitStop();
        }
        protected override void CheckForBlock(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget))
                return;

            Vector3 directionFromAttackToDamageTarget = undeadCharacter.transform.position - damageTarget.transform.position;
            float dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);

            if (damageTarget.characterNetworkManager.isBlocking.Value && dotValueFromAttackToDamageTarget > 0.3f)
            {
                charactersDamaged.Add(damageTarget);

                TakeBlockDamageEffect blockEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockDamageEffect);

                blockEffect.physicalDamage = physicalDamage;
                blockEffect.magicDamage = magicDamage;
                blockEffect.fireDamage = fireDamage;
                blockEffect.holyDamage = holyDamage;
                blockEffect.poiseDamage = poiseDamage;
                blockEffect.staminaDamage = stanceDamage;
                blockEffect.staminaDamage = poiseDamage;
                blockEffect.contactPoint = contactPoint;

                damageTarget.characterEffectsManager.ProcessInstantEffect(blockEffect);

                /*
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
                        damageEffect.angleHitFrom,
                        damageEffect.contactPoint.x,
                        damageEffect.contactPoint.y,
                        damageEffect.contactPoint.z);
                }
                */
            }
        }

        protected override void CheckForParry(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget))
                return;
            if (!undeadCharacter.characterNetworkManager.isParryable.Value)
                return;
            if (!damageTarget.IsOwner)
                return;
            if (damageTarget.characterNetworkManager.isParrying.Value)
            {
                charactersDamaged.Add(damageTarget);
                damageTarget.characterNetworkManager.NotifyServerOfParryServerRpc(undeadCharacter.NetworkObjectId);
                Invoke(nameof(DelayMethod), 0.5f);

                // parry_land: パリィ成功時のモーション
                //damageTarget.characterAnimatorManager.PlayTargetActionAnimationInstantly("Parry_Land_01", true);
            }
        }
        private void DelayMethod()
        {
            undeadCharacter.aiCharacterCombatManager.DamageStance(50);
        }
    }
}
