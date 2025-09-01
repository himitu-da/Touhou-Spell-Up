using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;


public enum SpawnPointType
{
    RelativeToShooter, // Shooterの位置を基準にする
    Absolute,          // ワールド絶対座標
    RelativeToPlayer,  // プレイヤーを基準にする (今後の拡張用)
}

public abstract class ShootPatternBase : PatternBase
{
    [Header("弾の設定")]
    [SerializeField] protected GameEntityReference _entity;

    [Header("発射地点の設定")]
    [SerializeField] protected SpawnPointTypeReference spawnPointType = new SpawnPointTypeReference { useConstant = true, constantValue = SpawnPointType.RelativeToShooter };
    [SerializeField] protected Vector3Reference positionOffset = new Vector3Reference { useConstant = true, constantValue = Vector3.zero };
    [Tooltip("射撃中にシューターの位置に追従するか")]
    [SerializeField] protected BoolReference followShooterPosition = new BoolReference { useConstant = true, constantValue = false };

    [Header("内部角度状態管理")]
    [Tooltip("内部で管理される角度状態（度）- 定数値でも値は保持されます")]
    [SerializeField] protected FloatReference internalAngle = new FloatReference { useConstant = true, constantValue = 0f };
    [Tooltip("内部角度の変更をコンソールログに出力するか（デバッグ用）")]
    [SerializeField] private bool logInternalAngleChanges = false;
    
    // 定数値の場合の内部角度保持用（シリアライズされない）
    [System.NonSerialized]
    private float _constantInternalAngle = 0f;
    [System.NonSerialized]
    private bool _hasInitializedConstantAngle = false;

    [Header("発生源シェイプ（オプション）")]
    [SerializeField] protected EmissionReference emissionShape;
    [Tooltip("順次発射か（同時ならfalse）")]
    [SerializeField] protected BoolReference sequential = new BoolReference { useConstant = true, constantValue = false };
    [Tooltip("順次発射時の間隔（秒）")]
    [SerializeField] protected FloatReference emissionInterval = new FloatReference { useConstant = true, constantValue = 0.1f };

    [Header("発射角度の設定")]
    [SerializeField] protected FloatReference directionOffset = new FloatReference { useConstant = true, constantValue = 0f };
    [Tooltip("パターン開始時のみ自機を狙うか")]
    [SerializeField] protected BoolReference aimAtPlayer = new BoolReference { useConstant = true, constantValue = false };
    [Tooltip("各Shot実行時に常に自機を狙うか（internalAngleは相対角度として適用）")]
    [SerializeField] protected BoolReference alwaysAimToPlayer = new BoolReference { useConstant = true, constantValue = false };
    [Tooltip("パターン実行開始時にinternalAngleをdirectionOffsetで初期化するか")]
    [SerializeField] protected BoolReference initializeAngleOnStart = new BoolReference { useConstant = true, constantValue = false };
    [Tooltip("パターン終了後に次回実行時の角度に加算するオフセット値")]
    [SerializeField] protected FloatReference postExecutionAngleOffset = new FloatReference { useConstant = true, constantValue = 0f };
    [Header("発射地点の極座標オフセット")]
    [Tooltip("発射地点からのオフセット距離")]
    [SerializeField] protected FloatReference spawnRadius = new FloatReference { useConstant = true, constantValue = 0f };

    // ExecuteImplのシグネチャを変更
    public override UniTask ExecuteImpl(GameEntityController controller, CancellationToken token)
    {
        // 既存のExecuteImplのロジックをここに移動、またはサブクラスに委譲
        // このクラス自体は抽象なので、サブクラスに実装を強制する
        return ExecuteShoot(controller, token);
    }

    // ShootPatternの本体。Emissionの有無で処理を分岐し、最終的にExecuteShootFromPointを呼び出す
    public virtual async UniTask ExecuteShoot(GameEntityController controller, CancellationToken token)
    {
        var emissions = new List<EmissionData>();

        if (emissionShape == null || emissionShape.Value == null)
        {
            // Emissionがない場合、単一の発生源として動作
            emissions.Add(new EmissionData
            {
                localPosition = positionOffset.Value, // 従来の位置オフセット
                localAngle = 0 // 角度はサブクラスで計算
            });
        }
        else
        {
            // Emissionがある場合、そこから発生源リストを取得
            emissions.AddRange(emissionShape.Value.GetEmissions(controller));
        }

        // 各発生源から弾を発射
        foreach (var emission in emissions)
        {
            if (token.IsCancellationRequested) break;

            await ExecuteShootFromPoint(controller, emission, token);

            if (sequential.Value && emissionInterval.Value > 0)
            {
                await UniTask.Delay((int)(emissionInterval.Value * 1000), cancellationToken: token);
            }
        }
    }

