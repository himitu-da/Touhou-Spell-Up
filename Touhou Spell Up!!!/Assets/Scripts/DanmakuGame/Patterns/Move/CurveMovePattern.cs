using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "CRV_", menuName = "Touhou Spell Up/Danmaku/Move Pattern/Curve", order = 0)]
public class CurveMovePattern : MovePatternBase
{
    [Header("移動設定")]
    [SerializeField] private bool overrideSpeed = false;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float angleChangeRate = 30f; // 度/秒

    [Header("初期方向設定")]
    [SerializeField] private bool overrideInitialDirection = false;
    [SerializeField] private float initialDirection = -90f; // 下向き（度）

    public override async UniTask ExecuteMove(IMovable movable, CancellationToken token)
    {
        // 初期角度の決定
        float currentAngle;
        if (overrideInitialDirection)
        {
            currentAngle = initialDirection;
        }
        else
        {
            // 現在の角度（ShootPatternで設定された角度）を使用
            currentAngle = movable.transform.eulerAngles.z;
        }

        // EnemyBulletからspeedを取得する試み
        var monoBehaviour = movable as MonoBehaviour;
        var bulletComponent = monoBehaviour?.GetComponent<EnemyBullet>();
        float moveSpeed = speed;
        if (!overrideSpeed && bulletComponent != null && bulletComponent.Property != null)
        {
            moveSpeed = bulletComponent.Property.Speed;
        }


        while (!token.IsCancellationRequested && movable != null)
        {
            // 角度を徐々に変化
            currentAngle += angleChangeRate * Time.deltaTime;
            movable.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            // オブジェクトの前方（この場合はローカルのY軸正方向、つまり画像の上方向）に移動
            movable.transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
