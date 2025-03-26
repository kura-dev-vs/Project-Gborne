using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Effects/Static Effects/Power Ups Effect")]
    public class PowerUpsEffect : StaticCharacterEffect
    {
        [SerializeField] int strengthGainedFromWeapon;

        public override void ProcessStaticEffect(CharacterManager character)
        {
            base.ProcessStaticEffect(character);
            if (character.IsOwner)
            {
                strengthGainedFromWeapon = Mathf.RoundToInt(character.characterNetworkManager.strength.Value / 2);
                Debug.Log("strength gained " + strengthGainedFromWeapon);
                character.characterNetworkManager.strengthModifier.Value += strengthGainedFromWeapon;
            }
        }
        public override void RemoveStaticEffect(CharacterManager character)
        {
            base.RemoveStaticEffect(character);
            if (character.IsOwner)
            {

                character.characterNetworkManager.strengthModifier.Value -= strengthGainedFromWeapon;
            }
        }
    }
}
