using UnityEngine;

public class MultiWeapon : Weapon
{
    [Header("각도")]
    [SerializeField] private float totalAngle = 45f;

    public override void Fire()
    {
        Vector3 baseDirection = (target.position - transform.position).normalized;

        // 총 각도 왼쪽부터
        float startAngle = -totalAngle / 2f;

        // 투사체 간 간격
        float angleStep = 0f;
        
        // 간격 : 투사체 개수 - 1 (간격의 수)
        angleStep = totalAngle / (projectileCount - 1);
        
        for (int i = 0; i < projectileCount; i++)
        {
            // 현재 투사체의 최종 각도 계산
            float finalAngle = startAngle + (angleStep * i);

            Quaternion rotation = Quaternion.AngleAxis(finalAngle, Vector3.up);
            Vector3 direction = rotation * baseDirection;

            Projectile projectile = GetFromPool();
            projectile.transform.position = transform.position;
            projectile.transform.rotation = Quaternion.LookRotation(direction);
            projectile.SetDirection(direction);
        }
    }
}
