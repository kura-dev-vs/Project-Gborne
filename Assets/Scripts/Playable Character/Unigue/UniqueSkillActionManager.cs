using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// キャラクター固有のアクションイベントを行うメソッド
    /// このスクリプトを継承した固有スクリプトを作成し、固有のキャラクターオブジェクトにコンポーネントとして付与、アニメーションイベント等で使用する
    /// スキル、爆発などの細かなアクションを記述する
    /// </summary>
    public class UniqueSkillActionManager : MonoBehaviour
    {
        // スキルアイテムから数値をスキルコライダーにペーストする
        protected virtual void SetDamages(SkillColliderBase skillCollider, PCSkill pcSkill)
        {
            skillCollider.physicalDamage = pcSkill.physicalDamage;
            skillCollider.magicDamage = pcSkill.magicDamage;
            skillCollider.fireDamage = pcSkill.fireDamage;
            skillCollider.holyDamage = pcSkill.holyDamage;
        }
    }
}
