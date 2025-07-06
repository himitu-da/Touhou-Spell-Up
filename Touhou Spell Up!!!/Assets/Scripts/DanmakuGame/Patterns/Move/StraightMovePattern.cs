using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "StraightMovePattern", menuName = "Touhou Spell Up/Danmaku/Move/Straight")]
public class StraightMovePattern : MovePatternBase
{
    [SerializeField] private Vector2 _direction = Vector2.down;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _duration = 2f; // 0以下で無限

    public override async UniTask ExecuteMove(Mover mover, CancellationToken token)
    {
        float elapsedTime = 0f;
        Vector3 moveVector = new Vector3(_direction.x, _direction.y, 0).normalized * _speed;

        while (!token.IsCancellationRequested)
        {
            if (_duration > 0 && elapsedTime >= _duration)
            {
                break;
            }

            mover.transform.position += moveVector * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
