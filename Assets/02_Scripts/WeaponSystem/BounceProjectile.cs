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
            enemy.TakeDamage(weapon.Damage);

            lastEnemy = enemy;

            currentBounce--;

            if (currentBounce > 0)
            {
                RandomBounce();
            }
            else
            {
                DisableProjectile();
            }
        }
    }

    private void RandomBounce()
    {
        // �� ���� ���� ���� ����
        Vector2 randomCircle = Random.insideUnitCircle.normalized;

        // Vector3�� ��ȯ , Y ����
        Vector3 randomDirection = new Vector3(randomCircle.x, 0f, randomCircle.y).normalized;

        // ȸ��
        transform.rotation = Quaternion.LookRotation(randomDirection);

        // ���ο� ���� ����
        SetVelocity(randomDirection);
    }
}
