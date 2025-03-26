using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.IO.LowLevel.Unsafe;

namespace RK
{
    /// <summary>
    /// キャラクターのアニメーション関連
    /// アニメーションで呼び出される共通イベントもここに入れる
    /// </summary>
    public class CharacterAnimatorManager : MonoBehaviour
    {
        [HideInInspector] public CharacterManager character;
        int vertical;
        int horizontal;
        [Header("Flags")]
        // animationのルートモーションを適用するか
        public bool applyRootMotion = false;
        [Header("Damage Animations")]
        public string lastDamageAnimationPlayed;
        // ping hit 
        [SerializeField] private string hit_Forward_Ping_01 = "hit_Forward_Ping_01";
        [SerializeField] private string hit_Forward_Ping_02 = "hit_Forward_Ping_02";
        [SerializeField] private string hit_Backward_Ping_01 = "hit_Backward_Ping_01";
        [SerializeField] private string hit_Backward_Ping_02 = "hit_Backward_Ping_02";
        [SerializeField] private string hit_Left_Ping_01 = "hit_Left_Ping_01";
        [SerializeField] private string hit_Left_Ping_02 = "hit_Left_Ping_02";
        [SerializeField] private string hit_Right_Ping_01 = "hit_Right_Ping_01";
        [SerializeField] private string hit_Right_Ping_02 = "hit_Right_Ping_02";

        public List<string> forward_Ping_Damage = new List<string>();
        public List<string> backward_Ping_Damage = new List<string>();
        public List<string> left_Ping_Damage = new List<string>();
        public List<string> right_Ping_Damage = new List<string>();
        string footStepState;

        // medium hit
        [SerializeField] private string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
        [SerializeField] private string hit_Forward_Medium_02 = "hit_Forward_Medium_02";
        [SerializeField] private string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
        [SerializeField] private string hit_Backward_Medium_02 = "hit_Backward_Medium_02";
        [SerializeField] private string hit_Left_Medium_01 = "hit_Left_Medium_01";
        [SerializeField] private string hit_Left_Medium_02 = "hit_Left_Medium_02";
        [SerializeField] private string hit_Right_Medium_01 = "hit_Right_Medium_01";
        [SerializeField] private string hit_Right_Medium_02 = "hit_Right_Medium_02";

        public List<string> forward_Medium_Damage = new List<string>();
        public List<string> backward_Medium_Damage = new List<string>();
        public List<string> left_Medium_Damage = new List<string>();
        public List<string> right_Medium_Damage = new List<string>();


        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }
        protected virtual void Start()
        {
            forward_Ping_Damage.Add(hit_Forward_Ping_01);
            forward_Ping_Damage.Add(hit_Forward_Ping_02);

            backward_Ping_Damage.Add(hit_Backward_Ping_01);
            backward_Ping_Damage.Add(hit_Backward_Ping_02);

            left_Ping_Damage.Add(hit_Left_Ping_01);
            left_Ping_Damage.Add(hit_Left_Ping_02);

            right_Ping_Damage.Add(hit_Right_Ping_01);
            right_Ping_Damage.Add(hit_Right_Ping_02);


            forward_Medium_Damage.Add(hit_Forward_Medium_01);
            forward_Medium_Damage.Add(hit_Forward_Medium_02);

            backward_Medium_Damage.Add(hit_Backward_Medium_01);
            backward_Medium_Damage.Add(hit_Backward_Medium_02);

            left_Medium_Damage.Add(hit_Left_Medium_01);
            left_Medium_Damage.Add(hit_Left_Medium_02);

            right_Medium_Damage.Add(hit_Right_Medium_01);
            right_Medium_Damage.Add(hit_Right_Medium_02);
        }
        /// <summary>
        /// ダメージアニメーションのリストが渡され、その中からランダムにモーションを渡す
        /// </summary>
        /// <param name="animationList"></param>
        /// <returns></returns>
        public string GetRandomAnimationFromList(List<string> animationList)
        {
            List<string> finalList = new List<string>();
            foreach (var item in animationList)
            {
                finalList.Add(item);
            }

            finalList.Remove(lastDamageAnimationPlayed);

            for (int i = finalList.Count - 1; i > -1; i--)
            {
                if (finalList[i] == null)
                {
                    finalList.RemoveAt(i);
                }
            }
            int randomValue = Random.Range(0, finalList.Count);
            return finalList[randomValue];
        }
        /// <summary>
        /// 移動時の入力を丸める
        /// </summary>
        /// <param name="horizontalMovement"></param>
        /// <param name="verticalMovement"></param>
        /// <param name="isSprinting"></param>
        public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            float snappedHorizontal = SnappedHV(horizontalMovement);
            float snappedVertical = SnappedHV(verticalMovement);

