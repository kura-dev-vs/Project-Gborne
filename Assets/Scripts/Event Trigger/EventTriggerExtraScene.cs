using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class EventTriggerExtraScene : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {

            if (other.GetComponent<PlayerManager>() == null)
                return;
            if (WorldExtraManager.instance.startFlag)
                return;
            if (!other.GetComponent<PlayerManager>().IsOwner)
                return;
            WorldExtraManager.instance.startFlag = true;
            WorldExtraManager.instance.timeCount = true;
            StartCoroutine(WorldExtraManager.instance.SpawnedCharacter());
        }
    }
}
