using UnityEngine;

public sealed class NormalWeapon : Weapon
{
    protected override void Awake()
    {
        isAvailable = true;
        base.Awake();
        
    }
    public override void Fire(IEnemy enemy)
    {
        if (!isAvailable) return;
        // ���� üũ
        if (CheckCondition(enemy) == false) return;

        Vector3 direction = GetDirection(enemy);

        FireProjectile(direction);

        AfterFire();
    }
}
