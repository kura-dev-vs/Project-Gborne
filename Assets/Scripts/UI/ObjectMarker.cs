using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ゲーム空間に生成されたUIをUIHudに表示し、その位置を更新する
    /// </summary>
    public class ObjectMarker : MonoBehaviour
    {
        private Camera _targetCamera;
        // UIを表示させる対象オブジェクト
        [HideInInspector] public Transform _target;
        [HideInInspector] public GameObject _targetObject;
        // 表示するUI
        [SerializeField] private Transform _targetUI;
        // オブジェクト位置のオフセット
        [SerializeField] private Vector3 _worldOffset;
        private RectTransform _parentUI;

        private void Awake()
        {
            _parentUI = _targetUI.parent.GetComponent<RectTransform>();
        }
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
            DestroyBar();
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
            RaycastHit hitInfo;

            if (Physics.Raycast(cameraTransform.position, targetDir, out hitInfo, Vector3.Distance(targetWorldPos, cameraTransform.position), WorldUtilityManager.instance.GetUILayers()))
            {
                _targetUI.gameObject.SetActive(false);
                return;
            }

            /*
            // オブジェクトのワールド座標→スクリーン座標変換
            var targetScreenPos = _targetCamera.WorldToScreenPoint(targetWorldPos);

            // スクリーン座標変換→UIローカル座標変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentUI,
                targetScreenPos,
                null,
                out var uiLocalPos
            );

            // RectTransformのローカル座標を更新
            _targetUI.localPosition = uiLocalPos;
            return;
            */

            _targetUI.localPosition = CameraUtils.WorldToScreenSpaceCamera
            (
                worldCamera: PlayerCamera.instance.mainCamera,
                canvasCamera: UICamera.instance.mainCamera,
                canvasRectTransform: GetComponent<RectTransform>(),
                worldPosition: targetWorldPos
            );
        }
        private void DestroyBar()
        {
            if (_targetObject == null)
            {
                Destroy(gameObject);
                return;
            }

            float dist = Vector3.Distance(_targetCamera.transform.position, _target.transform.position);
            if (dist > PlayerCamera.instance.targetIsActiveDistance)
            {
                _target.gameObject.GetComponent<Health_Bar_Instantiation>().isActive = false;
                Destroy(gameObject);
            }

        }
    }
}
