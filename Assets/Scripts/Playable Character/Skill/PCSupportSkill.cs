using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// サポートスキルの雛形
    /// </summary>
    [CreateAssetMenu(menuName = "Characters/PlayableCharacters/Support Skill")]
    public class PCSupportSkill : PCSkill
    {
        [Header("Skill Model")]
        public GameObject skillModel;

        [Header("Duration Time")]
        public float[] durationTime;

        [Header("Support Modifiers")]
        public float[] support_Modifier;
        public float[] base_Percentage;
        public float[] lv_Modifier;

        [Header("Whooshes")]
        public AudioClip[] whooshes;
    }
}
