using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 弾丸のvfx, sfxに関するスクリプト
    /// アセットから持ってきたものとほぼ同じ
    /// 発射時、推進時、着弾爆発時の3つのエフェクトを発生させる
    /// </summary>
    public class BulletVFXManager : MonoBehaviour
    {
        public GameObject impactParticle; // Effect spawned when projectile hits a collider
        public GameObject projectileParticle; // Effect attached to the gameobject as child
        public GameObject muzzleParticle; // Effect instantly spawned when gameobject is spawned
        private void Start()
        {
            projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
            projectileParticle.transform.parent = transform;
            if (muzzleParticle)
            {
                muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
                Destroy(muzzleParticle, 1.5f); // 2nd parameter is lifetime of effect in seconds
            }
        }
        public void ImpactVFX()
        {
            GameObject impactP = Instantiate(impactParticle, transform.position, Quaternion.Inverse(transform.rotation)) as GameObject; // Spawns impact effect

            ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>(); // Gets a list of particle systems, as we need to detach the trails
                                                                                 //Component at [0] is that of the parent i.e. this object (if there is any)
            for (int i = 1; i < trails.Length; i++) // Loop to cycle through found particle systems
            {
                ParticleSystem trail = trails[i];

                if (trail.gameObject.name.Contains("Trail"))
                {
                    trail.transform.SetParent(null); // Detaches the trail from the projectile
                    Destroy(trail.gameObject, 2f); // Removes the trail after seconds
                }
            }

            Destroy(projectileParticle, 3f); // Removes particle effect after delay
            Destroy(impactP, 3.5f); // Removes impact effect after delay
            Destroy(gameObject);
        }
    }
}
