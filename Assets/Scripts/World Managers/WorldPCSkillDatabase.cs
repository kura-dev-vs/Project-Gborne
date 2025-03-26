using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RK
{
    /// <summary>
    /// プレイヤーのスキルのためのデータベース
    /// idから該当のスキルを取り出す
    /// </summary>
    public class WorldPCSkillDatabase : MonoBehaviour
    {
        public static WorldPCSkillDatabase instance;
        public PCSkill unarmedPCSkill;
        [Header("PC Skills")]
        [SerializeField] List<PCSkill> pcSkills = new List<PCSkill>();

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

            // すべてのスキルにidを付与する
            for (int i = 0; i < pcSkills.Count; i++)
            {
                pcSkills[i].skillID = i;
            }
        }
        public PCSkill GetPCSkillByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return pcSkills.FirstOrDefault(skill => skill.skillID == ID);
        }
    }
}
