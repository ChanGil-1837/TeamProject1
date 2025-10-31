using UnityEngine;


public sealed class BounceWeapon : Weapon
{
    [SerializeField] private int maxBounceCount = 3;

    public int BounceLevel => bounceLevel;
    private int bounceLevel;

    public override void Fire(IEnemy enemy)
    {
        if (CheckCondition(enemy) == false) return;

        Vector3 direction = GetDirection(enemy);

        Projectile projectile = FireProjectile(direction);

        // �ٿ ����ü ����
        if (projectile is BounceProjectile bounceProjectile)
        {
            bounceProjectile.SetBounceCount(maxBounceCount);
        }

        AfterFire();
    }


    // �ٿ �߰� ���׷��̵�
    public void UpgradeBounceCount(int bounceIncrease)
    {
        maxBounceCount += bounceIncrease;
        bounceLevel++;
    }
}
