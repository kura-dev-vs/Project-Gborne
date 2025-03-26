using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// スキルの雛形
    /// これを継承してサポートスキル、近距離スキルといったように派生していく
    /// 将来的により複雑なスキルになる場合は継承してから固有のものを作成したほうが良いかも
    /// </summary>
    public class PCSkill : ScriptableObject
    {
        [Header("Skill Information")]
        public string skillName;
        public Sprite skillIcon;
        [TextArea] public string skillDescription;
        public int skillID;
        public float recastTime;
        public float generateEnergy;
        public int skill_Lv = 1;
        public int skillMagazine = 1;
        [Header("Skill Actions")]
        public PCSkillAction skillAction;

        [Header("Soul Modifiers")]
        public float physicalDamage;
        public float magicDamage;
        public float fireDamage;
        public float holyDamage;
    }
}
