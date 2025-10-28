using UnityEngine;


public sealed class BounceWeapon : Weapon
{
    [Header("최대 바운스 수")]
    [SerializeField] private int maxBounceCount = 3;

    public override void Fire()
    {
        Projectile projectile = GetFromPool();

        Vector3 direction = (target.position - transform.position).normalized;

        SetProjectileTransform(projectile, direction);

        // 바운스 투사체 설정
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
