using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "RotateAnimatePattern", menuName = "Danmaku/Pattern/Animate/Rotate")]
public class RotateAnimatePattern : AnimatePatternBase
{
    public enum AnimateMode
    {
        Lerp,
        Delta
    }

    [SerializeField] private AnimateMode _mode = AnimateMode.Lerp;

    [Header("Common")]
    [SerializeField] private FloatReference _duration = new FloatReference { useConstant = true, constantValue = 1f };

    [Header("Lerp Mode")]
    [SerializeField] private FloatReference _startAngle = new FloatReference { useConstant = true, constantValue = 0f };
    [SerializeField] private FloatReference _endAngle = new FloatReference { useConstant = true, constantValue = 360f };

    [Header("Delta Mode")]
    [SerializeField] private bool _useStartValue = false;
    [SerializeField] private FloatReference _deltaStartAngle = new FloatReference { useConstant = true, constantValue = 0f };
    [SerializeField] private FloatReference _deltaAnglePerSec = new FloatReference { useConstant = true, constantValue = 0f };

    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (_mode == AnimateMode.Lerp)
        {
            await ExecuteLerp(controller.transform, token);
        }
        else
        {
            await ExecuteDelta(controller.transform, token);
        }
    }

    private async UniTask ExecuteLerp(Transform transform, CancellationToken token)
    {
        float elapsedTime = 0f;
        float duration = _duration.Value;
        float startAngle = _startAngle.Value;
        float endAngle = _endAngle.Value;

        while (elapsedTime < duration)
        {
            if (token.IsCancellationRequested) return;

            float t = duration > 0 ? elapsedTime / duration : 1f;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        transform.rotation = Quaternion.Euler(0, 0, endAngle);
    }

    private async UniTask ExecuteDelta(Transform transform, CancellationToken token)
    {
        if (_useStartValue)
        {
            transform.rotation = Quaternion.Euler(0, 0, _deltaStartAngle.Value);
        }

        float duration = _duration.Value;
        if (duration <= 0) // Infinite
        {
            while (!token.IsCancellationRequested)
            {
                transform.Rotate(0, 0, _deltaAnglePerSec.Value * Time.deltaTime);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        else
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                if (token.IsCancellationRequested) return;
                transform.Rotate(0, 0, _deltaAnglePerSec.Value * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}
