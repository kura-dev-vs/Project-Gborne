using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ボス専用のnetworkmanager
    /// 基本は形態変化の管理に使用
    /// </summary>
    public class AIBossCharacterNetworkManager : AICharacterNetworkManager
    {
        AIBossCharacterManager aIBossCharacter;

        protected override void Awake()
        {
            base.Awake();

            aIBossCharacter = GetComponent<AIBossCharacterManager>();
        }

        public override void CheckHP(int oldValue, int newValue)
        {
            base.CheckHP(oldValue, newValue);

            if (aIBossCharacter.IsOwner)
            {
                if (currentHealth.Value <= 0)
                    return;

                if (!aIBossCharacter.phase02.Value)
                {
                    float healthNeededForShift = maxHealth.Value * (aIBossCharacter.minimumHealthPercentageToShift / 100);
                    if (currentHealth.Value <= healthNeededForShift)
                    {
                        aIBossCharacter.PhaseShift();
                    }
                }

            }
        }
    }
}
