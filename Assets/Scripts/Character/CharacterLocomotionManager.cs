using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// キャラクターの挙動管理
    /// rigidbodyを用いていないので重力再現を行う (将来的にrigidbodyを用いる場合は改修が必要)
    /// </summary>
    public class CharacterLocomotionManager : MonoBehaviour
    {
        [HideInInspector] public CharacterManager character;
        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -30f;
        [SerializeField] LayerMask groundLayer; // 接地レイヤー
        [SerializeField] float groundCheckSphereRadius = 0.1f;  // 接地判定
        [SerializeField] protected Vector3 yVelocity;   // 最終的に掛かるyベクトルの速度
        [SerializeField] protected float groundedYVelocity = -10;   // 接地時にかかり続けるy速度
        [SerializeField] protected float fallStartYVelocity = -1;
        protected bool fallingVelocityHAsBeenSet = false;
        protected float inAirTimer = 0;

        [Header("Flags")]
        public bool isRolling = false;
        public bool canRotate = true;
        public bool canMove = true;

        [Header("Stamina Regeneration")]
        [SerializeField] float staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 0.5f;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();
            if (character.characterNetworkManager.isGrounded.Value)
            {
                // 接地時
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHAsBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                // 空中時
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHAsBeenSet)
                {
                    fallingVelocityHAsBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }
                inAirTimer = inAirTimer + Time.deltaTime;
                if (character.animator)
                    character.animator.SetFloat("InAirTimer", inAirTimer);
                yVelocity.y += gravityForce * Time.deltaTime;
                if (yVelocity.y < -100f)
                {
                    yVelocity.y = -100f;
                }

            }
            // 最終的なyベクトル
            character.characterController.Move(yVelocity * Time.deltaTime);
        }
        /// <summary>
        /// 接地判定
        /// </summary>
        protected void HandleGroundCheck()
        {
            if (character.IsOwner)
            {
                character.characterNetworkManager.isGrounded.Value = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
            }
        }
        // gizmoの表示
        protected void OnDrawGizmosSelected()
        {
            if (character == null)
                return;
            Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
        public void EnableCanRotate()
        {
            canRotate = true;
        }
        public void DisableCanRotate()
        {
            canRotate = false;
        }

        public void EnableIsRipostable()
        {
            if (character.IsOwner)
            {
                character.characterNetworkManager.isRipostable.Value = true;
            }
        }

        /// <summary>
        /// スタミナ消費時、回復を行うまでのタイマーをリセットする用
        /// </summary>
        /// <param name="previousStaminaAmount"></param>
        /// <param name="currentStaminaAmount"></param>
        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
            }
        }

        /// <summary>
        /// スタミナの回復
        /// </summary> 
        public virtual void RegenerateStamina()
        {
            if (character == null)
                return;
            if (!character.IsOwner)
                return;
            if (character.characterNetworkManager.isSprinting.Value)
                return;
            if (character.isPerformingAction)
                return;
            staminaRegenerationTimer += Time.deltaTime;
            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer = staminaTickTimer + Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        if (character.characterNetworkManager.isBlocking.Value)
                        {
                            character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount * 0.1f;
                        }
                        else
                        {
                            character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                        }
                    }
                }
            }
        }
    }
}
