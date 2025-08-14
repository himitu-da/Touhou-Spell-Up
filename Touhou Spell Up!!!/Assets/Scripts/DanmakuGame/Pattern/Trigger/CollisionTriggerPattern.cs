using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// 衝突ベースでトリガーを発火するパターン
/// </summary>
[CreateAssetMenu(fileName = "COLLISION_", menuName = "Danmaku/Pattern/Trigger/Collision Trigger")]
public class CollisionTriggerPattern : TriggerPatternBase
{
    [Header("衝突トリガー設定")]
    [Tooltip("特定のタグとの衝突のみを監視するか")]
    [SerializeField] private BoolReference _filterByTag = new BoolReference { useConstant = true, constantValue = false };
    
    [Tooltip("監視対象のタグリスト（filterByTagがtrueの場合のみ有効）")]
    [SerializeField] private List<string> _targetTags = new List<string>();
    
    [Tooltip("特定のレイヤーとの衝突のみを監視するか")]
    [SerializeField] private BoolReference _filterByLayer = new BoolReference { useConstant = true, constantValue = false };
    
    [Tooltip("監視対象のレイヤーマスク（filterByLayerがtrueの場合のみ有効）")]
    [SerializeField] private LayerMaskReference _targetLayers = new LayerMaskReference { useConstant = true, constantValue = -1 };
    
    [Tooltip("衝突回数の閾値（0以下の場合は無制限）")]
    [SerializeField] private IntReference _collisionCountThreshold = new IntReference { useConstant = true, constantValue = 1 };

    private int _currentCollisionCount = 0;

    /// <summary>
    /// イベントベースの監視も併用するためリスナーを登録
    /// </summary>
    protected override void RegisterEventListeners(GameEntityController controller)
    {
        base.RegisterEventListeners(controller);
        controller.OnCollisionDetected.AddListener(OnCollisionDetected);
    }

    /// <summary>
    /// イベントリスナーを解除
    /// </summary>
    protected override void UnregisterEventListeners(GameEntityController controller)
    {
        base.UnregisterEventListeners(controller);
        if (controller != null)
        {
            controller.OnCollisionDetected.RemoveListener(OnCollisionDetected);
        }
    }

    /// <summary>
    /// 衝突検知イベントハンドラ
    /// </summary>
    private void OnCollisionDetected(Collider2D collider)
    {
        if (_targetController != null && IsValidCollision(collider))
        {
            _currentCollisionCount++;
            
            if (_debugLog.Value)
                Debug.Log($"[CollisionTriggerPattern] Valid collision detected with {collider.name}. Count: {_currentCollisionCount}/{_collisionCountThreshold.Value}", this);
            
            if (CheckCollisionCondition())
            {
                // イベントベースで即座にトリガー発火
                _ = FireTrigger(_targetController, _cancellationToken);
            }
        }
    }

    /// <summary>
    /// 衝突が有効かどうかをチェック
    /// </summary>
    private bool IsValidCollision(Collider2D collider)
    {
        // タグフィルタリング
        if (_filterByTag.Value && _targetTags.Count > 0)
        {
            bool tagMatched = false;
            foreach (string tag in _targetTags)
            {
                if (collider.CompareTag(tag))
                {
                    tagMatched = true;
                    break;
                }
            }
            if (!tagMatched) return false;
        }

        // レイヤーフィルタリング
        if (_filterByLayer.Value)
        {
            int colliderLayer = 1 << collider.gameObject.layer;
            if ((_targetLayers.Value & colliderLayer) == 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 衝突回数の条件をチェック
    /// </summary>
    private bool CheckCollisionCondition()
    {
        if (_collisionCountThreshold.Value <= 0)
        {
            return true; // 無制限の場合は常にtrue
        }
        
        return _currentCollisionCount >= _collisionCountThreshold.Value;
    }

    /// <summary>
    /// トリガー条件をチェック（ポーリングベース）
    /// イベントベースで十分だが、万一のために実装
    /// </summary>
    protected override bool CheckTriggerCondition(GameEntityController controller)
    {
        return controller.IsCollided && CheckCollisionCondition() && 
               (controller.LastCollider == null || IsValidCollision(controller.LastCollider));
    }

    /// <summary>
    /// トリガー発火時の追加処理
    /// </summary>
    protected override UniTask OnTriggerFired(GameEntityController controller, CancellationToken token)
    {
        if (_debugLog.Value)
        {
            string colliderName = controller.LastCollider != null ? controller.LastCollider.name : "Unknown";
            Debug.Log($"[CollisionTriggerPattern] Collision trigger fired! Last collider: {colliderName}, Total collisions: {_currentCollisionCount}", this);
        }
        
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// トリガーリセット時に衝突回数もリセット
    /// </summary>
    public override void ResetTrigger()
    {
        base.ResetTrigger();
        _currentCollisionCount = 0;
        
        if (_debugLog.Value)
            Debug.Log($"[CollisionTriggerPattern] Collision count reset", this);
    }
}
