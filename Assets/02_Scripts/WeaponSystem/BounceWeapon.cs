using UnityEngine;


public sealed class BounceWeapon : Weapon
{
    [Header("바운스")]
    [SerializeField] private int maxBounceCount = 3;

    public int BounceLevel => bounceLevel;
    private int bounceLevel;

    public override void Fire(IEnemy enemy)
    {
        if (CheckCondition(enemy) == false) return;

        Vector3 direction = GetDirection(enemy);

        Projectile projectile = FireProjectile(direction);

        // 바운스 투사체 설정
        if (projectile is BounceProjectile bounceProjectile)
        {
            bounceProjectile.SetBounceCount(maxBounceCount);
        }

        AfterFire();
    }


    // 바운스 추가 업그레이드
    public void UpgradeBounceCount(int bounceIncrease)
    {
        maxBounceCount += bounceIncrease;
        bounceLevel++;
    }
}
