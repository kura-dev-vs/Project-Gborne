using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessingManager : MonoBehaviour
{
    /// <summary>
    /// ポストプロセッシング用のマネージャー
    /// 将来的に和＾るどのマネージャーを用意してidで管理したほうがいいかも？
    /// </summary>
    public static PostProcessingManager instance;
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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
