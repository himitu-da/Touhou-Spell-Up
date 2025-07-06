using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

// 弾を扱えるパターンのための抽象クラス
public abstract class ShootablePattern : PatternBase
{
    [Tooltip("このパターン以下で使われる弾を上書きします")]
    [SerializeField] protected Bullet overrideBullet = null;

    // Executeメソッドをオーバーライドして、弾の上書きロジックを実装
    public override async UniTask Execute(Mover mover, Shooter shooter, Bullet inheritedBullet, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        // 自身に上書き弾が設定されていればそれを使い、なければ親から継承した弾を使う
        Bullet bulletToUse = this.overrideBullet != null ? this.overrideBullet : inheritedBullet;

        // 決定した弾を使って、具体的な処理を実行
        await ExecuteImpl(mover, shooter, bulletToUse, token);
    }
}
