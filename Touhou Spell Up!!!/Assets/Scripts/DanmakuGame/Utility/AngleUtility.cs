using UnityEngine;

// staticクラスにすることで、インスタンス化せずにメソッドを呼び出せる
public static class AngleUtility
{
    /// <summary>
    /// 指定した地点からターゲットの方向を向くための角度（Z軸回転）を計算します。
    /// </summary>
    /// <param name="fromPosition">角度を計算する基準点</param>
    /// <param name="targetPosition">目標地点</param>
    /// <param name="offset">角度のオフセット（スプライトの正面が上向きなら90）</param>
    /// <returns>度数法で表された角度</returns>
    public static float GetAngleToTarget(Vector3 fromPosition, Vector3 targetPosition, float offset = 90f)
    {
        Vector3 dir = targetPosition - fromPosition;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset;
    }

    /// <summary>
    /// 指定した地点からプレイヤーキャラクターの方向を向くための角度を計算します。
    /// </summary>
    /// <param name="fromPosition">角度を計算する基準点</param>
    /// <param name="offset">角度のオフセット</param>
    /// <returns>プレイヤーへの角度。プレイヤーが見つからない場合は0を返す。</returns>
    public static float GetAngleToPlayer(Vector3 fromPosition, float offset = 90f)
    {
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            return GetAngleToTarget(fromPosition, player.transform.position, offset);
        }
        else
        {
            // プレイヤーが見つからない場合のデフォルトの挙動
            Debug.LogWarning("PlayerControllerが見つかりませんでした。");
            return 0f;
        }
    }
}
