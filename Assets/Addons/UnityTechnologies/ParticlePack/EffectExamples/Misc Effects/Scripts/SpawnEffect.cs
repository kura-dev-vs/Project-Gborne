using System.Collections;
using System.Collections.Generic;
using RK;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    public ParticleSystem ps;
    ParticleSystem newParticle;

    private void LateUpdate()
    {
        if (newParticle == null)
            return;
        newParticle.transform.position = this.transform.position;
    }

    public void PlayEffect()
    {
        // パーティクルシステムのインスタンスを生成する。
        newParticle = Instantiate(ps);
        // パーティクルの発生場所をこのスクリプトをアタッチしているGameObjectの場所にする。
        newParticle.transform.position = this.transform.position;
        // パーティクルを発生させる。
        newParticle.Play();
        // インスタンス化したパーティクルシステムのGameObjectを削除する。(任意)
        // ※第一引数をnewParticleだけにするとコンポーネントしか削除されない。
        Destroy(newParticle.gameObject, 1.5f);
    }
}
