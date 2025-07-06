using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

// 全てのパターンの基底となる抽象クラス
public abstract class PatternBase : ScriptableObject
{
    // MoverとShooterの両方を受け取り、弾の情報を引き継げるExecuteメソッド
    // このメソッドは仮想(virtual)とし、サブクラスでのオーバーライドを可能にする
    public virtual async UniTask Execute(Mover mover, Shooter shooter, Bullet inheritedBullet, CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        // 実際の処理はExecuteImplに委譲する
        await ExecuteImpl(mover, shooter, inheritedBullet, token);
    }

    // サブクラスで具体的な処理を実装するための抽象メソッド
    public abstract UniTask ExecuteImpl(Mover mover, Shooter shooter, Bullet inheritedBullet, CancellationToken token);
}
