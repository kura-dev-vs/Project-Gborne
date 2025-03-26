using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// leftskill用のmanager. playablecharactermodelには忘れず付けておく
    /// </summary>
    public class LeftSkillManager : PlayerSkillManager
    {
        protected override void Start()
        {
            base.Start();

            skill = playableCharacter.leftSkill;
            recastTime = skill.recastTime;
            currentRecast = recastTime;
            skillMagazine = skill.skillMagazine;
        }
    }
}
