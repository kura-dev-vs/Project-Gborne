using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Unity.Netcode;

namespace RK
{
    /// <summary>
    /// 戦闘に関する共通コンポーネント
    /// 基本は変数に書いてあるとおり
    /// </summary>
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character;
        [Header("Last Attack Animation Performed")]
        public string lastAttackAnimationPerformed;
        [Header("Previous Poise Damage Taken")]
        public float previousPoiseDmageTaken;
        [Header("Attack Target")]
        public CharacterManager currentTarget;
        [Header("Attack Type")]
        public AttackType currentAttackType;
        [Header("Lock On Transform")]
        public Transform lockOnTransform;
        [Header("Attack Flags")]
        public bool canPerformRollingAttack = false;
        public bool canPerformBackstepAttack = false;
        public bool canBlock = true;
        public bool isChase = false;
        public bool canBeBackstabbed = true;
        [Header("Critical Attack")]
        private Transform riposteReceiverTransform;
        private Transform backstabReceiverTransform;
        [SerializeField] float criticalAttackDistanceCheck = 0.7f;
        public int pendingCriticalDamage;

        protected Transform _markerPanel;
        [SerializeField] protected HealthChangeTextController healthChangeText;
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            _markerPanel = PlayerUIManager.instance.playerUIHudManager.damageTextParent;
        }
        protected virtual void Start()
        {
            if (lockOnTransform == null)
                lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
        }
        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (character.IsOwner)
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else
                {
                    currentTarget = null;
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = 0;
                }
            }
        }

        public virtual void AttemptCriticalAttack()
        {
            if (character.isPerformingAction)
                return;
            if (character.characterNetworkManager.currentStamina.Value <= 0)
                return;
            RaycastHit[] hits = Physics.RaycastAll(character.characterCombatManager.lockOnTransform.position,
                character.transform.TransformDirection(Vector3.forward), criticalAttackDistanceCheck, WorldUtilityManager.instance.GetCharacterLayers());
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                CharacterManager targetCharacter = hit.transform.GetComponent<CharacterManager>();

                if (targetCharacter != null)
                {
                    // 本人の場合スルー
                    if (targetCharacter == character)
                        continue;
                    // 敵味方の判別
                    if (!WorldUtilityManager.instance.CanIDamageThisTarget(character.characterGroup, targetCharacter.characterGroup))
                        continue;
                    // 対象に対する位置と角度
                    Vector3 directionFromCharacterToTarget = character.transform.position - targetCharacter.transform.position;
                    float targetViewableAngle = Vector3.SignedAngle(directionFromCharacterToTarget, targetCharacter.transform.forward, Vector3.up);

                    if (targetCharacter.characterNetworkManager.isRipostable.Value)
                    {
                        if (targetViewableAngle >= -60 && targetViewableAngle <= 60)
                        {
                            AttemptRiposte(hit);
                            return;
                        }
                    }

                    if (targetCharacter.characterCombatManager.canBeBackstabbed)
                    {
                        if (targetViewableAngle <= 180 && targetViewableAngle >= 145)
                        {
                            AttemptBackstab(hit);
                            return;
                        }
                        if (targetViewableAngle >= -180 && targetViewableAngle <= -145)
                        {
                            AttemptBackstab(hit);
                            return;
                        }
                    }
                }
            }
        }
        public virtual void AttemptRiposte(RaycastHit hit)
        {

        }
        public virtual void AttemptBackstab(RaycastHit hit)
        {

        }

        public virtual void ApplyCriticalDamage()
        {
            //character.characterEffectsManager.PlayCriticalBloodSplatterVFX(character.characterCombatManager.lockOnTransform.position);
            //character.characterSoundFXManager.PlayCriticalStrikeSoundFX();

            if (character.IsOwner)
            {
                character.characterNetworkManager.currentHealth.Value -= pendingCriticalDamage;
                if (healthChangeText != null)
                {
                    var marker = Instantiate(healthChangeText, _markerPanel);
                    marker.Initialize(character.characterCombatManager.lockOnTransform.position, pendingCriticalDamage.ToString());
                }
            }
        }

        public IEnumerator ForceMoveEnemyCharacterToRipostePosition(CharacterManager enemyCharacter, Vector3 ripostePosition)
        {
            float timer = 0;

            while (timer < 0.5f)
            {
                timer += Time.deltaTime;

                if (riposteReceiverTransform == null)
                {
                    GameObject riposteTransformObject = new GameObject("Riposte Transform");
                    riposteTransformObject.transform.parent = transform;
                    riposteTransformObject.transform.position = Vector3.zero;
                    riposteReceiverTransform = riposteTransformObject.transform;
                }
                riposteReceiverTransform.localPosition = ripostePosition;
                enemyCharacter.transform.position = riposteReceiverTransform.position;
                transform.rotation = Quaternion.LookRotation(-enemyCharacter.transform.forward);
                yield return null;
            }
        }

        public IEnumerator ForceMoveEnemyCharacterToBackstabPosition(CharacterManager enemyCharacter, Vector3 ripostePosition)
        {
            float timer = 0;

            while (timer < 0.5f)
            {
                timer += Time.deltaTime;

                if (backstabReceiverTransform == null)
                {
                    GameObject backstabTransformObject = new GameObject("Backstab Transform");
                    backstabTransformObject.transform.parent = transform;
                    backstabTransformObject.transform.position = Vector3.zero;
                    backstabReceiverTransform = backstabTransformObject.transform;
                }
                backstabReceiverTransform.localPosition = ripostePosition;
                enemyCharacter.transform.position = backstabReceiverTransform.position;
                transform.rotation = Quaternion.LookRotation(enemyCharacter.transform.forward);
                yield return null;
            }
        }
        public virtual void CloseAllDamageColliders()
        {

        }
    }
}
