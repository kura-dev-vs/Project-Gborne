using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// キャラクターオブジェクトに必須
    /// 
    /// </summary>
    public class CharacterManager : NetworkBehaviour
    {
        #region Variables

        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        /*[HideInInspector]*/
        public CharacterController characterController;
        public Animator animator;
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterStatsManager characterStatsManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        [Header("Character Group")]
        public CharacterGroup characterGroup;
        public LockOnTransform myLockOnTransform;
        public float HitStopTime = 0.05f;

        [Header("Flags")]
        // キャラクターが新しいアクションを試みるのを止めるために使用する
        // 例えば、ダメージを受け、ダメージアニメーションを開始した場合
        // 気絶した場合、このフラグがtrueになる
        // 新しいアクションを試みる前に、このフラグをチェックすることができる
        public bool isPerformingAction = false;
        public bool canDodge = true;
        public bool canAttack = true;

        [Header("IK")]
        private Vector3 rightFootPosition, leftFootPosition, leftFootIKPosition, rightFootIKPosition;
        private Quaternion leftFootIKRotation, rightFootIKRotation;
        private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

        [Header("Feet Grounder")]
        public bool enableFeetIK = true;
        [Range(0, 2)][SerializeField] private float heightFromGroundRaycast = 0.5f;
        [Range(0, 2)][SerializeField] private float raycastDownDistance = 0.75f;
        [SerializeField] private LayerMask enviromentLayer;
        [SerializeField] public float pelvisOffset = 0f;
        [Range(0, 1)][SerializeField] private float pelvisUpAndDownSpeed = 0.1f;
        [Range(0, 1)][SerializeField] private float feetToIKPositionSpeed = 0.3f;

        public string leftFootAnimVariableName = "LeftFootCurve";
        public string rightFootAnimVariableName = "RightFootCurve";

        public bool useProIKFeature = false;
        public bool showSolverDebug = true;

        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            myLockOnTransform = GetComponentInChildren<LockOnTransform>();
        }
        protected virtual void Start()
        {
            IgnoreMyOwnColliders();
        }
        protected virtual void Update()
        {
            if (animator == null)
                return;
            animator.SetBool("isGrounded", characterNetworkManager.isGrounded.Value);
            // ローカルプレイヤーオブジェクトの場合こちらのposition,rotationをネットワーク変数に更新
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            // それ以外のクライアントからは更新されたネットワーク変数から位置を調整
            else
            {
                // position
                transform.position = Vector3.SmoothDamp
                (transform.position,
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.networkPositionSmoothTime);

                // rotation
                transform.rotation = Quaternion.Slerp
                (transform.rotation,
                characterNetworkManager.networkRotation.Value,
                characterNetworkManager.networkRotationSmoothTime);
            }
            canAttack = canDodge;
        }
        #endregion

        protected virtual void LateUpdate()
        {

        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (animator != null)
            {
                animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);
            }
            characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);

            isDead.OnValueChanged += characterNetworkManager.OnIsDeadChanged;
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;
            characterNetworkManager.isRipostable.OnValueChanged += characterNetworkManager.OnIsRipostableChanged;

        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            isDead.OnValueChanged -= characterNetworkManager.OnIsDeadChanged;
            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
            characterNetworkManager.isRipostable.OnValueChanged -= characterNetworkManager.OnIsRipostableChanged;

        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDamageAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                // 死亡時になんらかのフラグをリセットしたいときはここに書く

                if (!manuallySelectDamageAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }
            }
            // sfxやvfxなどここで書く
            yield return new WaitForSeconds(5);
        }
        public virtual void ReviveCharacter()
        {

        }

        /// <summary>
        /// キャラクターの持つ複数のコライダー同士の衝突判定を無効化する
        /// </summary>
        protected virtual void IgnoreMyOwnColliders()
        {
            Collider characterControllerCollider = GetComponent<Collider>();
            Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
            List<Collider> ignoreColliders = new List<Collider>();

            // モデルの関節ジョイントの位置依存のコライダーをlistでまとめる
            foreach (var collider in damageableCharacterColliders)
            {
                ignoreColliders.Add(collider);
            }

            // character colliderのコライダーもlistに追加
            ignoreColliders.Add(characterControllerCollider);

            // list内のすべてのcollider同士の衝突を無効化
            foreach (var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }
        /// <summary>
        /// 攻撃ヒット時のヒットストップ
        /// Dotweenを使用しているが将来的には別の方法で実装したい
        /// </summary>
        public void OnAttackHitStop()
        {
            // モーションを止める
            animator.speed = 0f;

            var seq = DOTween.Sequence();
            seq.SetDelay(HitStopTime);
            // モーションを再開
            seq.AppendCallback(() => animator.speed = 1f);

        }

        #region FeetGrounding

        /// <summary>
        /// AdjustFeetTarget メソッドを更新し、 Solver Position内の各Foot Positionを見つける
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (enableFeetIK == false)
                return;
            if (animator == null)
                return;

            AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
            AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

            // 位置を見つけてground レイヤーに向けてraycastする
            // 両足のsolver
            FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);
            FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
        }

        public void OnAnimatorIKHub(int layerIndex)
        {
            OnAnimatorIK(layerIndex);
        }
        private void OnAnimatorIK(int layerIndex)
        {
            if (enableFeetIK == false)
                return;
            if (animator == null)
                return;
            if (characterAnimatorManager.applyRootMotion)
                return;

            MovePelvisHeight();

            if (animator.GetBool("isMoving") == true)
                return;

            // 右足
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

            if (useProIKFeature)
            {
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat(rightFootAnimVariableName));
            }

            MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPositionY);

            // 左足
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

            if (useProIKFeature)
            {
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat(leftFootAnimVariableName));
            }

            MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
        }
        #endregion

        #region FeetGroundingMethods
        void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
        {
            Vector3 targetIKPosition = animator.GetIKPosition(foot);

            if (positionIKHolder != Vector3.zero)
            {
                targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
                positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

                float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
                targetIKPosition.y += yVariable;

                lastFootPositionY = yVariable;

                targetIKPosition = transform.TransformPoint(targetIKPosition);

                animator.SetIKRotation(foot, rotationIKHolder);
            }

            animator.SetIKPosition(foot, targetIKPosition);
        }
        private void MovePelvisHeight()
        {
            if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0)
            {
                lastPelvisPositionY = animator.bodyPosition.y;
                return;
            }

            float lOffsetPosition = leftFootIKPosition.y - transform.position.y;
            float rOffsetPosition = rightFootIKPosition.y - transform.position.y;

            float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

            Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOffset;

            newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

            if (animator.GetBool("isMoving") != true)
            {
                animator.bodyPosition = newPelvisPosition;
            }
            lastPelvisPositionY = animator.bodyPosition.y;

        }

        /// <summary>
        /// raycastを介して足の位置を特定する
        /// </summary>
        /// <param name="fromSkyPosition"></param>
        /// <param name="feetIKPositions"></param>
        /// <param name="feetIKRotations"></param>
        private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
        {
            RaycastHit feetOutHit;

            if (showSolverDebug)
                Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);

            if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, enviromentLayer))
            {
                // finding our feet ik positions from the sky position
                feetIKPositions = fromSkyPosition;
                feetIKPositions.y = feetOutHit.point.y + pelvisOffset;
                feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

                return;
            }

            feetIKPositions = Vector3.zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feetPositions"></param>
        /// <param name="foot"></param>
        private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
        {
            feetPositions = animator.GetBoneTransform(foot).position;
            feetPositions.y = transform.position.y + heightFromGroundRaycast;
        }
        #endregion
    }
}
