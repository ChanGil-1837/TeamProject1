using UnityEngine;

public sealed class MultiWeapon : Weapon
{
    [SerializeField] private int shotCount = 3;
    [SerializeField] private float totalAngle = 45f;

    public int ShotLevel => shotLevel;
    private int shotLevel;

    public override void Fire(IEnemy enemy)
    {
        if (!isAvailable) return;
        if (CheckCondition(enemy) == false) return;

        Vector3 baseDirection = GetDirection(enemy);

        // 1�� ���� ������ġ
        if (shotCount <= 1)
        {
            FireProjectile(baseDirection);
            return;
        }

        // ���� ���ʺ���
        float startAngle = -totalAngle / 2f;

        // ����ü �� ����
        float angleStep = 0f;
        
        // ���� ��� 
        angleStep = totalAngle / (shotCount - 1);
        
        for (int i = 0; i < shotCount; i++)
        {
            // ���� ���� ���
            float finalAngle = startAngle + (angleStep * i);

            Quaternion rotation = Quaternion.AngleAxis(finalAngle, Vector3.up);

            Vector3 direction = rotation * baseDirection;

            FireProjectile(direction);
        }

        AfterFire();
    }

    // ����ü �߰� ���׷��̵�
    public void UpgradeShotCount(int shotIncrease)
    {
        shotCount += shotIncrease;
        shotLevel++;
    }
}
