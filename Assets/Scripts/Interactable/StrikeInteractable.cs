using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class StrikeInteractable : Interactable
    {
        public override void Interact(PlayerManager player)
        {
            if (!player.IsOwner)
                return;

            player.playerInteractionManager.RemoveInteractionFromList(this);

            player.characterCombatManager.AttemptCriticalAttack();
        }
        public void ActiveCollider(bool status)
        {
            interactableCollider.enabled = status;
        }
    }
}
