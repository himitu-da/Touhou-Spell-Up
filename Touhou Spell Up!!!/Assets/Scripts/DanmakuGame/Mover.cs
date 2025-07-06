using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Mover : MonoBehaviour
{
    [SerializeField]
    private MovePatternBase _movePattern;

    private CancellationTokenSource _cancellationTokenSource;
    void Start()
    {
        if (_movePattern == null)
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _movePattern.Execute(this, _cancellationTokenSource.Token).Forget();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
