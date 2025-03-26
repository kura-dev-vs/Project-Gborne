using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RK
{
    /// <summary>
    /// ui遷移等に必要なイベントシステムをどのsceneでも保持するためのもの
    /// </summary>
    public class EventSystemManager : MonoBehaviour
    {
        public static EventSystemManager instance;
        public EventSystem eventSystem;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                eventSystem = GetComponent<EventSystem>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
