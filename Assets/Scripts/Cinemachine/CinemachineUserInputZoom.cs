using Unity.Cinemachine;
using UnityEngine;

public class CinemachineUserInputZoom : CinemachineExtension
{
    // 入力値に掛ける値
    [SerializeField] private float _inputScale = 5;

    // FOVの最小・最大
    [SerializeField, Range(1, 179)] private float _minFOV = 10;
    [SerializeField, Range(1, 179)] private float _maxFOV = 90;

    // ユーザー入力を必要とする
    //public override bool RequiresUserInput => true;

    private float _scrollDelta;
    [SerializeField] private float _adjustFOV;
    int beforeFov = 0;
    int afterFov = 0;
    private bool isChangeFov = false;
    public void ZoomInput(float inputZoom)
    {
        _scrollDelta += inputZoom;
    }

    // 各ステージ毎に実行されるコールバック
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        // Aimの直後だけ処理を実施
        if (stage != CinemachineCore.Stage.Aim)
            return;

        var lens = state.Lens;

        if (!Mathf.Approximately(_scrollDelta, 0))
        {
            // FOVの補正量を計算
            _adjustFOV = Mathf.Clamp(
                _adjustFOV - _scrollDelta * _inputScale,
                _minFOV - lens.FieldOfView,
                _maxFOV - lens.FieldOfView
            );

            _scrollDelta = 0;
        }

        if (isChangeFov)
        {
            isChangeFov = false;
            beforeFov = (int)_adjustFOV;
            _adjustFOV = afterFov;

        }
        // stateの内容は毎回リセットされるので、
        // 毎回補正する必要がある
        lens.FieldOfView += _adjustFOV;

        state.Lens = lens;
    }
    public void ChangeFov(float fov)
    {
        isChangeFov = true;
        afterFov = (int)fov;
    }
    public float GetFov()
    {
        beforeFov = (int)_adjustFOV;
        return beforeFov;
    }
}