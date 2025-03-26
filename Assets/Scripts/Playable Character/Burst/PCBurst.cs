using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// キャラクターの元素爆発
    /// これを雛形に固有の爆発の数値を作っていく予定
    /// </summary>
    [CreateAssetMenu(menuName = "Characters/PlayableCharacters/Burst")]
    public class PCBurst : ScriptableObject
    {
        [Header("Burst Information")]
        public string burstName;
        public Sprite burstIcon;
        [TextArea] public string burstDescription;
        public int burstID;
        public float recastTime = 10f;
        public float rechargeNeedEnergy = 50f;
        public int burst_Lv = 1;
        [Header("Burst Actions")]
        public PCBurstAction burstAction;

        [Header("Skill Model")]
        public GameObject burstModel;

        [Header("Duration Time")]
        public float[] durationTime;

        [Header("Support Modifiers")]
        public float[] support_Modifier;
        public float[] base_Percentage;
        public float[] lv_Modifier;
    }
}
