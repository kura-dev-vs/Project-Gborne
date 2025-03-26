using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RK
{
    /// <summary>
    /// ゲーム内で使用されるエフェクト (ダメージやヒール) に必要なコンポーネントの雛形
    /// idなど共通項目を実装して継承する
    /// </summary>
    public class InstantCharacterEffect : ScriptableObject
    {
        [Header("Effect ID")]
        public int instantEffectID;
        public virtual void ProcessEffect(CharacterManager character)
        {

        }
    }
}