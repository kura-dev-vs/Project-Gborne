using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// durk character　の遠距離攻撃専用コライダー
    /// </summary>
    public class DurkMagicCollider : DamageCollider
    {
        public AIDurkCharacterManager durkCharacterManager;
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                // 遠距離オブジェクトは生成時にcolliderオブジェクトが生成されるため、CharacterManagerのIgnoreMyOwnColliders()で衝突が無効化されない
                // よって自身と遠距離オブジェクトが接触した際はスルーする
                if (damageTarget == durkCharacterManager)
                    return;
                if (damageTarget.characterGroup == durkCharacterManager.characterGroup)
                    return;
                if (damageTarget.characterNetworkManager.isInvulnerable.Value)
                    return;
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                if (charactersDamaged.Contains(damageTarget))
                    return;
                //charactersDamaged.Add(damageTarget);
                if (damageTarget.IsOwner)
                {
                    CheckForBlock(damageTarget);
                    if (!damageTarget.characterNetworkManager.isInvulnerable.Value)
                        DamageTarget(damageTarget);
                }
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
            base.DamageTarget(damageTarget);

            /*
            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.magicDamage = magicDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.angleHitFrom = Vector3.SignedAngle(durkCharacterManager.transform.forward, damageTarget.transform.forward, Vector3.up);

            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
            */
        }
    }
}
