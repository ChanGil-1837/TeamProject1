using UnityEngine;


public sealed class BounceWeapon : Weapon
{
    [Header("�ִ� �ٿ ��")]
    [SerializeField] private int maxBounceCount = 3;

    public override void Fire()
    {
        Projectile projectile = GetFromPool();

        Vector3 direction = (target.position - transform.position).normalized;

        SetProjectileTransform(projectile, direction);

        // �ٿ ����ü ����
        if (projectile is BounceProjectile bounceProjectile)
        {
            bounceProjectile.SetBounceCount(maxBounceCount);
        }
    }


    public void UpgradeBounceCount(int bounceIncrease)
    {
        maxBounceCount += bounceIncrease;
    }
}
