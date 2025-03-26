using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace RK
{
    /// <summary>
    /// プレイヤーの爆発のためのデータベース
    /// idから該当の爆発を取り出す
    /// </summary>
    public class WorldPCBurstDatabase : MonoBehaviour
    {
        public static WorldPCBurstDatabase instance;
        public PCBurst unarmedPCBurst;
        [Header("PC Bursts")]
        [SerializeField] List<PCBurst> pcBursts = new List<PCBurst>();

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

            // すべての爆発にidを付与する
            for (int i = 0; i < pcBursts.Count; i++)
            {
                pcBursts[i].burstID = i;
            }
        }
        public PCBurst GetPCBurstByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return pcBursts.FirstOrDefault(burst => burst.burstID == ID);
        }
    }
}
