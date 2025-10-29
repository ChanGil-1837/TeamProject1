using UnityEngine;


public sealed class BounceWeapon : Weapon
{
    [Header("�ٿ")]
    [SerializeField] private int maxBounceCount = 3;

    public int BounceLevel => bounceLevel;
    private int bounceLevel;

    public override void Fire(IEnemy enemy)
    {
        Vector3 direction = (enemy.Transform.position - transform.position).normalized;

        Projectile projectile = FireProjectile(direction);

        // �ٿ ����ü ����
        if (projectile is BounceProjectile bounceProjectile)
        {
            bounceProjectile.SetBounceCount(maxBounceCount);
        }
    }


    // �ٿ �߰� ���׷��̵�
    public void UpgradeBounceCount(int bounceIncrease)
    {
        maxBounceCount += bounceIncrease;
        bounceLevel++;
    }
}
