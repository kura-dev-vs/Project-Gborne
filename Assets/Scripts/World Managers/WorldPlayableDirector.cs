using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RK
{
    public class WorldPlayableDirector : MonoBehaviour
    {
        public static WorldPlayableDirector instance;
        public PlayableDirector director;
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
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            director = GetComponent<PlayableDirector>();
        }
    }
}
