using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(fileName = "BASIC_", menuName = "Touhou Spell Up/Danmaku/Shoot/Basic Shot")]
public class BasicShotPattern : ShootPatternBase
{
    public override async UniTask ExecuteShootFromPoint(GameEntityController controller, EmissionData emissionData, CancellationToken token)
    {
        if (_entity == null || _entity.Prefab == null)
        {
            Debug.LogError("発射する弾が指定されていません！", this);
            return;
        }

        // EmissionDataから基準位置を計算
        Vector3 baseSpawnPosition = controller.transform.position + controller.transform.rotation * emissionData.localPosition;

        // 基準角度を計算
        float angle;
        if (aimAtPlayer)
        {
            angle = AngleUtility.GetAngleToPlayer(baseSpawnPosition) + 180f;
        }
        else
        {
            angle = controller.transform.eulerAngles.z + emissionData.localAngle;
        }
        angle += directionOffset;

        Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

        // 最終的な発射位置を計算
        Vector3 finalSpawnPosition = CalculateFinalSpawnPosition(baseSpawnPosition, angle);

        // spawnPointの位置と角度で、指定された弾を1つ生成する
        controller.InstantiateProperty(_entity, finalSpawnPosition, spawnRotation);

        await UniTask.CompletedTask;
    }
}
