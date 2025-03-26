using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// rightskill用のmanager. playablecharactermodelには忘れず付けておく
    /// </summary>
    public class RightSkillManager : PlayerSkillManager
    {
        protected override void Start()
        {
            base.Start();

            skill = playableCharacter.rightSkill;
            recastTime = skill.recastTime;
            currentRecast = recastTime;
            skillMagazine = skill.skillMagazine;
        }
    }
}
