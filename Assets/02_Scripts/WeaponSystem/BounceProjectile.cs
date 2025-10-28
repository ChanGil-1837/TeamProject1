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
            enemy.TakeDamage(weapon.Damage);

            lastEnemy = enemy;

            currentBounce--;

            if (currentBounce > 0)
            {
                ApplyRandomBounce();
            }
            else
            {
                DisableProjectile();
            }
        }
    }

    private void ApplyRandomBounce()
    {
        // 원 내부 랜덤 벡터 생성
        Vector2 randomCircle = Random.insideUnitCircle.normalized;

        // Vector3로 변환 , Y 고정
        Vector3 randomDirection = new Vector3(randomCircle.x, 0f, randomCircle.y).normalized;

        // 회전
        transform.rotation = Quaternion.LookRotation(randomDirection);

        // 새로운 방향 설정
        SetDirection(randomDirection);
    }
}
