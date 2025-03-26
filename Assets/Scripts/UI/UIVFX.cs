using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class UIVFX : MonoBehaviour
    {
        ParticleSystem newParticle;
        private void Start()
        {
            //newParticle = GetComponent<ParticleSystem>();
        }
        float currentTime = 0;
        [SerializeField] float destroyTime = 1;
        GameObject trackingTarget;
        private void Update()
        {
            newParticle.Simulate(
                t: Time.unscaledDeltaTime,
                withChildren: true,
                restart: false);

            currentTime += Time.unscaledDeltaTime;
            if (currentTime > destroyTime)
            {
                Destroy(gameObject);
            }
        }
        public void PlayEffect(GameObject target)
        {
            newParticle = GetComponent<ParticleSystem>();
            newParticle.Play();
            trackingTarget = target;
            this.transform.position = trackingTarget.transform.position;
        }
    }
}
