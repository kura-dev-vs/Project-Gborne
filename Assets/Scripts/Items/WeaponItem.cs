using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// itemを継承した武器専用
    /// </summary>
    public class WeaponItem : EquipmentItem
    {
        [Header("Animations")]
        public AnimatorOverrideController weaponAnimator;
        [Header("Model Instantiation")]
        public WeaponModelType weaponModelType;
        [Header("Weapon Model")]
        public GameObject weaponModel;
        [Header("Weapon Class")]
        public WeaponClass weaponClass;
        [Header("Weapon Requirements")]
        public int strengthREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;
        public int faithREQ = 0;
        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int holyDamage = 0;
        public int lightningDamage = 0;

        // 武器ガード吸収（ブロック力）
        [Header("Weapon Poise")]
        // 攻撃時、よろけていたらダメージ増加
        public float poiseDamage = 10;
        public float stanceDamage = 10;

        [Header("Attack Modifiers")]
        public float light_Attack_01_Modifier = 1.0f;
        public float light_Attack_02_Modifier = 1.2f;
        public float light_Attack_03_Modifier = 1.5f;
        public float light_Attack_04_Modifier = 2.0f;
        public float heavy_Attack_01_Modifier = 1.4f;
        public float heavy_Attack_02_Modifier = 1.6f;
        public float charge_Attack_01_Modifier = 2.0f;
        public float charge_Attack_02_Modifier = 2.2f;
        public float running_Attack_01_Modifier = 1.1f;
        public float rolling_Attack_01_Modifier = 1.1f;
        public float backstep_Attack_01_Modifier = 1.1f;
        public float riposte_Attack_01_Modifier = 3.3f;
        public float backstab_Attack_01_Modifier = 3.3f;


        [Header("Stamina Costs")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostMultiplier = 1f;
        public float heavyAttackStaminaCostMultiplier = 1.3f;
        public float chargeAttackStaminaCostMultiplier = 1.5f;
        public float runningAttackStaminaCostMultiplier = 1.1f;
        public float rollingAttackStaminaCostMultiplier = 1.1f;
        public float backstepAttackStaminaCostMultiplier = 1.1f;

        [Header("Weapon Blocking Absorption")]
        public float physicalBaseDamageAbsorption = 50;
        public float magicBaseDamageAbsorption = 50;
        public float fireBaseDamageAbsorption = 50;
        public float holyBaseDamageAbsorption = 50;
        public float lightningBaseDamageAbsorption = 50;
        public float stability = 50;    // ブロック動作で減少するスタミナ


        // 武器アイテムの持つアクション
        [Header("Actions")]
        public WeaponItemAction oh_RB_Action;   // RBアクション
        public WeaponItemAction oh_LB_Action;   // LBアクション
        public WeaponItemAction oh_RT_Action;   // RTアクション

        // 武器を振ったときの音
        [Header("Whooshes")]
        public AudioClip[] whooshes;
        public AudioClip[] blocking;

        // 遠距離武器用のオプション
        [Header("Bullet (Ranged Option)")]
        public GameObject bullet;
        public int baseHPCostPct = 6;
    }
}
