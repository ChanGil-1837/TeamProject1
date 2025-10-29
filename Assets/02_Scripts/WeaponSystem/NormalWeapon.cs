using UnityEngine;

public sealed class NormalWeapon : Weapon
{
    public override void Fire(IEnemy enemy)
    {
        Vector3 direction = (enemy.Transform.position - transform.position).normalized;

        FireProjectile(direction);
    }
}
