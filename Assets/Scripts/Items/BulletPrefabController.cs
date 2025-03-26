using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 遠距離武器の弾丸が持つコンポーネント
    /// まっすぐにのみ飛、速度をいじれる
    /// </summary>
    public class BulletPrefabController : MonoBehaviour
    {
        [HideInInspector] public Vector3 bulletDirection;
        //[HideInInspector] 
        public float bulletSpeed = 2000f;
        private Rigidbody rb;

        // 前フレームのワールド位置
        private Vector3 _prevPosition;
        private void Start()
        {
            _prevPosition = transform.position;
            Destroy(gameObject, 5.0f);
            rb = GetComponent<Rigidbody>();
        }
        private void FixedUpdate()
        {
            if (bulletDirection == null)
                return;

            // 現在フレームのワールド位置
            var position = transform.position;
            // 移動量を計算
            var delta = position - _prevPosition;
            // 次のUpdateで使うための前フレーム位置更新
            _prevPosition = position;

            //transform.position += bulletDirection.normalized * bulletSpeed * Time.deltaTime;
            rb.linearVelocity = bulletDirection.normalized * bulletSpeed * Time.deltaTime;

            // 進行方向（移動量ベクトル）に向くようなクォータニオンを取得
            var rotation = Quaternion.LookRotation(delta, Vector3.up);

            // オブジェクトの回転に反映
            transform.rotation = rotation;
        }

    }
}
