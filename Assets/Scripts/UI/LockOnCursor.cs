using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class LockOnCursor : MonoBehaviour
    {
        private Camera _targetCamera;
        // UIを表示させる対象オブジェクト
        [HideInInspector] public Transform _target;
        [HideInInspector] public GameObject _targetObject;
        // 表示するUI
        [SerializeField] private Transform _targetUI;
        // オブジェクト位置のオフセット
        [SerializeField] private Vector3 _worldOffset;

        private void Start()
        {
            _targetCamera = PlayerCamera.instance.mainCamera;
        }
        public void Initialize(Transform target, GameObject targetObject)
        {
            _target = target;
            _targetObject = targetObject;
        }
        private void Update()
        {
            OnUpdatePosition();
        }

        // UIの位置を更新する
        private void OnUpdatePosition()
        {
            if (_targetObject == null)
                return;
            var cameraTransform = _targetCamera.transform;

            // カメラの向きベクトル
            var cameraDir = cameraTransform.forward;
            // オブジェクトの位置
            var targetWorldPos = _target.position + _worldOffset;
            // カメラからターゲットへのベクトル
            var targetDir = targetWorldPos - cameraTransform.position;

            // 内積を使ってカメラ前方かどうかを判定
            var isFront = Vector3.Dot(cameraDir, targetDir) > 0;

            // カメラ前方ならUI表示、後方なら非表示
            _targetUI.gameObject.SetActive(isFront);
            if (!isFront) return;

            _targetUI.localPosition = CameraUtils.WorldToScreenSpaceCamera
            (
                worldCamera: PlayerCamera.instance.mainCamera,
                canvasCamera: UICamera.instance.mainCamera,
                canvasRectTransform: GetComponent<RectTransform>(),
                worldPosition: targetWorldPos
            );
        }
    }
}
