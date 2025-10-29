using UnityEngine;


public sealed class BounceWeapon : Weapon
{
    [Header("바운스")]
    [SerializeField] private int maxBounceCount = 3;

    public int BounceLevel => bounceLevel;
    private int bounceLevel;

    public override void Fire(IEnemy enemy)
    {
        Vector3 direction = (enemy.Transform.position - transform.position).normalized;

        Projectile projectile = FireProjectile(direction);

        // 바운스 투사체 설정
        if (projectile is BounceProjectile bounceProjectile)
        {
            bounceProjectile.SetBounceCount(maxBounceCount);
        }
    }


    // 바운스 추가 업그레이드
    public void UpgradeBounceCount(int bounceIncrease)
    {
        maxBounceCount += bounceIncrease;
        bounceLevel++;
    }
}
