using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BounceProjectile : Projectile
{
    private IEnemy lastEnemy;   // 마지막 히트 적

    private int currentBounce;  // 남은 바운스 수

    // 바운스 수 설정
    public void SetBounceCount(int bounces)
    {
        currentBounce = bounces;
    }

    // 적 히트
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
