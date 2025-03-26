using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class PCSkillAction : ScriptableObject
    {
        public int skillActionID;

        public virtual void AttemptToPerformSkill(PlayerManager playerPerformingAction, PlayerSkillManager pcSkill)
        {

        }
    }
}
