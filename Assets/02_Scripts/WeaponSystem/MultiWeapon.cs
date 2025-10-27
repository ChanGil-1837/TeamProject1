using UnityEngine;

public class MultiWeapon : Weapon
{
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
        angleStep = totalAngle / (projectileCount - 1);
        
        for (int i = 0; i < projectileCount; i++)
        {
            // ���� ����ü�� ���� ���� ���
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
