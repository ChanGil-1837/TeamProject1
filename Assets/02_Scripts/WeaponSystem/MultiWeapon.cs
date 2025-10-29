using UnityEngine;

public sealed class MultiWeapon : Weapon
{
    [Header("����ü ��")]
    [SerializeField] private int shotCount = 3;
    [Header("����")]
    [SerializeField] private float totalAngle = 45f;

    public int ShotLevel => shotLevel;
    private int shotLevel;

    public override void Fire(IEnemy enemy)
    {
        // ����� �� ����
        Vector3 baseDirection = (enemy.Transform.position - transform.position).normalized;

        // 1�� ���� ������ġ
        if(shotCount <= 1)
        {
            FireProjectile(baseDirection);
            return;
        }

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

            FireProjectile(direction);
        }
    }

    // ����ü �߰� ���׷��̵�
    public void UpgradeShotCount(int shotIncrease)
    {
        shotCount += shotIncrease;
        shotLevel++;
    }
}
