using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// AIステートの雛型
    /// Tick: Updateと同義。このステートで実行される処理を書く
    /// SwitchState: このステートに切り替わった時の処理
    /// ResetStateFlags: フラグ関連のリセットを行う。ここで状態フラグをリセットし、その状態に戻ると、再び空白になるようにする。
    /// </summary>
    public class AIState : ScriptableObject
    {
        public virtual AIState Tick(AICharacterManager aiCharacter)
        {
            return this;
        }
        protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
        {
            ResetStateFlags(aiCharacter);
            return newState;
        }
        protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
        {

        }
    }
}
