using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// スタンプ攻撃専用
    /// </summary>
    public class DurkStompCollider : DamageCollider
    {
        [SerializeField] AIDurkCharacterManager durkCharacterManager;
        protected override void Awake()
        {
            base.Awake();

            durkCharacterManager = GetComponentInParent<AIDurkCharacterManager>();
        }
        protected override void Start()
        {
            base.Start();
            physicalDamage = durkCharacterManager.durkCombatManager.stompDamage;
        }
        public void StompAttack()
        {
            GameObject stompVFX = Instantiate(durkCharacterManager.durkCombatManager.durkImpactVFX, transform);
            Collider[] colliders = Physics.OverlapSphere(transform.position, durkCharacterManager.durkCombatManager.stompAttackAOERadius, WorldUtilityManager.instance.GetCharacterLayers());
            List<CharacterManager> charactersDamaged = new List<CharacterManager>();

            foreach (var collider in colliders)
            {
                CharacterManager damageTarget = collider.GetComponentInParent<CharacterManager>();

                if (damageTarget != null)
                {
                    if (charactersDamaged.Contains(damageTarget))
                        continue;
                    if (damageTarget == durkCharacterManager)
                        continue;
                    charactersDamaged.Add(damageTarget);

                    if (damageTarget.IsOwner)
                    {
                        Debug.Log("poiseDamage : collider :" + poiseDamage);
                        CheckForBlock(damageTarget);
                        if (!damageTarget.characterNetworkManager.isInvulnerable.Value)
                            DamageTarget(damageTarget);
                        /*
                        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                        damageEffect.physicalDamage = durkCharacterManager.durkCombatManager.stompDamage;
                        damageEffect.poiseDamage = durkCharacterManager.durkCombatManager.stompDamage;

                        character.characterEffectsManager.ProcessInstantEffect(damageEffect);
                        */
                    }
                }
            }
        }
    }
}
