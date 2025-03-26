using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Effects/Static Effects/Vitality Effect")]
    public class VitalityEffect : StaticCharacterEffect
    {
        public override void ProcessStaticEffect(CharacterManager character)
        {
            base.ProcessStaticEffect(character);

            character.characterNetworkManager.vitality.Value += 5;
        }
    }
}
