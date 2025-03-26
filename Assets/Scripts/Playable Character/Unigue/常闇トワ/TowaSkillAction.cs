using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 常闇トワの固有アクション
    /// </summary>
    public class TowaSkillAction : MonoBehaviour
    {
        public PlayerManager player;
        PCSkill rightSlill;
        public PCSupportSkill leftSkill;
        SkillHealCollider skillCollider;
        public PCBurst burst;
        SkillHealCollider burstCollider;
        private void Start()
        {
            player = GetComponentInParent<PlayerManager>();
            rightSlill = GetComponent<RightSkillManager>().skill;
            leftSkill = (PCSupportSkill)GetComponent<LeftSkillManager>().skill;
            burst = GetComponent<PlayerBurstManager>().burst;
        }
        private void SetSkillModifier(CharacterManager character, PCSupportSkill supportSkill, GameObject skillModel)
        {
            skillCollider = skillModel.GetComponent<SkillHealCollider>();
            skillCollider.characterCausingHeal = character;
            skillCollider.baseHeal = supportSkill.support_Modifier[0];
            skillCollider.durationTime = supportSkill.durationTime[0];
        }
        private void SetBurstModifier(CharacterManager character, PCBurst burst, GameObject burstModel)
        {
            burstCollider = burstModel.GetComponent<SkillHealCollider>();
            burstCollider.characterCausingHeal = character;
            burstCollider.baseHeal = burst.support_Modifier[1];
            burstCollider.durationTime = burst.durationTime[0];
        }

        public void SupportSkillInstantiation()
        {
            var skillModel = Instantiate(leftSkill.skillModel, transform.parent);
            SetSkillModifier(player, leftSkill, skillModel);
        }
        public void BurstInstantiation()
        {
            GameObject burstModel = Instantiate(burst.burstModel, player.transform.position, Quaternion.identity);
            SetBurstModifier(player, burst, burstModel);
        }
    }
}
