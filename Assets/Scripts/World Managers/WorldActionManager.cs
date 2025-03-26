using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Serialization;

namespace RK
{
    /// <summary>
    /// 武器アイテムやスキル、爆発などプレイヤー側が行うアクションの登録を行う
    /// 各種個別で設定されるIDで管理し、アクションを行う際はidから検索して該当したアクションを行う
    /// </summary>
    public class WorldActionManager : MonoBehaviour
    {
        public static WorldActionManager instance;
        [Header("Weapon Item Actions")]
        public WeaponItemAction[] weaponItemActions;
        public PCSkillAction[] pcSkillActions;
        public PCBurstAction[] pcBurstActions;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            for (int i = 0; i < weaponItemActions.Length; i++)
            {
                weaponItemActions[i].actionID = i;
            }

            for (int i = 0; i < pcSkillActions.Length; i++)
            {
                pcSkillActions[i].skillActionID = i;
            }

            for (int i = 0; i < pcBurstActions.Length; i++)
            {
                pcBurstActions[i].burstActionID = i;
            }
        }
        public WeaponItemAction GetWeaponItemActionByID(int ID)
        {
            // actionIDとIDが一致するactionを返す
            return weaponItemActions.FirstOrDefault(action => action.actionID == ID);
        }
        public PCSkillAction GetPCSkillActionByID(int ID)
        {
            // actionIDとIDが一致するactionを返す
            return pcSkillActions.FirstOrDefault(skillAction => skillAction.skillActionID == ID);
        }
        public PCBurstAction GetPCBurstActionByID(int ID)
        {
            // actionIDとIDが一致するactionを返す
            return pcBurstActions.FirstOrDefault(burstAction => burstAction.burstActionID == ID);
        }
    }
}