using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Mover : MonoBehaviour
{
    [SerializeField]
    private PatternBase _movePattern;

    [SerializeField, Tooltip("このMoverが使用するShooter")]
    private Shooter _shooter;

    private CancellationTokenSource _cancellationTokenSource;
    void Start()
    {
                // Shooterが設定されていなければ、自身のGameObjectから取得を試みる
        if (_shooter == null)
        {
            _shooter = GetComponent<Shooter>();
        }

        if (_movePattern == null)
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _movePattern.Execute(this, _shooter, _cancellationTokenSource.Token).Forget();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
