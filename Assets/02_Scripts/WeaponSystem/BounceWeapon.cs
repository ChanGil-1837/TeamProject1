using UnityEngine;


public sealed class BounceWeapon : Weapon
{
    [Header("�ٿ")]
    [SerializeField] private int maxBounceCount = 3;

    public int BounceLevel => bounceLevel;
    private int bounceLevel;

    public override void Fire(IEnemy enemy)
    {
        if (!isAvailable) return;
        
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
