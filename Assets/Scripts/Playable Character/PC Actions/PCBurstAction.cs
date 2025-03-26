using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 元素爆発でのアクション関連
    /// </summary>
    [CreateAssetMenu(menuName = "Characters/PlayableCharacters/Actions/Burst Action")]
    public class PCBurstAction : ScriptableObject
    {
        public int burstActionID;
        [SerializeField] string burstAnimation = "BurstAnimation";

        public virtual void AttemptToPerformBurst(PlayerManager playerPerformingAction, PlayerBurstManager pcBurst)
        {
            if (!playerPerformingAction.IsOwner)
                return;
            if (pcBurst.nowRecasting)
                return;
            if (!pcBurst.chargedBurst)
                return;

            if (!playerPerformingAction.playerNetworkManager.isGrounded.Value)
                return;
            if (playerPerformingAction.IsOwner)
                playerPerformingAction.playerNetworkManager.isAttacking.Value = true;

            pcBurst.nowRecasting = true;
            pcBurst.currentEnergy = 0;
            PerformBurst(playerPerformingAction, pcBurst);
        }
        private void PerformBurst(PlayerManager playerPerformingAction, PlayerBurstManager pcBurst)
        {
            PlayableCharacter pc = playerPerformingAction.entry.playableCharacterInventoryManager.currentCharacter;
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
            playerPerformingAction.playerAnimatorManager.PlayTargetSkillActionAnimation(pc, AttackType.Skill, burstAnimation, true);
            playerPerformingAction.entry.playableCharacterEntryManager.playableCharacterModel.GetComponent<VCamScript>().debugPlay = true;
        }
    }
}
