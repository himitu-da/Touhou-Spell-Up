using UnityEngine;

/// <summary>
/// 共有される角度（float）のリソース
/// </summary>
[CreateAssetMenu(fileName = "SharedAngle", menuName = "Touhou Spell Up/Danmaku/Shared Value/Angle (float)")]
public class SharedAngle : SharedResource<float>
{
    // SharedResource<float>を継承するだけで、
    // 必要な機能（Valueプロパティ、Resetメソッドなど）はすべて実装済み。
    // 今後、角度に特化した処理（例：ラジアンへの変換など）が必要になった場合は、
    // このクラスに追記していく。
}
