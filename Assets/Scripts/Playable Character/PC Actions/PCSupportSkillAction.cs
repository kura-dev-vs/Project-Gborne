using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// サポートスキル用
    /// 拡張性が低いので基本動作をここに置いて雛形にし、各キャラクター固有のものを作っていくかも
    /// </summary>
    [CreateAssetMenu(menuName = "Characters/PlayableCharacters/Actions/Support Skill Action")]
    public class PCSupportSkillAction : PCSkillAction
    {
        [SerializeField] string skillAnimation = "skillAnimation";
        public override void AttemptToPerformSkill(PlayerManager playerPerformingAction, PlayerSkillManager pcSkill)
        {
            base.AttemptToPerformSkill(playerPerformingAction, pcSkill);
            if (!playerPerformingAction.IsOwner)
                return;
            if (pcSkill.nowRecasting)
                return;

            if (!playerPerformingAction.playerNetworkManager.isGrounded.Value)
                return;
            if (playerPerformingAction.IsOwner)
                playerPerformingAction.playerNetworkManager.isAttacking.Value = true;

            pcSkill.nowRecasting = true;
            PerformSkill(playerPerformingAction, pcSkill);
        }
        private void PerformSkill(PlayerManager playerPerformingAction, PlayerSkillManager pcSkill)
        {
            PlayableCharacter pc = playerPerformingAction.entry.playableCharacterInventoryManager.currentCharacter;
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                playerPerformingAction.playerAnimatorManager.PlayTargetSkillActionAnimation(pc, AttackType.Skill, skillAnimation, true);

            }
            // otherwise, if we are not already attacking just perform a regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                playerPerformingAction.playerAnimatorManager.PlayTargetSkillActionAnimation(pc, AttackType.Skill, skillAnimation, true);
            }
            else
            {
                // playerPerformingAction.isPerformingAction == true && playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon == false
                // 
            }

        }
    }
}
