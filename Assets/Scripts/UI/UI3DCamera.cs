using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class UI3DCamera : MonoBehaviour
    {
        // 固定
        // localposition = (0,2,0)
        // localrotation = 10,0,0
        public static UI3DCamera instance;
        public Camera mainCamera;
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
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            mainCamera = GetComponentInChildren<Camera>();
        }
    }
}
