using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 誘導する弾丸、スキルの挙動の制御を設定するスクリプト
    /// 弾丸にコンポーネントとして設定する
    /// </summary>
    public class HomingController : MonoBehaviour
    {
        CharacterManager targetCharacter;
        private Rigidbody rb;
        float deleteTime = 1.5f;
        [SerializeField] bool canHoming = false;
        public float bulletSpeed = 2000f;
        public float rotateSpeed = 200f;
        [SerializeField] float limitAngular;
        Vector3 firstDirection;
        [HideInInspector] public Vector3 bulletDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Destroy(gameObject, deleteTime);
        }
        private void FixedUpdate()
        {
            if (canHoming)
            {
                if (targetCharacter != null)
                {
                    Homing();
                }
                else
                {
                    rb.linearVelocity = transform.forward.normalized * bulletSpeed * Time.deltaTime;
                }
            }
            else
            {
                rb.linearVelocity = transform.forward.normalized * bulletSpeed * Time.deltaTime;
            }
        }
        public void SetBulletDirection(Vector3 bullet)
        {
            bulletDirection = bullet;
        }
        public void SetTarget(CharacterManager character)
        {
            targetCharacter = character;

            // 上下の銃口補正のみデフォルトで付与する
            firstDirection = bulletDirection.normalized;
            firstDirection = new Vector3(0, firstDirection.y, 0);
        }
        private void Homing()
        {
            var target = targetCharacter.characterCombatManager.lockOnTransform.position;
            var verticalMuzzle = transform.forward + firstDirection;
            Vector3 direction = (target - rb.position).normalized;
            Vector3 rotateAmount = Vector3.Cross(verticalMuzzle, direction);
            rb.angularVelocity = rotateAmount * rotateSpeed * Time.deltaTime;
            if (rb.angularVelocity.magnitude > limitAngular)
            {
                rb.angularVelocity = rb.angularVelocity.normalized * limitAngular;
            }
            rb.linearVelocity = verticalMuzzle.normalized * bulletSpeed * Time.deltaTime;

        }
    }
}
