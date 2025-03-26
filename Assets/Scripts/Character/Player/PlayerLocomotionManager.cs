using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RK
{
    /// <summary>
    /// プレイヤーキャラクターの挙動を管理する
    /// </summary>
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        [HideInInspector] public PlayerManager player;
        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;

        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float sprintingSpeed = 10;
        [SerializeField] float rotationSpeed = 10;
        [SerializeField] int sprintingStaminaCost = 10;

        [Header("Jump")]
        [SerializeField] float jumpHeight = 2;
        [SerializeField] float jumpStaminaCost = 0;
        [SerializeField] float jumpForwardSpeed = 5;
        [SerializeField] float freeFallSpeed = 2;
        private Vector3 jumpDirection;

        [Header("Dodge")]
        private Vector3 rollDirection;
        [SerializeField] float dodgeStaminaCost = 20;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        protected override void Update()
        {
            base.Update();
            if (player.playerAnimatorManager == null)
                return;

            if (player.IsOwner)
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;

                if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
                {
                    player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
                }
                else
                {
                    player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, player.playerNetworkManager.isSprinting.Value);
                }
            }
        }
        public void HandleAllMovement()
        {
            HandleGroundMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
        }
        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.vertical_Input;
            horizontalMovement = PlayerInputManager.instance.horizontal_Input;
            moveAmount = PlayerInputManager.instance.moveAmount;
        }
        private void HandleGroundMovement()
        {
            GetMovementValues();

            if (!player.playerLocomotionManager.canMove)
                return;

            // カメラの方向から、X-Z平面の単位ベクトルを取得
            moveDirection = Vector3.Scale(PlayerCamera.instance.mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized * verticalMovement;
            // 方向キーの入力値とカメラの向きから、移動方向を決定
            moveDirection = moveDirection + Vector3.Scale(PlayerCamera.instance.mainCamera.transform.right, new Vector3(1, 0, 1)).normalized * horizontalMovement;

            moveDirection.y = 0;
            moveDirection.Normalize();
            moveDirection.y = -1;

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    // ダッシュ
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5f)
                {
                    // 歩行
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);

                }
            }
        }
        private void HandleJumpingMovement()
        {
            if (player.playerNetworkManager.isJumping.Value)
            {
                player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }
        private void HandleFreeFallMovement()
        {
            if (!player.playerNetworkManager.isGrounded.Value)
            {
                Vector3 freeFallDirection;
                freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.vertical_Input;
                freeFallDirection = freeFallDirection + PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontal_Input;
                freeFallDirection.y = 0;

                player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
            }
        }
        private void HandleRotation()
        {
            if (player.isDead.Value)
                return;
            if (!player.playerLocomotionManager.canRotate)
                return;
            if (PlayerInputManager.instance.moveAmount == 0)
                return;
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (player.playerNetworkManager.isSprinting.Value || player.playerLocomotionManager.isRolling)
                {
                    Vector3 targetDirection;

                    // カメラの方向から、X-Z平面の単位ベクトルを取得
                    targetDirection = Vector3.Scale(PlayerCamera.instance.mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
                    // 方向キーの入力値とカメラの向きから、移動方向を決定
                    targetDirection = targetDirection * verticalMovement + PlayerCamera.instance.mainCamera.transform.right * horizontalMovement;

                    if (targetDirection == Vector3.zero)
                    {
                        targetDirection = transform.forward;
                    }

                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRotation;
                }
                else
                {
                    if (player.playerCombatManager.currentTarget == null)
                        return;
                    Vector3 targetDirection;
                    targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                    targetDirection.y = 0;
                    targetDirection.Normalize();

                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRotation;
                }
            }
            else
            {
                // カメラの方向から、X-Z平面の単位ベクトルを取得
                targetRotationDirection = Vector3.Scale(PlayerCamera.instance.transform.forward, new Vector3(1, 0, 1)).normalized;
                // 方向キーの入力値とカメラの向きから、移動方向を決定
                targetRotationDirection = targetRotationDirection * verticalMovement + PlayerCamera.instance.transform.right * horizontalMovement;

                if (targetRotationDirection == Vector3.zero)
                {
                    targetRotationDirection = transform.forward;
                }

                Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }
        }
        public void HandleSprinting()
        {
            if (player.isPerformingAction)
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
            // スタミナがないときはスプリントをやめる
            if (player.playerNetworkManager.currentStamina.Value <= 0)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }

            // スプリント入力時、レバーが0.5以上倒れていればスプリント
            if (moveAmount >= 0.5)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            // 0.5以下ではスプリントをやめる
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }
        }

        /// <summary>
        /// 回避関連の挙動
        /// </summary>
        public void AttemptToPerformeDodge()
        {
            /*
            if (player.isPerformingAction)
                return;
                */
            if (!player.canDodge)
                return;
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;
            if (!player.playerNetworkManager.isGrounded.Value)
                return;

            player.playerAnimatorManager.applyRootMotion = false;
            player.playerCombatManager.canComboWithMainHandWeapon = false;

            rollDirection = PlayerCamera.instance.mainCamera.transform.forward * PlayerInputManager.instance.vertical_Input;
            rollDirection += PlayerCamera.instance.mainCamera.transform.right * PlayerInputManager.instance.horizontal_Input;
            rollDirection.y = 0;
            rollDirection.Normalize();

            // レバーが倒れている場合は倒れている角度によってバックフリップとフォワードフリップを使い分ける
            if (PlayerInputManager.instance.moveAmount > 0)
            {
                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    if (PlayerInputManager.instance.vertical_Input < -0.1)
                    {
                        rollDirection = -PlayerCamera.instance.mainCamera.transform.forward * PlayerInputManager.instance.vertical_Input;
                        rollDirection -= PlayerCamera.instance.mainCamera.transform.right * PlayerInputManager.instance.horizontal_Input;
                        rollDirection.y = 0;
                        rollDirection.Normalize();

                        Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                        player.transform.rotation = playerRotation;
                        player.playerAnimatorManager.PlayTargetActionAnimation("Back_Flip_01", true, true);
                    }
                    else
                    {
                        rollDirection = PlayerCamera.instance.mainCamera.transform.forward * PlayerInputManager.instance.vertical_Input;
                        rollDirection += PlayerCamera.instance.mainCamera.transform.right * PlayerInputManager.instance.horizontal_Input;
                        rollDirection.y = 0;
                        rollDirection.Normalize();


                        Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                        player.transform.rotation = playerRotation;
                        player.playerAnimatorManager.PlayTargetActionAnimation("Forward_Flip_01", true, true);
                    }
                }
                else
                {
                    rollDirection = PlayerCamera.instance.mainCamera.transform.forward * PlayerInputManager.instance.vertical_Input;
                    rollDirection += PlayerCamera.instance.mainCamera.transform.right * PlayerInputManager.instance.horizontal_Input;
                    rollDirection.y = 0;
                    rollDirection.Normalize();

                    Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                    player.transform.rotation = playerRotation;
                    player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);

                }

                player.playerLocomotionManager.isRolling = true;
            }
            // レバー入力がない場合は真後ろにバックフリップ
            else
            {
                player.playerAnimatorManager.PlayTargetActionAnimation("Back_Flip_01", true, true);
            }

            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
        }

        /// <summary>
        /// ジャンプ関連
        /// </summary>
        public void AttemptToPerformJump()
        {
            if (player.isPerformingAction)
                return;
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;
            if (player.playerNetworkManager.isJumping.Value)
                return;
            if (!player.playerNetworkManager.isGrounded.Value)
                return;
            player.playerAnimatorManager.PlayTargetActionAnimation("Main_Jump_01", false);

            // 下があるとジャンプ攻撃同時押しの挙動バグが無くなるけどテクニックとして残すのもあり
            //player.isPerformingAction = true;

            player.playerNetworkManager.isJumping.Value = true;
            player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

            //jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.vertical_Input;
            //jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontal_Input;

            // カメラの方向から、X-Z平面の単位ベクトルを取得
            jumpDirection = Vector3.Scale(PlayerCamera.instance.transform.forward, new Vector3(1, 0, 1)).normalized;
            // 方向キーの入力値とカメラの向きから、移動方向を決定
            jumpDirection = jumpDirection * PlayerInputManager.instance.vertical_Input + PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontal_Input;

            jumpDirection.y = 0;

            // walk, run, sprintの三種類でジャンプ時の横移動ベクトルを変更させる
            // 将来的にrigidbodyで挙動調整するなら不要になる
            if (jumpDirection != Vector3.zero)
            {
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1;
                }
                else if (PlayerInputManager.instance.moveAmount > 0.5)
                {
                    jumpDirection *= 0.5f;
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5f)
                {
                    jumpDirection *= 0.25f;
                }
            }

            ApplyJumpingVelocity();
        }
        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }

        public void RotationToTarget()
        {
            if (player.playerCombatManager.currentTarget == null)
                return;
            Vector3 targetDirection;
            targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
            transform.rotation = finalRotation;
        }
    }
}
