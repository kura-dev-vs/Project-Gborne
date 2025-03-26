using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    [CreateAssetMenu(menuName = "Character Effects/Static Effects/Absorption Effect")]
    public class AbsorptionEffect : StaticCharacterEffect
    {
        [SerializeField] float physicalModifier;
        public float physicalAbsModifier = 0.5f;
        public override void ProcessStaticEffect(CharacterManager character)
        {
            base.ProcessStaticEffect(character);
            if (character.IsOwner)
            {

                if (character.GetComponent<EntryManager>() != null)
                {
                    var entry = character.GetComponent<EntryManager>();

                    for (int i = 0; i < 4; i++)
                    {
                        GameObject pcObject = entry.playableCharacterInventoryManager.pcObjects[i];
                        if (pcObject == null)
                            continue;

                        float baseAbsorption = pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption;
                        Debug.Log("before: " + baseAbsorption);
                        physicalModifier = baseAbsorption * physicalAbsModifier;

                        pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption += physicalModifier;
                        pcObject.GetComponent<PlayableCharacterManager>().playableCharacter.finalPhysicalAbs = pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption;
                        Debug.Log("after: " + pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption);
                    }
                }
            }
        }
        public override void RemoveStaticEffect(CharacterManager character)
        {
            base.RemoveStaticEffect(character);
            if (character.IsOwner)
            {

                if (character.GetComponent<EntryManager>() != null)
                {
                    var entry = character.GetComponent<EntryManager>();

                    for (int i = 0; i < 4; i++)
                    {
                        GameObject pcObject = entry.playableCharacterInventoryManager.pcObjects[i];
                        if (pcObject == null)
                            continue;
                        float baseAbsorption = pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption;
                        Debug.Log("before: " + baseAbsorption);

                        pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption = baseAbsorption / (1.0f + physicalAbsModifier);
                        pcObject.GetComponent<PlayableCharacterManager>().playableCharacter.finalPhysicalAbs = pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption;
                        Debug.Log("after: " + pcObject.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption);
                    }
                }
            }
        }
    }
}
