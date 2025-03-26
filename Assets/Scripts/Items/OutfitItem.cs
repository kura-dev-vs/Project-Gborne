using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 防具用アイテム
    /// </summary> 
    public class OutfitItem : EquipmentItem
    {
        public Outfit outfitType;
        public int level = 1;
        [Header("Outfit Absorption Bonus")]
        public float physicalDamageAbsorption;
        public float magicDamageAbsorption;
        public float fireDamageAbsorption;
        public float lightningDamageAbsorption;
        public float holyDamageAbsorption;

        [Header("Outfit Resistance Bonus")]
        public float immunity;  // 毒耐性
        public float robustness;    // 出血凍結耐性
        public float focus;     // 睡眠、発狂耐性
        public float vitality;  // 呪い耐性

        [Header("Pose")]
        public float poise;
    }
}
