using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    //[SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject lifeGaugePrefab;

    [Header("攻撃パターン")]
    [SerializeField] ShootPatternBase attackPattern;

    private Health _health;
    private GameObject _lifeGaugeInstance;

    void Start()
    {
        _health = GetComponent<Health>();
        if (lifeGaugePrefab != null)
        {
            _lifeGaugeInstance = Instantiate(lifeGaugePrefab, transform.position, Quaternion.identity, transform);
            
            // 生成したインスタンスからCanvasコンポーネントを取得
            var canvas = _lifeGaugeInstance.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Render ModeをWorld Spaceに設定
                canvas.renderMode = RenderMode.WorldSpace;
                // メインカメラをEvent Cameraに設定
                canvas.worldCamera = Camera.main;
            }

            var lifeGaugeController = _lifeGaugeInstance.GetComponent<EnemyLifeGaugeController>();
            if (lifeGaugeController != null)
            {
                lifeGaugeController.Initialize(_health);
            }
        }

        if (attackPattern != null)
        {
            // 第2引数にnullを渡して実行。使用する弾はattackPattern自身がoverrideBulletPrefabで指定する想定
            attackPattern.Execute(transform, null, this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}
