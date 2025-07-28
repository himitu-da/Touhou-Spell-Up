using UnityEngine;

[CreateAssetMenu(fileName = "BLT_", menuName = "Touhou Spell Up/Danmaku/Bullet", order = 1)]
public class Bullet : GameEntity
{
    // 親クラスのプロパティを隠蔽し、より具体的な型を返す新しいプロパティを定義します。
    // これにより、シリアライズのエラーを回避しつつ、型安全性を維持します。
    public new BulletProperty Property => base.Property as BulletProperty;
}
