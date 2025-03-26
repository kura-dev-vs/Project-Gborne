using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// スタミナを奪うエフェクト
    /// テスト用に作っただけなので実装するかは未定
    /// </summary>
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;
        public override void ProcessEffect(CharacterManager character)
        {
            CalculateStaminaDamage(character);
        }
        private void CalculateStaminaDamage(CharacterManager character)
        {
            if (character.IsOwner)
            {
                Debug.Log("CHARACTER IS TAKING: " + staminaDamage + " STAMINA DAMAGE");
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
}
