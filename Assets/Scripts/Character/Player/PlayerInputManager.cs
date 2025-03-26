using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RK
{
    /// <summary>
    /// プレイヤーのinput systemからの入力を一括で管理
    /// 新たに入力を追加するときはbool変数を追加し、onenableでbool変数とinput actionを対応付ける
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        // input controls
        private PlayerControls playerControls;
        // singleton
        public static PlayerInputManager instance;
        // local player
        public PlayerManager player;
        public EntryManager entry;

        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] Vector2 camera_Input;
        public float cameraVertical_Input;
        public float cameraHorizontal_Input;
        [SerializeField] float cameraZoom;

        [Header("LOCK ON INPUT")]
        [SerializeField] bool lockOn_Input;
        [SerializeField] bool lockOn_Left_Input;
        [SerializeField] bool lockOn_Right_Input;
        [SerializeField] float lockOn_Change_Input;
        private Coroutine lockOnCoroutine;

        [Header("PLAYER MOVEMENT INPUT")]
        [SerializeField] Vector2 movementInput;
        public float vertical_Input;
        public float horizontal_Input;
        public float moveAmount;

        [Header("PLAYER ACTION INPUT")]
        [SerializeField] bool dodge_Input = false;
        [SerializeField] bool sprint_Input = false;
        [SerializeField] bool jump_Input = false;
        [SerializeField] bool switch_Right_Weapon_Input = false;
        [SerializeField] bool switch_Left_Weapon_Input = false;
        [SerializeField] bool interaction_Input = false;

        [Header("BUMPER INPUTS")]
        [SerializeField] bool RB_Input = false;
        [SerializeField] bool LB_Input = false;
        [SerializeField] bool Burst_Input = false;

        [Header("TRIGGER INPUTS")]
        [SerializeField] bool RT_Input = false;
        [SerializeField] bool LT_Input = false;
        [SerializeField] bool Hold_RT_Input = false;

        [Header("BUMPER INPUTS")]
        [SerializeField] bool power_Ups_Input = false;

        [Header("QUED INPUTS")]
        [SerializeField] private bool input_Que_Is_Active = false;
        [SerializeField] float default_Que_Input_Timer = 0.35f;
        [SerializeField] float que_Input_Timer = 0;
        [SerializeField] bool que_RB_Input = false;
        [SerializeField] bool que_RT_Input = false;
        [Header("MENU INPUTS")]
        [SerializeField] bool menu_Input = false;
        [SerializeField] bool openCharacterMenuInput = false;
        [SerializeField] bool closeMenuInput = false;

        [SerializeField] bool change1, change2, change3, change4;
        [SerializeField] bool interactionUp = false, interactionDown = false;
        [SerializeField] float interactionScroll;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // when the scene changes, run this logic
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // 指定のシーンでのみplayer controlを有効
            //if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            if (WorldSaveGameManager.instance.CheckBattleScene(newScene.buildIndex))
            {
                instance.enabled = true;
                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            // それ以外では操作を無効
            // プレイヤーの意図しない挙動を止めるため
            else
            {
                instance.enabled = false;
                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }
        /// <summary>
        /// player controlのinput actionとbool 変数を対応付ける
        /// </summary>
        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => camera_Input = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.CameraZoom.performed += i => cameraZoom = i.ReadValue<float>();

                // actions
                playerControls.PlayerActions.Dodge.performed += i => dodge_Input = true;
                playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
                playerControls.PlayerActions.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;
                playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input = true;
                playerControls.PlayerActions.Interact.performed += i => interaction_Input = true;


                // bumpers
                playerControls.PlayerActions.RB.performed += i => RB_Input = true;
                playerControls.PlayerActions.LB.performed += i => LB_Input = true;
                playerControls.PlayerActions.LB.canceled += i => player.playerNetworkManager.isBlocking.Value = false;
                playerControls.PlayerActions.Burst.performed += i => Burst_Input = true;
                // triggers
                playerControls.PlayerActions.RT.performed += i => RT_Input = true;
                playerControls.PlayerActions.LT.performed += i => LT_Input = true;
                playerControls.PlayerActions.HoldRT.performed += i => Hold_RT_Input = true;
                playerControls.PlayerActions.HoldRT.canceled += i => Hold_RT_Input = false;
                // power ups
                playerControls.PlayerActions.PowerUps.performed += i => power_Ups_Input = true;
                playerControls.PlayerActions.PowerUps.canceled += i => power_Ups_Input = false;


                // lock on
                playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                //playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
                //playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;
                playerControls.PlayerActions.SeekLockOnTarget.performed += i => lockOn_Change_Input = i.ReadValue<float>();
                // 長押し時true
                playerControls.PlayerActions.Sprint.performed += i => sprint_Input = true;
                // 離したらfalse
                playerControls.PlayerActions.Sprint.canceled += i => sprint_Input = false;

                // que inputs
                playerControls.PlayerActions.QueRB.performed += i => QueInput(ref que_RB_Input);
                playerControls.PlayerActions.QueRT.performed += i => QueInput(ref que_RT_Input);

                // menu
                playerControls.PlayerActions.Menu.performed += i => menu_Input = true;
                playerControls.PlayerActions.Dodge.performed += i => closeMenuInput = true;
                playerControls.PlayerActions.OpenCharacterMenu.performed += i => openCharacterMenuInput = true;

                playerControls.CharacterChange.Character1.performed += i => change1 = true;
                playerControls.CharacterChange.Character2.performed += i => change2 = true;
                playerControls.CharacterChange.Character3.performed += i => change3 = true;
                playerControls.CharacterChange.Character4.performed += i => change4 = true;

                playerControls.PlayerActions.InteractionScroll.performed += i => interactionScroll = i.ReadValue<float>();
            }

            playerControls.Enable();
        }
        private void OnDestroy()
        {
            // このオブジェクトを破棄したら、このイベントの登録を解除する
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
        // ウィンドウを最小化したり、下げたりした場合は、インプットの調整を停止
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }
        private void Update()
        {
            if (player == null)
                return;
            HandleAllInputs();
        }
        private void HandleAllInputs()
        {
            HandleUIInputs();
            if (PlayerUIManager.instance.playerUICurrentPTManager.uiActivity)
            {
                HandleAllInputsFalse();
                return;
            }
            HandleActionInputs();
        }
        private void HandleUIInputs()
        {
            HandleMenuUIInput();
        }
        private void HandleAllInputsFalse()
        {
            cameraZoom = 0;
            dodge_Input = false;
            jump_Input = false;
            switch_Right_Weapon_Input = false;
            switch_Left_Weapon_Input = false;
            interaction_Input = false;
            RB_Input = false;
            RT_Input = false;
            LB_Input = false;
            LT_Input = false;
            Hold_RT_Input = false;
            lockOn_Input = false;
            sprint_Input = false;
            que_RB_Input = false;
            change1 = false;
            change2 = false;
            change3 = false;
            change4 = false;
            Burst_Input = false;
        }
        private void HandleActionInputs()
        {
            HandleCameraZoomInput();
            HandleLockOnInput();
            HandleLockOnSwitchTargetInput();
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleRBInput();
            HandleLBInput();
            HandleRTInput();
            HandleLTInput();
            HandleChargeRTInput();
            HandleSwitchRightWeaponInput();
            HandleSwitchLeftWeaponInput();
            HandleInteractionInput();

            HandleQuedInputs();

            HandleCharacterChange();
            HandleBurstInput();

            HandlePowerUpsInput();
            HandleCloseUIInput();
            HandleOpenCharacterMenuUIInput();
            HandleInteractionScroll();
        }

        private void HandleLockOnInput()
        {
            // ロックオン状態でターゲットが外れた場合新たなターゲットを探す
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;

                if (player.playerCombatManager.currentTarget.isDead.Value)
                {
                    // 新たなターゲットを探すときに即座に行わずcoroutineで一旦置く
                    lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
                    return;
                }
            }

            // 既ロックオンの場合ロックオンを外す
            if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;
                PlayerCamera.instance.ClearLockOnTargets();
                return;
            }

            // 未ロックオンの場合ターゲットを探す
            if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;

                PlayerCamera.instance.HandleLocatingLockOnTargets();
                if (PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    CharacterManager target = PlayerCamera.instance.nearestLockOnTarget;
                    player.playerCombatManager.SetTarget(target);
                    PlayerCamera.instance.SetLockOnTargets(target);
                    player.playerNetworkManager.isLockedOn.Value = true;
                }
            }
        }
        private void HandleLockOnSwitchTargetInput()
        {
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (lockOn_Change_Input > 0.5f)
                {
                    lockOn_Right_Input = true;
                }
                else if (lockOn_Change_Input < -0.5f)
                {
                    lockOn_Left_Input = true;
                }
                lockOn_Change_Input = 0;
                if (lockOn_Left_Input)
                {
                    lockOn_Left_Input = false;

                    PlayerCamera.instance.GetLockonTargetLeftOrRight("left");
                    if (PlayerCamera.instance.nearestLockOnTarget != null)
                    {
                        CharacterManager target = PlayerCamera.instance.nearestLockOnTarget;
                        player.playerCombatManager.SetTarget(target);
                        PlayerCamera.instance.SetLockOnTargets(target);
                    }
                }

                if (lockOn_Right_Input)
                {
                    lockOn_Right_Input = false;

                    PlayerCamera.instance.GetLockonTargetLeftOrRight("right");
                    if (PlayerCamera.instance.nearestLockOnTarget != null)
                    {
                        CharacterManager target = PlayerCamera.instance.nearestLockOnTarget;
                        player.playerCombatManager.SetTarget(target);
                        PlayerCamera.instance.SetLockOnTargets(target);
                    }

                }
            }
        }
        // movement
        private void HandlePlayerMovementInput()
        {
            vertical_Input = movementInput.y;
            horizontal_Input = movementInput.x;

            // 入力を絶対数に変更
            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

            // 0~0.5は0.5f, 0.5~1.0は1f
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }

            if (player == null)
                return;
            if (moveAmount != 0)
            {
                player.playerNetworkManager.isMoving.Value = true;
                player.animator.SetBool("isMoving", true);
            }
            else
            {
                player.playerNetworkManager.isMoving.Value = false;
                player.animator.SetBool("isMoving", false);
            }
            // ロックオン時, 非ダッシュ時はstrafe動作を使用
            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
            }
        }
        private void HandleCameraMovementInput()
        {
            cameraVertical_Input = camera_Input.y;
            cameraHorizontal_Input = camera_Input.x;
        }
        private void HandleCameraZoomInput()
        {
            if (cameraZoom > 0.8)
            {
                PlayerCamera.instance.GetComponentInChildren<CinemachineUserInputZoom>().ZoomInput(1);
            }
            else if (cameraZoom < -0.8)
            {
                PlayerCamera.instance.GetComponentInChildren<CinemachineUserInputZoom>().ZoomInput(-1);
            }
            cameraZoom = 0;
        }
        // action
        private void HandleDodgeInput()
        {
            if (dodge_Input)
            {
                dodge_Input = false;

                player.playerLocomotionManager.AttemptToPerformeDodge();

            }
        }
        private void HandleSprintInput()
        {
            if (player == null)
                return;
            if (sprint_Input)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }
        private void HandleJumpInput()
        {
            if (jump_Input)
            {
                jump_Input = false;

                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }
        private void HandleRBInput()
        {
            if (RB_Input)
            {
                RB_Input = false;
                if (!player.canAttack)
                    return;

                player.playerNetworkManager.SetCharacterActionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RB_Action, player.playerInventoryManager.currentRightHandWeapon);
            }
        }
        private void HandleLBInput()
        {
            if (LB_Input)
            {
                LB_Input = false;

                if (!player.canAttack)
                    return;

                player.playerNetworkManager.SetCharacterActionHand(false);


                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentLeftHandWeapon.oh_LB_Action, player.playerInventoryManager.currentLeftHandWeapon);
            }
        }
        private void HandleBurstInput()
        {
            if (Burst_Input)
            {
                Burst_Input = false;
                if (!player.canAttack)
                    return;
                player.entry.playableCharacterActionManager.PerformPCBasedBurstAction(player.playerBurstManager.burst.burstAction, player.playerBurstManager);
            }
        }
        private void HandleRTInput()
        {
            if (RT_Input)
            {
                RT_Input = false;
                if (!player.canAttack)
                    return;

                player.playerNetworkManager.SetCharacterActionHand(true);


                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RT_Action, player.playerInventoryManager.currentRightHandWeapon);
            }
        }
        private void HandleLTInput()
        {
            if (LT_Input)
            {
                LT_Input = false;
                if (!player.canAttack)
                    return;

                string direction = "left";
                //player.entry.playableCharacterActionManager.PerformPCBasedSkillAction(player.entry.playableCharacterInventoryManager.currentCharacter.leftSkill.skillAction, player.entry.playableCharacterInventoryManager.currentCharacter.leftSkill);
                player.entry.playableCharacterActionManager.PerformPCBasedSkillAction(player.leftSkillManager.skill.skillAction, player.leftSkillManager, direction);
            }
        }
        private void HandleChargeRTInput()
        {
            // チャージが必要なアクション（攻撃）をしているときだけ、チャージをチェックしたい
            if (player.isPerformingAction)
            {
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isChargingAttack.Value = Hold_RT_Input;
                }
            }
        }
        private void HandleSwitchRightWeaponInput()
        {
            if (switch_Right_Weapon_Input)
            {
                switch_Right_Weapon_Input = false;
                player.playerEquipmentManager.SwitchRightWeapon();
            }
        }
        private void HandleSwitchLeftWeaponInput()
        {
            if (switch_Left_Weapon_Input)
            {
                switch_Left_Weapon_Input = false;
                player.playerEquipmentManager.SwitchLeftWeapon();
            }
        }
        private void HandleInteractionInput()
        {
            if (interaction_Input)
            {
                interaction_Input = false;
                player.playerInteractionManager.Interact();
            }
        }
        private void QueInput(ref bool quedInput)   // 参照を渡すということは、特定のブールを渡すということであり、そのブールの値（真か偽か）を渡すということではない
        {
            // que入力をすべてリセットし、一度に1つだけqueできるようにする
            que_RB_Input = false;
            //que_RT_Input = false;
            //que_LB_Input = false;
            //que_LT_Input = false;

            if (player.isPerformingAction || player.playerNetworkManager.isJumping.Value)
            {
                quedInput = true;
                que_Input_Timer = default_Que_Input_Timer;
                input_Que_Is_Active = true;
            }
        }
        private void ProcessQuedInput()
        {
            if (player.isDead.Value)
                return;
            if (que_RB_Input)
                RB_Input = true;
            if (que_RT_Input)
                RT_Input = true;
        }
        private void HandleQuedInputs()
        {
            if (input_Que_Is_Active)
            {
                // タイマーが0を超えている間、入力を押し続ける
                if (que_Input_Timer > 0)
                {
                    que_Input_Timer -= Time.deltaTime;
                    ProcessQuedInput();
                }
                else
                {
                    // すべてのque入力をリセット
                    que_RB_Input = false;
                    que_RT_Input = false;
                    input_Que_Is_Active = false;
                    que_Input_Timer = 0;
                }
            }
        }
        private void HandleMenuUIInput()
        {
            if (menu_Input)
            {
                menu_Input = false;
                PlayerUIManager.instance.OpenMenuUI();
            }
        }
        private void HandleOpenCharacterMenuUIInput()
        {
            if (openCharacterMenuInput)
            {
                openCharacterMenuInput = false;
                PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindow();
                PlayerUIManager.instance.CloseAllMenuWindows();
                PlayerUIManager.instance.playerUICharacterMenuManager.OpenCharacterMenu();
            }
        }
        private void HandleCloseUIInput()
        {
            if (closeMenuInput)
            {
                closeMenuInput = false;
                if (PlayerUIManager.instance.menuWindowIsOpen)
                {
                    PlayerUIManager.instance.CloseAllMenuWindows();
                }

            }
        }
        private void HandlePowerUpsInput()
        {
            if (!power_Ups_Input)
                return;
            if (power_Ups_Input)
            {
                power_Ups_Input = false;
                if (player.playerNetworkManager.isPowerUps.Value)
                {
                    player.playerNetworkManager.isPowerUps.Value = false;
                    return;
                }
                else
                {
                    player.playerNetworkManager.isPowerUps.Value = true;
                    return;
                }
            }
        }
        private void HandleCharacterChange()
        {
            if (player.isPerformingAction)
            {
                change1 = false;
                change2 = false;
                change3 = false;
                change4 = false;
                return;
            }
            if (change1)
            {
                change1 = false;
                entry.playableCharacterInventoryManager.ChangeCharacter(0);
            }
            if (change2)
            {
                change2 = false;
                entry.playableCharacterInventoryManager.ChangeCharacter(1);
            }
            if (change3)
            {
                change3 = false;
                entry.playableCharacterInventoryManager.ChangeCharacter(2);
            }
            if (change4)
            {
                change4 = false;
                entry.playableCharacterInventoryManager.ChangeCharacter(3);
            }
        }
        private void HandleInteractionScroll()
        {
            if (interactionScroll > 0.8)
            {
                interactionScroll = 0;
                player.playerInteractionManager.ChangeCurrentInteractableSelecting(1);
            }
            else if (interactionScroll < -0.8)
            {
                interactionScroll = 0;
                player.playerInteractionManager.ChangeCurrentInteractableSelecting(-1);
            }
        }
    }
}
