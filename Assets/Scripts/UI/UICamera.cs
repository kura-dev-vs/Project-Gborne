using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class UICamera : MonoBehaviour
    {
        public static UICamera instance;
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
