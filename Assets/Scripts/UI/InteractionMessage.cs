using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    public class InteractionMessage : MonoBehaviour
    {
        [Header("Just Got Item Information")]
        [SerializeField] Image interactionIcon;
        [SerializeField] TextMeshProUGUI interactionText;
        public GameObject selectingFrame;
        public void SetInteractionInformation(Interactable interactable)
        {
            if (interactable.interactableIcon != null)
                interactionIcon.sprite = interactable.interactableIcon;
            interactionText.text = interactable.interactableText;
            PickUpItemInteractable pickUp = interactable as PickUpItemInteractable;
            if (pickUp != null)
            {
                interactionText.text = pickUp.GetItemInfo().itemName;
                interactionIcon.sprite = pickUp.GetItemInfo().itemIcon;
            }
        }
    }
}
