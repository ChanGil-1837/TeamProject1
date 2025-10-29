using UnityEngine;

public sealed class NormalWeapon : Weapon
{
    public override void Fire(IEnemy enemy)
    {
        // ���� üũ
        if (CheckCondition(enemy) == false) return;

        Vector3 direction = GetDirection(enemy);

        FireProjectile(direction);

        AfterFire();
    }
}
