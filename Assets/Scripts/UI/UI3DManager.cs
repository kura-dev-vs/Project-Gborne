using System.Collections;
using System.Collections.Generic;
using Mebiustos.MMD4MecanimFaciem;
using UnityEngine;

namespace RK
{
    public class UI3DManager : MonoBehaviour
    {
        SkinnedMeshRenderer[] meshes;
        float idleLeavingTime = 0;
        [HideInInspector] public bool idleAnimation = false;

        private void Awake()
        {
            meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        }
        public void Transparent()
        {
            foreach (SkinnedMeshRenderer i in meshes)
            {
                //i.enabled = false;
                i.gameObject.layer = LayerMask.NameToLayer("Black");
            }
            StartCoroutine(OpaqueCoroutines(.15f));
        }

        public void Opaque()
        {
            foreach (SkinnedMeshRenderer i in meshes)
            {
                //i.enabled = true;
                i.gameObject.layer = LayerMask.NameToLayer("3D UI");
            }
        }

        IEnumerator OpaqueCoroutines(float second)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < second)
            {
                yield return null;
            }
            Opaque();
        }
        private void LateUpdate()
        {
            if (idleAnimation)
            {
                idleLeavingTime += Time.unscaledDeltaTime;
            }
            else
            {
                idleLeavingTime = 0;
            }

            if (idleLeavingTime > 10)
            {
                if (GetComponent<Animator>() != null)
                {
                    GetComponent<Animator>().Play("Leaving Motion");
                }
            }
        }
        public void AnimatorEvent(string shape)
        {
            GetComponent<FaciemController>().SetFace(shape);
        }
    }
}