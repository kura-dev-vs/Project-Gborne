using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 遠距離スキル
    /// 拡張性が低いので基本動作をここに置いて雛形にし、各キャラクター固有のものを作っていくかも
    /// </summary>
    [CreateAssetMenu(menuName = "Characters/PlayableCharacters/Actions/Ranged Skill Action")]
    public class PCRangedSkillAction : PCSkillAction
    {
        [SerializeField] string skillAnimation = "skillAnimationName";
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
            // コンボが可能なとき
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                playerPerformingAction.playerAnimatorManager.PlayTargetSkillActionAnimation(pc, AttackType.Skill, skillAnimation, true);

            }
            // コンボ関係ないとき
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                playerPerformingAction.playerAnimatorManager.PlayTargetSkillActionAnimation(pc, AttackType.Skill, skillAnimation, true);
            }
            else
            {
            }

        }
    }
}
