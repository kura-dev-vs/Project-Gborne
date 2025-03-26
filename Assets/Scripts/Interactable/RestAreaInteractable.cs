using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 休息ポイントのインタラクトスクリプト
    /// </summary>
    public class RestAreaInteractable : Interactable
    {
        [Header("Rest Point Info")]
        [SerializeField] int restPointID;
        public NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [Header("VFX")]
        [SerializeField] GameObject[] activatedParticle;

        [Header("Interaction Text")]
        [SerializeField] string unactivatedInteractionText = "Activation";
        [SerializeField] string activatedInteractionText = "Rest";
        protected override void Start()
        {
            base.Start();

            if (IsOwner)
            {
                if (WorldSaveGameManager.instance.currentCharacterData.restPoint.ContainsKey(restPointID))
                {
                    isActivated.Value = WorldSaveGameManager.instance.currentCharacterData.restPoint[restPointID];
                }
                else
                {
                    isActivated.Value = false;
                }
            }

            if (isActivated.Value)
            {
                interactableText = activatedInteractionText;
            }
            else
            {
                interactableText = unactivatedInteractionText;
            }

        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // マルチプレイに参加した場合、参加時にonchange関数を実行する
            if (!IsOwner)
                OnIsActivatedChanged(false, isActivated.Value);

            isActivated.OnValueChanged += OnIsActivatedChanged;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            isActivated.OnValueChanged -= OnIsActivatedChanged;
        }
        private void ActivationRextPoint(PlayerManager player)
        {
            isActivated.Value = true;

            if (WorldSaveGameManager.instance.currentCharacterData.restPoint.ContainsKey(restPointID))
                WorldSaveGameManager.instance.currentCharacterData.restPoint.Remove(restPointID);

            WorldSaveGameManager.instance.currentCharacterData.restPoint.Add(restPointID, true);

            player.playerAnimatorManager.PlayTargetActionAnimation("Activate_Rest_Point_01", true);

            PlayerUIManager.instance.playerUIPopUpManager.SendRestPointPopUp("Activation");
            StartCoroutine(WaitForAnimationAndPopUpThenRestoreCollider());
        }
        private void RestAtRestPoint(PlayerManager player)
        {
            Debug.Log("resting");
            interactableCollider.enabled = true;
            player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value;
            player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;

            WorldAIManager.instance.ResetAllCharacters();
            WorldSaveGameManager.instance.SaveGame();
        }
        private IEnumerator WaitForAnimationAndPopUpThenRestoreCollider()
        {
            yield return new WaitForSeconds(2);
            interactableCollider.enabled = true;
        }
        private void OnIsActivatedChanged(bool oldStatus, bool newStatus)
        {
            if (isActivated.Value)
            {
                // 起動中のエフェクト
                for (int i = 0; i < activatedParticle.Length; i++)
                {
                    activatedParticle[i].SetActive(true);
                }

                interactableText = activatedInteractionText;

            }
            else
            {
                interactableText = unactivatedInteractionText;
            }
        }
        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (!isActivated.Value)
            {
                ActivationRextPoint(player);
            }
            else
            {
                RestAtRestPoint(player);
            }
        }
    }
}
