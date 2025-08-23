using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "ScaleAnimatePattern", menuName = "Danmaku/Pattern/Animate/Scale")]
public class ScaleAnimatePattern : AnimatePatternBase
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
    [SerializeField] private Vector2Reference _startScale = new Vector2Reference { useConstant = true, constantValue = Vector2.one };
    [SerializeField] private Vector2Reference _endScale = new Vector2Reference { useConstant = true, constantValue = Vector2.one };

    [Header("Delta Mode")]
    [SerializeField] private bool _useStartValue = false;
    [SerializeField] private Vector2Reference _deltaStartScale = new Vector2Reference { useConstant = true, constantValue = Vector2.one };
    [SerializeField] private Vector2Reference _deltaScalePerSec = new Vector2Reference { useConstant = true, constantValue = Vector2.zero };

    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        if (_mode == AnimateMode.Lerp)
        {
            await ExecuteLerp(controller.State, token);
        }
        else
        {
            await ExecuteDelta(controller.State, token);
        }
    }

    private async UniTask ExecuteLerp(GameEntityState state, CancellationToken token)
    {
        float elapsedTime = 0f;
        float duration = _duration.Value;
        Vector2 startScale = _startScale.Value;
        Vector2 endScale = _endScale.Value;

        while (elapsedTime < duration)
        {
            if (token.IsCancellationRequested) return;

            float t = duration > 0 ? elapsedTime / duration : 1f;
            state.ScaleMultiplier = Vector2.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        state.ScaleMultiplier = endScale;
    }

    private async UniTask ExecuteDelta(GameEntityState state, CancellationToken token)
    {
        if (_useStartValue)
        {
            state.ScaleMultiplier = _deltaStartScale.Value;
        }

        float duration = _duration.Value;
        if (duration <= 0) // Infinite
        {
            while (!token.IsCancellationRequested)
            {
                state.ScaleMultiplier += (Vector3)_deltaScalePerSec.Value * Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        else
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                if (token.IsCancellationRequested) return;
                state.ScaleMultiplier += (Vector3)_deltaScalePerSec.Value * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}
