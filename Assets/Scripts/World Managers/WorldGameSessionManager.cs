using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ゲームセッションに参加しているプレイヤーの登録を行う
    /// </summary>
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager instance;
        [Header("Active Players In Session")]
        public List<PlayerManager> players = new List<PlayerManager>();
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
        }
        public void AddPlayerToActivePlayersList(PlayerManager player)
        {
            // リストをチェックし、まだその選手が含まれていなければ、追加する。
            if (!players.Contains(player))
            {
                players.Add(player);
            }

            RemoveNullPlayer();
        }
        public void RemovePlayerFromActivePlayersList(PlayerManager player)
        {
            // リストをチェックし、プレーヤーが含まれていれば、削除する。
            if (!players.Contains(player))
            {
                players.Remove(player);
            }

            RemoveNullPlayer();
        }
        // リストにnullがないかチェックし、あった場合はnullを削除する。
        private void RemoveNullPlayer()
        {
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
    }
}
