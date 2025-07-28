using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "StraightMovePattern", menuName = "Touhou Spell Up/Danmaku/Move/Straight")]
public class StraightMovePattern : MovePatternBase
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _duration = 2f; // 0以下で無限

    public override async UniTask ExecuteMove(GameEntityController controller, CancellationToken token)
    {
        float elapsedTime = 0f;

        while (!token.IsCancellationRequested)
        {
            if (_duration > 0 && elapsedTime >= _duration)
            {
                break;
            }

            controller.transform.Translate(Vector3.up * _speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
