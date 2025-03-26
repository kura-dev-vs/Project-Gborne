using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RK
{
    /// <summary>
    /// aicharacter用のcharactermanager
    /// 基本はinspectorで設定する各ステートやmanagerの管理
    /// </summary>
    public class AICharacterManager : CharacterManager
    {
        [Header("Character Name")]
        public string characterName = "";

        [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
        [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
        [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
        [HideInInspector] public AICharacterInventoryManager aICharacterInventoryManager;
        [HideInInspector] public StrikeInteractable strikeInteractable;
        [HideInInspector] public BackstabInteractable backstabInteractable;
        [Header("Navmesh Agent")]
        public NavMeshAgent navMeshAgent;
        [Header("Current State")]
        [SerializeField] protected AIState currentState;
        [Header("States")]
        public IdleState idle;
        public PursueTargetState pursueTarget;
        public CombatStanceState combatStance;
        public AttackState attack;
        public float destroyTime = 5f;
        public int defeatedScore = 10;
        protected override void Awake()
        {
            base.Awake();
            aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
            aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
            aICharacterInventoryManager = GetComponent<AICharacterInventoryManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();

            strikeInteractable = GetComponentInChildren<StrikeInteractable>();
            backstabInteractable = GetComponentInChildren<BackstabInteractable>();
        }
        protected override void Update()
        {
            base.Update();
            aiCharacterCombatManager.HandleActionRecovery(this);
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsOwner)
                ProcessStateMachine();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                idle = Instantiate(idle);
                pursueTarget = Instantiate(pursueTarget);
                combatStance = Instantiate(combatStance);
                attack = Instantiate(attack);
                currentState = idle;
            }
            isDead.OnValueChanged += OnIsDeadChanged;
            aiCharacterNetworkManager.currentHealth.OnValueChanged += aiCharacterNetworkManager.CheckHP;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            isDead.OnValueChanged -= OnIsDeadChanged;
            aiCharacterNetworkManager.currentHealth.OnValueChanged -= aiCharacterNetworkManager.CheckHP;
        }
        private void OnIsDeadChanged(bool oldStatus, bool newStatus)
        {
            if (IsOwner)
            {
                PlayerCamera.instance.player.playerNetworkManager.currentScore.Value += defeatedScore;
            }

            Destroy(gameObject, destroyTime);
            WorldAIManager.instance.spawnedInCharacters.Remove(this);
        }
        // option 01
        private void ProcessStateMachine()
        {
            AIState nextState = currentState?.Tick(this);
            if (nextState != null)
            {
                currentState = nextState;
            }
            // the position/rotation should be reset only after the state machine has processed it's tick
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
            if (aiCharacterCombatManager.currentTarget != null)
            {
                aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
                aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
                aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
            }

            if (navMeshAgent.enabled)
            {
                Vector3 agentDestination = navMeshAgent.destination;
                float remainingDistance = Vector3.Distance(agentDestination, transform.position);

                if (remainingDistance > navMeshAgent.stoppingDistance)
                {
                    aiCharacterNetworkManager.isMoving.Value = true;
                }
                else
                {
                    aiCharacterNetworkManager.isMoving.Value = false;
                }
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false;
            }
        }

    }
}
