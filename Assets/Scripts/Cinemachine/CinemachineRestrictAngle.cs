using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace RK
{
    [AddComponentMenu("")] // Hide in menu
    [ExecuteAlways]
    public class CinemachineRestrictAngle : CinemachineExtension
    {
        /// <summary>
        /// カメラの角度を制限する
        /// field を public にすることで SaveDuringPlay が可能になる
        /// </summary>
        [Header("適用段階")]
        public CinemachineCore.Stage m_ApplyAfter = CinemachineCore.Stage.Aim;

        [Header("俯瞰角 閾値")]
        [Range(0f, 90f)]
        public float lowAngleThreshold = 80f;

        [Header("アオリ角 閾値")]
        [Range(0f, 90f)]
        public float highAngleThreshold = 0f;

        /// <summary>
        /// カメラパラメータ更新後に呼び出される Callback。ここで結果を微調整する。
        /// </summary>
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != m_ApplyAfter) return;
            var eulerAngles = state.RawOrientation.eulerAngles;
            if (eulerAngles.x > 180f)
            {
                eulerAngles.x -= 360f;
            }

            eulerAngles.x = Mathf.Clamp(eulerAngles.x, -highAngleThreshold, lowAngleThreshold);

            if (eulerAngles.x < 0f)
            {
                eulerAngles.x += 360f;
            }

            state.RawOrientation = Quaternion.Euler(eulerAngles);
        }
    }
}
