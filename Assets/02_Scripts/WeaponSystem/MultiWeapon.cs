using UnityEngine;

public sealed class MultiWeapon : Weapon
{
    [Header("투사체 수")]
    [SerializeField] private int shotCount = 3;
    [Header("각도")]
    [SerializeField] private float totalAngle = 45f;

    public int ShotLevel => shotLevel;
    private int shotLevel;

    public override void Fire(IEnemy enemy)
    {
        // 가까운 적 방향
        Vector3 baseDirection = (enemy.Transform.position - transform.position).normalized;

        // 1발 이하 안전장치
        if(shotCount <= 1)
        {
            FireProjectile(baseDirection);
            return;
        }

        // 총 각도 왼쪽부터
        float startAngle = -totalAngle / 2f;

        // 투사체 간 간격
        float angleStep = 0f;
        
        // 간격 : 투사체 개수 - 1 (간격의 수)
        angleStep = totalAngle / (shotCount - 1);
        
        for (int i = 0; i < shotCount; i++)
        {
            // 현재 투사체의 최종 각도 계산
            float finalAngle = startAngle + (angleStep * i);

            Quaternion rotation = Quaternion.AngleAxis(finalAngle, Vector3.up);
            Vector3 direction = rotation * baseDirection;

            FireProjectile(direction);
        }
    }

    // 투사체 추가 업그레이드
    public void UpgradeShotCount(int shotIncrease)
    {
        shotCount += shotIncrease;
        shotLevel++;
    }
}
