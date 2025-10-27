using UnityEngine;

public class NormalWeapon : Weapon
{
    public override void Fire()
    {
        Vector3 direction = (target.position - transform.position).normalized;

        Projectile projectile = GetFromPool();
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        projectile.SetDirection(direction);
    }
}
