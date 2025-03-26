using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ダメージを与えた場合、ヒールを行った場合など体力が変化するエフェクトを受けたときにその数値をUIHudに表示する
    /// </summary>
    public class HealthChangeTextController : MonoBehaviour
    {
        [HideInInspector] public TextMeshProUGUI healthText;
        private Camera _targetCamera;
        // UIを表示させる対象オブジェクト
        [HideInInspector] public Vector3 _target;
        // 表示するUI
        [SerializeField] private Transform _targetUI;
        // オブジェクト位置のオフセット
        [SerializeField] private Vector3 _worldOffset;
        [SerializeField] private float randomRange;
        private RectTransform _parentUI;
        [SerializeField] float deleteTime = 2f;

        private void Awake()
        {
            _parentUI = _targetUI.parent.GetComponent<RectTransform>();
            healthText = GetComponentInChildren<TextMeshProUGUI>();
            _worldOffset += new Vector3(Random.Range(-randomRange, randomRange), Random.Range(0, randomRange), Random.Range(-randomRange, randomRange));
        }
        private void Start()
        {
            _targetCamera = PlayerCamera.instance.mainCamera;
            Destroy(gameObject, deleteTime);
        }
        public void Initialize(Vector3 target, string text)
        {
            _target = target;
            healthText.SetText(text);
        }
        public void SetColor(Color color)
        {
            healthText.color = color;
        }
        private void Update()
        {
            OnUpdatePosition();
        }

        // UIの位置を更新する
        private void OnUpdatePosition()
        {
            var cameraTransform = _targetCamera.transform;

            // カメラの向きベクトル
            var cameraDir = cameraTransform.forward;
            // オブジェクトの位置
            var targetWorldPos = _target + _worldOffset;
            // カメラからターゲットへのベクトル
            var targetDir = targetWorldPos - cameraTransform.position;

            // 内積を使ってカメラ前方かどうかを判定
            var isFront = Vector3.Dot(cameraDir, targetDir) > 0;

            // カメラ前方ならUI表示、後方なら非表示
            _targetUI.gameObject.SetActive(isFront);
            if (!isFront) return;

            /*
            Screen space overlayの場合
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
            */

            // Screen space cameraの場合

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
