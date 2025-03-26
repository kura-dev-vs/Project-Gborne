using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RK
{
    public class StaticCharacterEffect : ScriptableObject
    {
        [Header("Effect I.D")]
        public int staticEffectID;
        public Image staticEffectIcon;
        [TextArea] public string effectDescription;


        public virtual void ProcessStaticEffect(CharacterManager character)
        {

        }
        public virtual void RemoveStaticEffect(CharacterManager character)
        {

        }
    }
}
