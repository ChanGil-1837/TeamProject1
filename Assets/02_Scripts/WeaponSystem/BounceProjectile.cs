using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BounceProjectile : Projectile
{
    private IEnemy lastEnemy;   // ������ ��Ʈ ��

    private int currentBounce;  // ���� �ٿ ��

    // �ٿ �� ����
    public void SetBounceCount(int bounces)
    {
        currentBounce = bounces;
    }

    // �� ��Ʈ
    protected override void EnemyHit(IEnemy enemy)
    {
        if (enemy != null)
        {
            //enemy.TakeDamage(weapon.Damage);

            lastEnemy = enemy;

            currentBounce--;

            // Bounce Logic
        }
    }
}
