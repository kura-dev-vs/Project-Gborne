using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using DG.Tweening;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// DurkCharacterの追加戦闘関連コンポーネント
    /// ダメージコライダー、ダメージ、VFX
    /// Animation eventのメソッドはここに書く
    /// </summary>
    public class AIDurkCombatManager : AICharacterCombatManager
    {
        AIDurkCharacterManager aIDurkManager;
        [Header("Damage Colliders")]
        [SerializeField] DurkHandDamageCollider rightHandDamageCollider;
        [SerializeField] DurkHandDamageCollider leftHandDamageCollider;
        [SerializeField] DurkStompCollider stompCollider;
        [SerializeField] DurkHandDamageCollider roaringCollider;
        [SerializeField] GameObject attack04_magic;
        [SerializeField] Transform durkStompingFoot;
        public float stompAttackAOERadius = 1.5f;
        [Header("Damage")]
        [SerializeField] int baseDamage = 25;
        public int basePoiseDamage = 25;
        [SerializeField] float attack01DamageModifier = 1.0f;
        [SerializeField] float attack02DamageModifier = 1.4f;
        [SerializeField] float attack03DamageModifier = 1.4f;
        public float stompDamage = 25;
        public float roreDamage = 50;
        public float magicDamage = 25;
        public float chaseSpeed = 5f;
        Rigidbody rb;

        [Header("VFX")]
        public GameObject durkImpactVFX;
        public GameObject durkRoreVFX;
        protected override void Awake()
        {
            base.Awake();

            aIDurkManager = GetComponent<AIDurkCharacterManager>();
            rb = GetComponent<Rigidbody>();
        }
        public void SetAttack01Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
            rightHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
            leftHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
        }
        public void SetAttack02Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            rightHandDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
            leftHandDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
        }
        public void SetAttack03Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
            rightHandDamageCollider.poiseDamage = basePoiseDamage * attack03DamageModifier;
            leftHandDamageCollider.poiseDamage = basePoiseDamage * attack03DamageModifier;
        }
        public void OpenRightHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
            rightHandDamageCollider.EnableDamageCollider();
        }
        public void CloseRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }
        public void OpenLeftHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
            leftHandDamageCollider.EnableDamageCollider();
            aIDurkManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(aIDurkManager.durkSoundFXManager.pattaWhooshes));
            ChaseTarget();
        }
        public void CloseLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
            isChase = false;
        }
        public void ActivateDurkStomp()
        {
            stompCollider.poiseDamage = attack03DamageModifier * basePoiseDamage;
            stompCollider.StompAttack();
            isChase = false;
        }
        public override void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            base.PivotTowardsTarget(aiCharacter);
        }
        public void OpenRoaringCollider()
        {
            var rore = Instantiate(durkRoreVFX, transform.position, Quaternion.identity);
            Destroy(rore, 2.5f);
            roaringCollider.physicalDamage = roreDamage;
            roaringCollider.EnableDamageCollider();

        }
        public void CloseRoaringCollider()
        {
            roaringCollider.DisableDamageCollider();
        }
        private void Update()
        {
            if (isChase)
            {
                var target = aiCharacter.aiCharacterCombatManager.currentTarget.transform.position - transform.position;
                transform.position += new Vector3(target.x, 0, target.z) * chaseSpeed * Time.deltaTime;
            }
        }
        public void ChaseTarget()
        {
            isChase = true;
            //var targetPosition = aiCharacter.aiCharacterCombatManager.currentTarget.transform.position;
            //transform.DOMove(targetPosition, chaseSpeed);
        }
        public void ActivateMagic()
        {
            Transform rightHandTransform = rightHandDamageCollider.transform;
            Vector3 bulletDirection;
            GameObject bullet = Instantiate(attack04_magic, rightHandTransform.position, aIDurkManager.transform.rotation);
            DurkMagicCollider collider = bullet.GetComponent<DurkMagicCollider>();
            collider.durkCharacterManager = aIDurkManager;
            collider.magicDamage = magicDamage;

            Vector3 targetPosition = aIDurkManager.durkCombatManager.currentTarget.characterCombatManager.lockOnTransform.position;
            bulletDirection = targetPosition - rightHandTransform.position;
            bullet.GetComponent<HomingController>().SetBulletDirection(bulletDirection);
            bullet.GetComponent<HomingController>().SetTarget(aIDurkManager.durkCombatManager.currentTarget);
        }
    }
}
