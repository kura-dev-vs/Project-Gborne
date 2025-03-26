using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace RK
{
    /// <summary>
    /// aicharacter用のcombatmanager
    /// ターゲットの索敵範囲や攻撃間隔等を用意
    /// </summary>
    public class AICharacterCombatManager : CharacterCombatManager
    {
        protected AICharacterManager aiCharacter;
        [Header("Action Recovery")]
        public float actionRecoveryTimer = 0;
        [Header("Pivot")]
        public bool enablePivot = true;
        [Header("Target Imformation")]
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetsDirection;

        [Header("Detection")]
        [SerializeField] float detectionRadius = 15;
        public float minimumFOV = -35;
        public float maximumFOV = 35;
        [Header("Attack Rotation Speed")]
        public float attackRotationSpeed = 25;

        [Header("Stance Settings")]
        public float maxStance;
        public float currentStance;
        //public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        //public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [SerializeField] float stanceRegenerationPersecond = 15;    // 1秒毎の回復量
        [SerializeField] bool ignoreStanceBreak = false;

        [Header("Stance Timer")]
        [SerializeField] float stanceRegenerationTimer = 0;
        private float stanceTickTimer = 0;
        [SerializeField] float defaultTimerUntilStanceRegenerationBeings = 15;

        protected override void Awake()
        {
            base.Awake();
            aiCharacter = GetComponent<AICharacterManager>();
        }
        private void FixedUpdate()
        {
            HandleStanceBreak();
        }
        private void HandleStanceBreak()
        {
            if (!aiCharacter.IsOwner)
                return;
            if (aiCharacter.isDead.Value)
                return;

            if (stanceRegenerationTimer > 0)
            {
                stanceRegenerationTimer -= Time.deltaTime;
            }
            else
            {
                stanceRegenerationTimer = 0;

                if (currentStance < maxStance)
                {
                    stanceTickTimer += Time.deltaTime;

                    if (stanceTickTimer >= 1)
                    {
                        stanceTickTimer = 0;
                        currentStance += stanceRegenerationPersecond;
                    }
                }
                else
                {
                    currentStance = maxStance;
                }
            }

            if (currentStance <= 0)
            {
                DamageIntensity previousDamageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(previousPoiseDmageTaken);

                if (previousDamageIntensity == DamageIntensity.Colossal)
                {
                    currentStance = 1;
                    return;
                }
                currentStance = maxStance;
                if (ignoreStanceBreak)
                    return;
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Stance_Break_01", true);
                //aiCharacter.criticalInteractable.ActiveCollider();
            }
        }

        public void DamageStance(int stanceDamage)
        {
            stanceRegenerationTimer = defaultTimerUntilStanceRegenerationBeings;

            currentStance -= stanceDamage;
        }

        // <summary>
        /// targetの索敵. 先にspherecollliderで候補を見つけてから正面の敵をターゲットに入れる
        /// </summary>
        /// <param name="aiCharacter"></param>
        public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;
            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();
                if (targetCharacter == null)
                    continue;
                if (targetCharacter == aiCharacter)
                    continue;
                if (targetCharacter.isDead.Value)
                    continue;
                // ターゲットとcharacterGroupを比較して別グループだった場合候補に入れる
                if (WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
                {
                    //　候補の中から正面の敵をターゲットにする
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);
                    if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                    {
                        // linecastが障害物に邪魔されていないか
                        if (aiCharacter.characterCombatManager.lockOnTransform.position == null)
                        {
                            Debug.Log("CAN'T FIND LOCKON TRANSFORM");
                            return;
                        }
                        if (Physics.Linecast(
                            aiCharacter.characterCombatManager.lockOnTransform.position,
                            targetCharacter.characterCombatManager.lockOnTransform.position,
                            WorldUtilityManager.instance.GetEnviroLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                            Debug.Log("Blocked");
                        }
                        else
                        {
                            this.targetsDirection = targetCharacter.transform.position - transform.position;
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, this.targetsDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            if (enablePivot)
                                PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// ターゲットの方へ回頭するアニメーション
        /// </summary>
        /// <param name="aiCharacter"></param>
        public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return;
            if (viewableAngle >= 90 && viewableAngle <= 180)
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("zombie_turn_R", true);
            else if (viewableAngle <= -90 && viewableAngle >= -180)
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("zombie_turn_L", true);
        }
        /// <summary>
        /// 上記ほどじゃない場合にrotationで正面に捉え続ける
        /// </summary>
        /// <param name="aiCharacter"></param>
        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
        /// <summary>
        /// アタックアニメーション中にターゲットへ回転させる目的. animatorのイベントでcanrotateをオンにすると有効になる
        /// </summary>
        /// <param name="aiCharacter"></param>
        public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null)
                return;
            if (!aiCharacter.aiCharacterLocomotionManager.canRotate)
                return;
            if (!aiCharacter.isPerformingAction)
                return;
            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
        }
        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTimer > 0)
            {
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    }
}
