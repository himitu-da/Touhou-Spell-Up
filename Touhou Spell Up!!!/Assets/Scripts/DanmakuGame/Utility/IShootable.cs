using UnityEngine;

public interface IShootable
{
    void InstantiateBullet(Bullet bullet, Vector3 position, Quaternion rotation);
}
