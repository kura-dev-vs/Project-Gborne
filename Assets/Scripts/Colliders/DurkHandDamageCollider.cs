using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// durk enemy専用のハンドダメージコライダー
    /// </summary>
    public class DurkHandDamageCollider : DamageCollider
    {
        [SerializeField] AIBossCharacterManager bossCharacter;
        protected override void Awake()
        {
            base.Awake();
            damageCollider = GetComponent<Collider>();
            bossCharacter = GetComponentInParent<AIBossCharacterManager>();
        }
        protected override void CheckForParry(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget))
                return;
            if (!bossCharacter.characterNetworkManager.isParryable.Value)
                return;
            if (!damageTarget.IsOwner)
                return;
            if (damageTarget.characterNetworkManager.isParrying.Value)
            {
                charactersDamaged.Add(damageTarget);
                damageTarget.characterNetworkManager.NotifyServerOfParryServerRpc(bossCharacter.NetworkObjectId);
                bossCharacter.aiCharacterCombatManager.DamageStance(50);
                // parry_land: パリィ成功時のモーション
                //damageTarget.characterAnimatorManager.PlayTargetActionAnimationInstantly("Parry_Land_01", true);
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
            damageEffect.angleHitFrom = Vector3.SignedAngle(bossCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

            // ローカルプレイヤー側から見たときに当たった場合のみダメージを受ける
            if (damageTarget.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    bossCharacter.NetworkObjectId,
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
            bossCharacter.OnAttackHitStop();
        }
    }
}
