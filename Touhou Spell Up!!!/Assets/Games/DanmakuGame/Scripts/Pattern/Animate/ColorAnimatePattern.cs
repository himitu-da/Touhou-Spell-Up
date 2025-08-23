using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "ColorAnimatePattern", menuName = "Danmaku/Pattern/Animate/Color")]
public class ColorAnimatePattern : AnimatePatternBase
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
    [SerializeField] private ColorReference _startColor = new ColorReference { useConstant = true, constantValue = Color.white };
    [SerializeField] private ColorReference _endColor = new ColorReference { useConstant = true, constantValue = Color.white };

    [Header("Delta Mode")]
    [SerializeField] private bool _useStartValue = false;
    [SerializeField] private ColorReference _deltaStartColor = new ColorReference { useConstant = true, constantValue = Color.white };
    [SerializeField] private ColorReference _deltaColorPerSec = new ColorReference { useConstant = true, constantValue = new Color(0, 0, 0, 0) };


    public override async UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        var spriteRenderer = controller.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the entity.", controller);
            return;
        }

        if (_mode == AnimateMode.Lerp)
        {
            await ExecuteLerp(spriteRenderer, token);
        }
        else
        {
            await ExecuteDelta(spriteRenderer, token);
        }
    }

    private async UniTask ExecuteLerp(SpriteRenderer spriteRenderer, CancellationToken token)
    {
        float elapsedTime = 0f;
        float duration = _duration.Value;
        Color startColor = _startColor.Value;
        Color endColor = _endColor.Value;

        while (elapsedTime < duration)
        {
            if (token.IsCancellationRequested) return;

            float t = duration > 0 ? elapsedTime / duration : 1f;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);

            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        spriteRenderer.color = endColor;
    }

    private async UniTask ExecuteDelta(SpriteRenderer spriteRenderer, CancellationToken token)
    {
        if (_useStartValue)
        {
            spriteRenderer.color = _deltaStartColor.Value;
        }

        float duration = _duration.Value;
        if (duration <= 0) // Infinite
        {
            while (!token.IsCancellationRequested)
            {
                spriteRenderer.color += _deltaColorPerSec.Value * Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        else
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                if (token.IsCancellationRequested) return;
                spriteRenderer.color += _deltaColorPerSec.Value * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}
