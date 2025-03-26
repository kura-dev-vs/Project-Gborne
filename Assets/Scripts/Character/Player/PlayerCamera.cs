using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using RK;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/// <summary>
/// playercamera用のコンポーネント
/// ロックオン
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    public Camera mainCamera;
    [SerializeField] Camera uiCamera;
    public Camera ui3DCamera;
    public static PlayerCamera instance;
    public PlayerManager player;
    public CinemachineCamera cameraObject;
    public CinemachineCamera lockOnCamera;
    public CinemachineBrain brain;
    CameraDock cameraDock;
    public GameObject blackObject;
    [Header("Lock On")]
    [SerializeField] private float lockOnRadius = 20;
    [SerializeField] float minimumViewableAngle = -90;
    [SerializeField] float maximumViewableAngle = 90;
    [SerializeField] int lockOnPriority = 11;
    [SerializeField] int anLockOnPriority = 0;
    public CharacterManager nearestLockOnTarget;
    [SerializeField] RectTransform lockonCursor;
    [SerializeField] float lockonFactor = 0.3f;
    float lockonThreshold = 0.5f;
    Transform cameraTrn;
    public float targetIsActiveDistance = 20f;

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
        cameraTrn = mainCamera.transform;
        brain = GetComponentInChildren<CinemachineBrain>();
        cameraDock = GetComponentInChildren<CameraDock>();
    }
    private void LateUpdate()
    {
        if (player == null)
            return;
        if (cameraObject.Follow == null)
        {
            cameraObject.Follow = player.transform;
            cameraObject.LookAt = player.transform;
            lockOnCamera.Follow = player.transform;
        }

        transform.rotation = mainCamera.transform.rotation;
        //if (lockOnCamera.LookAt != null)
        if (cameraDock.lookAtTarget != null)
        {
            //var targetScreenPos = mainCamera.WorldToScreenPoint(lockOnCamera.LookAt.transform.position);
            var targetScreenPos = mainCamera.WorldToScreenPoint(cameraDock.Target.transform.position);
            RectTransform parentUI = lockonCursor.parent.GetComponent<RectTransform>();
        }
        else
        {
            ClearLockOnTargets();
        }
    }

    public void HandleLocatingLockOnTargets()
    {
        // unitilitymanagerからキャラクターのレイヤーマスクを使用
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());
        if (colliders == null)
            return;

        List<CharacterManager> availableTargets = GetAvailableTargets(colliders);
        if (availableTargets == null)
            return;

        // 上記のリスト全てのベクトルとカメラのベクトルを比較し、画面中央に一番近いものを探す
        var tumpleData = GetOptimalEnemy(availableTargets);

        float degreemum = tumpleData.Item1;
        CharacterManager target = tumpleData.Item2;

        // 求めた一番小さい値が一定値より小さい場合、ターゲティングをオン
        if (Mathf.Abs(degreemum) <= lockonThreshold)
        {
            nearestLockOnTarget = target;
        }
    }

    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        lockOnCamera.LookAt = null;

        cameraDock.lookAtTarget = null;

        player.playerCombatManager.SetTarget(nearestLockOnTarget);
        lockOnCamera.Priority = anLockOnPriority;
        player.playerNetworkManager.isLockedOn.Value = false;
        lockonCursor.GetComponent<Image>().enabled = false;
    }
    public void SetLockOnTargets(CharacterManager target)
    {
        lockonCursor.GetComponent<Image>().enabled = true;
        lockonCursor.GetComponentInParent<LockOnCursor>().Initialize(target.myLockOnTransform.transform, target.myLockOnTransform.transform.gameObject);

        cameraDock.lookAtTarget = target.myLockOnTransform.transform;

        //lockOnCamera.LookAt = target.myLockOnTransform.transform;
        lockOnCamera.LookAt = cameraDock.Target.transform;
        lockOnCamera.Priority = lockOnPriority;
    }
    public IEnumerator WaitThenFindNewTarget()
    {
        yield return new WaitForSeconds(2.0f);
        while (player.isPerformingAction)
        {
            yield return null;
        }
        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            SetLockOnTargets(nearestLockOnTarget);
            player.playerNetworkManager.isLockedOn.Value = true;
        }
        yield return null;
    }

    private List<CharacterManager> GetAvailableTargets(Collider[] colliders)
    {
        List<CharacterManager> availableTargets = new List<CharacterManager>();

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

            if (lockOnTarget != null)
            {
                // 候補が死んでたり自分のオブジェクトの場合はスルー
                if (lockOnTarget.isDead.Value)
                    continue;

                if (lockOnTarget.transform.root == player.transform.root)
                    continue;

                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                //float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, Vector3.Scale(PlayerCamera.instance.transform.forward, new Vector3(1, 0, 1)).normalized);

                // 候補がview内にいるかどうかを距離と角度で選定
                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;
                    // raycastが障害物に邪魔されていればスルー
                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position,
                        lockOnTarget.characterCombatManager.lockOnTransform.position,
                        out hit, WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        continue;
                    }
                    else
                    {
                        // 最終候補に追加
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }
        return availableTargets;
    }

    // リスト全てのベクトルとカメラのベクトルを比較し、画面中央に一番近いものを探す
    // degreep: カメラの前方ベクトルX,Z成分からなる角度
    private (float, CharacterManager) GetOptimalEnemy(List<CharacterManager> availableTargets)
    {
        float degreep = Mathf.Atan2(cameraTrn.forward.x, cameraTrn.forward.z);
        float degreemum = Mathf.PI * 2;
        CharacterManager target = null;

        foreach (var enemy in availableTargets)
        {
            // pos: 敵からカメラへ向けたベクトル
            // pos2: カメラから敵に向けたベクトル(水平方向に制限して正規化)
            Vector3 pos = cameraTrn.position - enemy.transform.position;
            Vector3 pos2 = enemy.transform.position - cameraTrn.position;
            pos2.y = 0.0f;
            pos2.Normalize();

            // degree: pos2のX,Z成分からなる角度. カメラの前方からどれだけ回転しているか
            float degree = Mathf.Atan2(pos2.x, pos2.z);
            // degreeを-180°～180°に正規化
            degree = degreeNormalize(degree, degreep);

            // pos.magnitude: 敵とカメラの距離
            // pos.magnitudeに応じて角度に重みをかけ、距離が近いほど角度の重みが大きく選好される
            degree = degree + degree * (pos.magnitude / 50) * lockonFactor;

            // Mathf.Abs(degreemum): 以前に記録された最小角度差の絶対値
            // Mathf.Abs(degree): 現在の角度差の絶対値
            if (Mathf.Abs(degreemum) >= Mathf.Abs(degree))
            {
                degreemum = degree;
                target = enemy;
            }
        }
        return (degreemum, target);
    }

    // マウス、右スティック入力時の処理
    public void GetLockonTargetLeftOrRight(string direction)
    {
        CharacterManager target;

        // to do use a layermask
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());
        if (colliders == null)
            return;

        List<CharacterManager> availableTargets = GetAvailableTargets(colliders);
        if (availableTargets == null)
            return;

        // 3. 2のリスト全てのベクトルとカメラのベクトルを比較し、画面中央に一番近いものを探す
        if (direction.Equals("left"))
        {
            // 左入力時
            var tumpleData = GetEnemyLeftOrRight(availableTargets, "left");

            target = tumpleData;
        }
        else
        {
            // 右入力時
            var tumpleData = GetEnemyLeftOrRight(availableTargets, "right");

            target = tumpleData;
        }
        nearestLockOnTarget = target;
    }
    private CharacterManager GetEnemyLeftOrRight(List<CharacterManager> availableTargets, string direction)
    {
        float degreep = Mathf.Atan2(cameraTrn.forward.x, cameraTrn.forward.z);
        float degreemum = Mathf.PI * 2;
        CharacterManager target = nearestLockOnTarget;
        //Vector3 currentLookAt = playerCamera.GetLookAtPosition();

        foreach (var enemy in availableTargets)
        {
            if (enemy == nearestLockOnTarget)
            {
                continue;
            }
            // pos: 敵からカメラへ向けたベクトル
            // pos2: カメラから敵に向けたベクトル(水平方向に制限して正規化)
            Vector3 pos = cameraTrn.position - enemy.transform.position;
            Vector3 pos2 = enemy.transform.position - cameraTrn.position;
            pos2.y = 0.0f;
            pos2.Normalize();

            // degree: pos2のX,Z成分からなる角度. カメラの前方からどれだけ回転しているか
            float degree = Mathf.Atan2(pos2.x, pos2.z);
            // degreeを-180°～180°に正規化
            degree = degreeNormalize(degree, degreep);
            if (direction.Equals("left"))
            {
                if (degree < 0)
                {
                    // enemyが画面中央より右側にいる場合候補から外す
                    continue;
                }
            }
            else
            {
                if (degree > 0)
                {
                    // enemyが画面中央より左側にいる場合候補から外す
                    continue;
                }
            }
            // pos.magnitude: 敵とカメラの距離
            // pos.magnitudeに応じて角度に重みをかけ、距離が近いほど角度の重みが大きく選好される
            degree = degree + degree * (pos.magnitude / 50) * lockonFactor;
            // Mathf.Abs(degreemum): 以前に記録された最小角度差の絶対値
            // Mathf.Abs(degree): 現在の角度差の絶対値
            if (Mathf.Abs(degreemum) >= Mathf.Abs(degree))
            {
                degreemum = degree;
                target = enemy;
            }
        }
        return target;
    }

    // degreeを-180°～180°に正規化
    private float degreeNormalize(float degree, float degreep)
    {
        if (Mathf.PI <= (degreep - degree))
        {
            // degreep (カメラの前方ベクトル) とdegree (カメラから敵へのベクトル) との角度差が180°以上
            // degreeから360°引いて正規化(-180から180に制限)
            degree = degreep - degree - Mathf.PI * 2;
        }
        else if (-Mathf.PI >= (degreep - degree))
        {
            // degreep (カメラの前方ベクトル) とdegree (カメラから敵へのベクトル) との角度差が-180°以下
            // degreeから360°足して正規化(-180から180に制限)
            degree = degreep - degree + Mathf.PI * 2;
        }
        else
        {
            // そのままdegreeを使用
            degree = degreep - degree;
        }
        return degree;
    }
}
