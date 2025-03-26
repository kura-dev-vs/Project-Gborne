using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace RK
{
    /// <summary>
    /// プレイヤーに生じるエフェクト (ダメ―ジ、ヒール等) の管理
    /// </summary>
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] InstantCharacterEffect effectToTest;
        [SerializeField] bool processEffect = false;

        private void Update()
        {
            if (processEffect)
            {
                processEffect = false;
                InstantCharacterEffect effect = Instantiate(effectToTest);
                ProcessInstantEffect(effect);
            }
        }
        public void SetManager()
        {
            character = GetComponentInParent<CharacterManager>();
        }
    }
}
