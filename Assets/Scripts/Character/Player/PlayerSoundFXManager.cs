using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class PlayerSoundFXManager : CharacterSoundFXManager
    {
        PlayerManager player;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponentInParent<PlayerManager>();
        }
        public override void PlayBlockSoundFX()
        {
            base.PlayBlockSoundFX();
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerCombatManager.currentWeaponBeingUsed.blocking));
        }
    }
}
