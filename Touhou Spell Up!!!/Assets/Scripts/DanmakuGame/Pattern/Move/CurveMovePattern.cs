using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "CRV_", menuName = "Danmaku/Pattern/Move/Curve", order = 0)]
public class CurveMovePattern : MovePatternBase
{
    [Header("移動設定")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float angleChangeRate = 30f; // 度/秒

    [Header("初期方向設定")]
    [SerializeField] private bool overrideInitialDirection = false;
    [SerializeField] private float initialDirection = -90f; // 下向き（度）

    public override async UniTask ExecuteMove(MovementState state, CancellationToken token)
    {
        // 初期角度の決定
        if (overrideInitialDirection)
        {
            state.Rotation = Quaternion.Euler(0, 0, initialDirection);
        }

        while (!token.IsCancellationRequested)
        {
            // 角度を徐々に変化
            float currentAngle = state.Rotation.eulerAngles.z;
            currentAngle += angleChangeRate * Time.deltaTime;
            state.Rotation = Quaternion.Euler(0, 0, currentAngle);

            // 向きに基づいて速度を設定
            state.Velocity = state.Rotation * Vector3.up * speed;

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
