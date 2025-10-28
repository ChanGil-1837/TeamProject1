using UnityEngine;

public sealed class MultiWeapon : Weapon
{
    [Header("�߻� ��")]
    [SerializeField] private int shotCount;
    [Header("����")]
    [SerializeField] private float totalAngle = 45f;

    public override void Fire()
    {
        Vector3 baseDirection = (target.position - transform.position).normalized;

        // �� ���� ���ʺ���
        float startAngle = -totalAngle / 2f;

        // ����ü �� ����
        float angleStep = 0f;
        
        // ���� : ����ü ���� - 1 (������ ��)
        angleStep = totalAngle / (shotCount - 1);
        
        for (int i = 0; i < shotCount; i++)
        {
            // ���� ����ü�� ���� ���� ���
            float finalAngle = startAngle + (angleStep * i);

            Quaternion rotation = Quaternion.AngleAxis(finalAngle, Vector3.up);
            Vector3 direction = rotation * baseDirection;

            Projectile projectile = GetFromPool();

            SetProjectileTransform(projectile, direction);
        }
    }


    public void UpgradeShotCount(int shotIncrease)
    {
        shotCount += shotIncrease;
    }
}