            if (isSprinting)
            {
                snappedVertical = 2;
            }
            character.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
            character.animator.SetFloat("InputAmount", Mathf.Abs(snappedHorizontal) + Mathf.Abs(snappedVertical), 0.1f, Time.deltaTime);

        }
        private float SnappedHV(float movement)
        {
            float snapped;
            if (movement > 0 && movement <= 0.5f)
            {
                snapped = 0.5f;
            }
            else if (movement > 0.5f && movement <= 1)
            {
                snapped = 1;
            }
            else if (movement < 0 && movement >= -0.5f)
            {
                snapped = -0.5f;
            }
            else if (movement < -0.5f && movement >= -1)
            {
                snapped = -1;
            }
            else
            {
                snapped = 0;
            }
            return snapped;
        }

        /// <summary>
        /// 特定のアクションアニメーションの実行メソッド
        /// </summary>
        /// <param name="targetAnimation">行うアニメーション名</param>
        /// <param name="isPerformingAction">アクションを実行中かどうか。アクション実行中に他のアクションを行わせたくない挙動の再現用</param>
        /// <param name="applyRootMotion">ルートモーションの適用</param>
        /// <param name="canRotate"></param>
        /// <param name="canMove"></param>
        public virtual void PlayTargetActionAnimation(
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
            character.isPerformingAction = isPerformingAction;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;
            character.canDodge = !isPerformingAction;

            // アニメーションの再生をサーバーに伝え他のクライアントも同じアニメーションを行う
            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
        public virtual void PlayTargetActionAnimationInstantly(
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.Play(targetAnimation);
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
            character.isPerformingAction = isPerformingAction;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;
            character.canDodge = !isPerformingAction;

            // アニメーションの再生をサーバーに伝え他のクライアントも同じアニメーションを行う
            character.characterNetworkManager.NotifyTheServerOfInstantActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
        public virtual void PlayTargetAttackActionAnimation(
            WeaponItem weapon,
            AttackType attackType,
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = true,
            bool canMove = false)
        {
            // attack typeを記録
            // コンボの場合最後に行ったアニメーションヵら次のアニメーションを選ぶので最後に行ったモーションを記録する
            // 今後カウンターダメージやパリィ再現のためフラグを追加するかも
            character.characterCombatManager.currentAttackType = attackType;

            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
            UpdateAnimatorController(weapon.weaponAnimator);
            character.isPerformingAction = isPerformingAction;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;
            character.canDodge = !isPerformingAction;

            // アニメーションの再生をサーバーに伝え他のクライアントも同じアニメーションを行う
            character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }


        public virtual void PlayTargetSkillActionAnimation(
            PlayableCharacter pc,
            AttackType attackType,
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
            UpdateAnimatorController(pc.pcAnimator);
            character.isPerformingAction = isPerformingAction;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;
            character.canDodge = !isPerformingAction;

            // アニメーションの再生をサーバーに伝え他のクライアントも同じアニメーションを行う
            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
        public void UpdateAnimatorController(AnimatorOverrideController weaponController)
        {
            character.animator.runtimeAnimatorController = weaponController;
        }

        public virtual void EnableCanDoCombo()
        {

        }
        public virtual void DisableCanDoCombo()
        {

        }
        public virtual void EableCanDodge()
        {

        }
        public void EnableIsInvulnerable()
        {
            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = true;
        }
        public void DisableIsInvulnerable()
        {
            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = false;
        }
        public void EnableIsParrying()
        {
            if (character.IsOwner)
                character.characterNetworkManager.isParrying.Value = true;
        }
        public void DisableIsParrying()
        {
            if (character.IsOwner)
                character.characterNetworkManager.isParrying.Value = false;
        }
        public void EnableCanDoRollingAttack()
        {
            character.characterCombatManager.canPerformRollingAttack = true;
        }
        public void DisableCanDoRollingAttack()
        {
            character.characterCombatManager.canPerformRollingAttack = false;
        }
        public void EnableCanDoBackstepAttack()
        {
            character.characterCombatManager.canPerformBackstepAttack = true;
        }
        public void DisableCanDoBackstepAttack()
        {
            character.characterCombatManager.canPerformBackstepAttack = false;
        }
        public virtual void InstantiationBullet()
        {

        }

        private void OnAnimatorIK(int layerIndex)
        {
            character.OnAnimatorIKHub(layerIndex);
        }

        private void PlayFootStepRightSoundFX()
        {
            if (!character.characterNetworkManager.isGrounded.Value)
                return;
            if (footStepState == "Right")
                return;
            footStepState = "Right";

            if (!character.isPerformingAction)
                character.characterSoundFXManager.PlayFootStepSoundFX();
        }
        private void PlayFootStepLeftSoundFX()
        {
            if (!character.characterNetworkManager.isGrounded.Value)
                return;
            if (footStepState == "Left")
                return;
            footStepState = "Left";

            if (!character.isPerformingAction)
                character.characterSoundFXManager.PlayFootStepSoundFX();
        }
    }
}
