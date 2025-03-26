using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// DurkCharacterの追加SFXマネージャー
    /// </summary>
    public class AIDurkSoundFXManager : CharacterSoundFXManager
    {
        [Header("Patta Whooshes")]
        public AudioClip[] pattaWhooshes;
        [Header("Patta Impacts")]
        public AudioClip[] pattaImpacts;
        [Header("Stomp Impacts")]
        public AudioClip[] stompImpacts;
        public virtual void PlayPattaImpactSoundFX()
        {
            if (pattaImpacts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(pattaImpacts));
        }
        public virtual void PlayStompImpactSoundFX()
        {
            if (stompImpacts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(stompImpacts));
        }

    }
}
