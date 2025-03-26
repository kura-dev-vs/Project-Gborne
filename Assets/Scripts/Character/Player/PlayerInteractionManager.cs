using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class PlayerInteractionManager : MonoBehaviour
    {
        PlayerManager player;
        [SerializeField] private List<Interactable> currentInteractableActions;
        public int currentInteractionIndex = 0;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }
        private void Start()
        {
            currentInteractableActions = new List<Interactable>();
        }
        private void FixedUpdate()
        {
            if (!player.IsOwner)
                return;
            // uiメニューが開いておらず、ポップアップ（現在のインタラクションメッセージ）がない場合、インタラクション可能かどうかをチェックする。
            if (!PlayerUIManager.instance.menuWindowIsOpen && !PlayerUIManager.instance.popUpWindowIsOpen)
            {
                CheckForInteractable();
                PlayerUIManager.instance.playerUIHudManager.RefreshSelectingInteraction(currentInteractionIndex);
            }
        }
        private void CheckForInteractable()
        {
            if (currentInteractableActions.Count == 0)
            {
                PlayerUIManager.instance.playerUIHudManager.ResetInteractionMessageSlotUI();
                currentInteractionIndex = 0;
                return;
            }

            if (currentInteractableActions[0] == null)
            {
                currentInteractableActions.RemoveAt(0); // 位置0にある現在のインタラクト可能なアイテムがNULLになった（ゲームから削除された）場合、位置0をリストから削除
                PlayerUIManager.instance.playerUIHudManager.RemoveInteractionMessageSlotUI(0);
                if (currentInteractionIndex > 0)
                    currentInteractionIndex--;
                return;
            }

            // インタラクティブなアクションがあるにもかかわらず、プレーヤーに通知していない場合は、ここで通知
            if (currentInteractableActions[0] != null)
            {
                //PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopUp(currentInteractableActions[0].interactableText);
            }
        }
        private void RefreshInteractionList()
        {
            for (int i = currentInteractableActions.Count - 1; i > -1; i--)
            {
                if (currentInteractableActions[i] == null)
                {
                    currentInteractableActions.RemoveAt(i);
                    PlayerUIManager.instance.playerUIHudManager.RemoveInteractionMessageSlotUI(i);
                    if (i < currentInteractionIndex)
                        currentInteractionIndex--;
                }
            }
        }
        public void AddInteractionToList(Interactable interactableObject)
        {
            RefreshInteractionList();
            if (!currentInteractableActions.Contains(interactableObject))
            {
                currentInteractableActions.Add(interactableObject);
                PlayerUIManager.instance.playerUIHudManager.SetInteractionMessageSlotUI(interactableObject);
            }
        }
        public void RemoveInteractionFromList(Interactable interactableObject)
        {
            if (currentInteractableActions.Contains(interactableObject))
            {
                int num = currentInteractableActions.IndexOf(interactableObject);
                currentInteractableActions.Remove(interactableObject);
                PlayerUIManager.instance.playerUIHudManager.RemoveInteractionMessageSlotUI(num);
                if (num <= currentInteractionIndex && currentInteractionIndex != 0)
                    currentInteractionIndex--;
            }

            RefreshInteractionList();
        }
        public void Interact()
        {
            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindow();
            if (currentInteractableActions.Count == 0)
                return;

            if (currentInteractableActions[currentInteractionIndex] != null)
            {
                currentInteractableActions[currentInteractionIndex].Interact(player);
                RefreshInteractionList();
            }
        }
        public void ChangeCurrentInteractableSelecting(int posOrNeg)
        {
            if (currentInteractableActions.Count > 1)
            {
                if (posOrNeg > 0)
                {
                    if (currentInteractableActions.Count > currentInteractionIndex + 1)
                        currentInteractionIndex++;
                }
                else
                {
                    if (0 < currentInteractionIndex)
                        currentInteractionIndex--;
                }
            }
        }
    }
}
