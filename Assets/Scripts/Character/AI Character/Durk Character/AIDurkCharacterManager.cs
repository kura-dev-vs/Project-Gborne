using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// Durk Character専用の追加Character MAnager
    /// 専用のsoundやcombatを設定したいのでこれで追加管理
    /// </summary>
    public class AIDurkCharacterManager : AIBossCharacterManager
    {
        [HideInInspector] public AIDurkSoundFXManager durkSoundFXManager;
        [HideInInspector] public AIDurkCombatManager durkCombatManager;
        protected override void Awake()
        {
            base.Awake();

            durkSoundFXManager = GetComponent<AIDurkSoundFXManager>();
            durkCombatManager = GetComponent<AIDurkCombatManager>();
        }
    }
}
