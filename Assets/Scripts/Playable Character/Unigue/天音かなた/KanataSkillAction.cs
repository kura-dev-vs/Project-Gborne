using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 天音かなたの固有アクション
    /// </summary>
    public class KanataSkillAction : UniqueSkillActionManager
    {
        public PlayerManager player;
        PCSkill rightSlill;
        public PCRangedSkill leftSkill;
        SkillRangedCollider skillCollider;
        public PCBurst burst;
        SkillRangedCollider burstCollider;
        private void Start()
        {
            player = GetComponentInParent<PlayerManager>();
            rightSlill = GetComponent<RightSkillManager>().skill;
            leftSkill = (PCRangedSkill)GetComponent<LeftSkillManager>().skill;
            burst = GetComponent<PlayerBurstManager>().burst;
        }
        private void SetSkillModifier(PlayerManager player, PCRangedSkill rangedSkill, GameObject skillModel)
        {
            skillCollider = skillModel.GetComponent<SkillRangedCollider>();
            skillCollider.characterCausingDamage = player.entry;
            skillCollider.physicalDamage = rangedSkill.base_Damage[0];
            skillCollider.skillModifier = 1;
            SetDamages(skillCollider, rangedSkill);
        }
        private void SetBurstModifier(PlayerManager player, PCBurst burst, GameObject burstModel)
        {
            burstCollider = burstModel.GetComponent<SkillRangedCollider>();
            burstCollider.characterCausingDamage = player.entry;
            burstCollider.physicalDamage = burst.support_Modifier[1];
            burstCollider.skillModifier = 1;
            burstCollider.durationTime = burst.durationTime[0];
        }

        public void RangedSkillInstantiation()
        {
            Transform leftWeaponTransform = player.playerEquipmentManager.leftHandWeaponModel.transform;
            Vector3 bulletDirection;
            GameObject bullet = Instantiate(leftSkill.skillModel, leftWeaponTransform.position, player.transform.rotation);
            SetSkillModifier(player, leftSkill, bullet);
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                Vector3 targetPosition = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position;
                bulletDirection = targetPosition - leftWeaponTransform.position;
                bullet.GetComponent<HomingController>().SetBulletDirection(bulletDirection);
                bullet.GetComponent<HomingController>().SetTarget(player.playerCombatManager.currentTarget);
            }
            else
            {
                bulletDirection = player.transform.forward;
                bullet.GetComponent<HomingController>().SetBulletDirection(bulletDirection);
            }

            SkillRangedCollider bulletCollider = bullet.GetComponent<SkillRangedCollider>();
            SetDamages(bulletCollider, leftSkill);
        }
        public void BurstInstantiation()
        {
            GameObject burstModel;
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                Vector3 targetPosition = player.playerCombatManager.currentTarget.transform.position;
                burstModel = Instantiate(burst.burstModel, targetPosition, Quaternion.identity);
            }
            else
            {
                burstModel = Instantiate(burst.burstModel, player.transform.position, Quaternion.identity);
            }
            SetBurstModifier(player, burst, burstModel);
        }
    }
}
