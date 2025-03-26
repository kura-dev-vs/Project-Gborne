using Unity.Cinemachine;
using UnityEngine;

namespace RK
{
    public class CameraDock : MonoBehaviour
    {
        [HideInInspector] public Transform lookAtTarget = null;
        public float TransitionAmount = 50;
        public GameObject Target;

        public void SetTarget(Transform target)
        {
            lookAtTarget = target;
        }
        private void Update()
        {
            if (lookAtTarget == null)
                return;

            Target.transform.position = Vector3.Lerp(Target.transform.position, lookAtTarget.transform.position, TransitionAmount * Time.deltaTime);
        }
    }
}
