using UnityEngine;

/// <summary>
/// プレイヤーに関するユーティリティ機能を提供します。
/// </summary>
public static class PlayerUtility
{
    private static Transform _playerTransform;

    /// <summary>
    /// プレイヤーのTransformを取得します。
    /// 初回呼び出し時にプレイヤーを検索し、結果をキャッシュします。
    /// </summary>
    /// <returns>プレイヤーのTransform。見つからない場合はnull。</returns>
    public static Transform GetPlayerTransform()
    {
        if (_playerTransform == null)
        {
            // PlayerControllerを持つオブジェクトを検索
            var playerController = Object.FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                _playerTransform = playerController.transform;
            }
            else
            {
                Debug.LogWarning("PlayerControllerが見つかりませんでした。");
                return null;
            }
        }
        return _playerTransform;
    }

    /// <summary>
    /// プレイヤーがシーンから破棄された場合などにキャッシュをクリアします。
    /// </summary>
    public static void ClearCache()
    {
        _playerTransform = null;
    }
}
