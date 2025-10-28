using UnityEngine;

public sealed class NormalWeapon : Weapon
{
    public override void Fire()
    {
        Projectile projectile = GetFromPool();

        Vector3 direction = (target.position - transform.position).normalized;

        SetProjectileTransform(projectile, direction);
    }
}
