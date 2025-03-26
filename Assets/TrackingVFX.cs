using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class TrackingVFX : MonoBehaviour
    {
        ParticleSystem newParticle;
        GameObject trackingTarget;

        private void LateUpdate()
        {
            if (trackingTarget != null)
                this.transform.position = trackingTarget.transform.position;
        }

        public void PlayEffect(GameObject target)
        {
            newParticle = GetComponent<ParticleSystem>();
            newParticle.Play();
            trackingTarget = target;
            this.transform.position = trackingTarget.transform.position;
            Destroy(gameObject, 1.5f);
        }
    }
}
