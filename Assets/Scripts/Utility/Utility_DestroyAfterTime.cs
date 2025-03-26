using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 一定時間後にゲームオブジェクトを削除する
    /// </summary>
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float timeUntilDestroyed = 5;

        private void Awake()
        {
            Destroy(gameObject, timeUntilDestroyed);
        }

    }
}