    // サブクラスは、このメソッドを実装して「１つの発生源からどのように弾を撃つか」を定義する
    public abstract UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token);

    protected float GetAimAngle(GameEntityController controller, Vector3 spawnPosition)
    {
        if (aimAtPlayer.Value)
        {
            // 180度回転させて、逆向きになる問題を修正
            return AngleUtility.GetAngleToPlayer(spawnPosition) + 180f;
        }
        else
        {
            return controller.transform.eulerAngles.z;
        }
    }

    // 発射地点を計算するヘルパーメソッド
    protected Vector3 GetSpawnPosition(GameEntityController controller)
    {
        switch (spawnPointType.Value)
        {
            case SpawnPointType.Absolute:
                return positionOffset.Value;

            case SpawnPointType.RelativeToShooter:
                return controller.transform.position + positionOffset.Value;

            case SpawnPointType.RelativeToPlayer:
                var playerTransform = PlayerUtility.GetPlayerTransform();
                if (playerTransform != null)
                {
                    return playerTransform.position + positionOffset.Value;
                }
                else
                {
                    // プレイヤーが見つからない場合はシューターを基準にする
                    Debug.LogWarning("Player not found for RelativeToPlayer spawn type. Falling back to RelativeToShooter.");
                    return controller.transform.position + positionOffset.Value;
                }

            default:
                return controller.transform.position;
        }
    }

    protected Vector3 CalculateFinalSpawnPosition(Vector3 basePosition, float angle)
    {
        if (spawnRadius.Value <= 0)
        {
            return basePosition;
        }
        Vector3 polarOffset = Quaternion.Euler(0, 0, angle) * Vector3.up * spawnRadius.Value;
        return basePosition + polarOffset;
    }

    /// <summary>
    /// パターン開始時の角度を初期化します
    /// </summary>
    protected float InitializeBaseAngle(GameEntityController controller, EmissionData emissionData, Vector3 spawnPosition)
    {
        // InitializeAngleOnStartが有効な場合のみ、directionOffsetで内部角度を初期化
        if (initializeAngleOnStart.Value)
        {
            SetInternalAngle(directionOffset.Value);
        }
        
        // 基準角度を計算
        float baseAngle;
        if (aimAtPlayer.Value)
        {
            // パターン開始時のみ自機狙い
            baseAngle = AngleUtility.GetAngleToPlayer(spawnPosition) + 180f;
        }
        else
        {
            // 通常の角度計算
            baseAngle = controller.transform.eulerAngles.z + emissionData.localAngle;
        }
        
        // 内部角度を加算（自機狙いの場合は相対角度として機能）
        return baseAngle + GetInternalAngle();
    }

    /// <summary>
    /// 各Shot実行時の角度を計算します
    /// </summary>
    protected float CalculateShootAngle(GameEntityController controller, EmissionData emissionData, Vector3 spawnPosition)
    {
        float baseAngle;
        
        if (alwaysAimToPlayer.Value)
        {
            // 毎回自機狙い
            baseAngle = AngleUtility.GetAngleToPlayer(spawnPosition) + 180f;
        }
        else if (aimAtPlayer.Value)
        {
            // パターン開始時のみ自機狙い（InitializeBaseAngleで計算済み）
            baseAngle = AngleUtility.GetAngleToPlayer(spawnPosition) + 180f;
        }
        else
        {
            // 通常の角度計算
            baseAngle = controller.transform.eulerAngles.z + emissionData.localAngle;
        }
        
        // 内部角度を加算（自機狙いの場合は相対角度として機能）
        return baseAngle + GetInternalAngle();
    }

    /// <summary>
    /// パターン終了時に角度のオフセットを適用します
    /// </summary>
    protected void ApplyPostExecutionAngleOffset()
    {
        if (postExecutionAngleOffset.Value != 0f)
        {
            float newAngle = GetInternalAngle() + postExecutionAngleOffset.Value;
            SetInternalAngle(newAngle);
        }
    }

    /// <summary>
    /// 内部角度の現在値を取得します
    /// </summary>
    protected float GetInternalAngle()
    {
        float result;
        
        if (internalAngle.useConstant)
        {
            // 定数値の場合は内部保持値を使用（初回は設定値を使用）
            if (!_hasInitializedConstantAngle)
            {
                _constantInternalAngle = internalAngle.constantValue;
                _hasInitializedConstantAngle = true;
            }
            result = _constantInternalAngle;
        }
        else if (internalAngle.parameter != null)
        {
            result = internalAngle.parameter.Value;
        }
        else
        {
            result = 0f;
        }
        
        return result;
    }

    /// <summary>
    /// 内部角度の値を設定します（定数値・GameParameter両方に対応）
    /// </summary>
    protected void SetInternalAngle(float value)
    {
        float oldValue = GetInternalAngle();
        
        // 角度を0-360の範囲に正規化
        value = value % 360f;
        if (value < 0)
        {
            value += 360f;
        }

        // デバッグログ（オプション）
        if (logInternalAngleChanges && Application.isPlaying)
        {
            Debug.Log($"[{name}] InternalAngle: {oldValue:F1}° → {value:F1}° (useConstant: {internalAngle.useConstant}, param: {internalAngle.parameter?.name ?? "null"})", this);
        }

        if (internalAngle.useConstant)
        {
            // 定数値の場合は内部保持値に設定
            _constantInternalAngle = value;
            _hasInitializedConstantAngle = true;
            
            // Inspector表示のためにconstantValueも更新（エディタでの確認用）
            #if UNITY_EDITOR
            internalAngle.constantValue = value;
            #endif
        }
        else if (internalAngle.parameter != null)
        {
            // GameParameterが設定されている場合はそちらに値を設定
            internalAngle.parameter.Value = value;
        }
    }

    /// <summary>
    /// 内部角度を基準とした角度を計算します（パターン継続実行用）
    /// </summary>
    protected float CalculateAngleFromInternal(float additionalOffset = 0f)
    {
        return GetInternalAngle() + additionalOffset;
    }

    /// <summary>
    /// 内部角度に値を加算します
    /// </summary>
    protected void AddToInternalAngle(float deltaAngle)
    {
        float newAngle = GetInternalAngle() + deltaAngle;
        SetInternalAngle(newAngle);
    }
}
