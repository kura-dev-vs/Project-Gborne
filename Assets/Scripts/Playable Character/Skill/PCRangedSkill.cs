using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 遠距離スキル用の雛形
    /// inspectorからこの変数を調整し、そこでの数値がゲーム上に反映される
    /// </summary>
    [CreateAssetMenu(menuName = "Characters/PlayableCharacters/Ranged Skill")]
    public class PCRangedSkill : PCSkill
    {
        [Header("Skill Model")]
        public GameObject skillModel;

        [Header("Ranged Modifiers")]
        public float[] base_Damage;
        public float[] base_Percentage;
        public float[] lv_Modifier;

        [Header("Whooshes")]
        public AudioClip[] whooshes;

    }
}
