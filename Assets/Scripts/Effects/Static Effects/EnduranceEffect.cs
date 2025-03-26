using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Effects/Static Effects/Endurance Effect")]
    public class EnduranceEffect : StaticCharacterEffect
    {
        public override void ProcessStaticEffect(CharacterManager character)
        {
            base.ProcessStaticEffect(character);

            character.characterNetworkManager.endurance.Value += 5;
        }
    }
}
