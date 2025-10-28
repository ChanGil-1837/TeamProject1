using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;
    protected Weapon weapon;

    [SerializeField] private float speed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // ���� �� �ʱ�ȭ
    public void Init(Weapon weapon)
    {
        // ����ü�� ����
        this.weapon = weapon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IEnemy enemy;

            other.TryGetComponent<IEnemy>(out enemy);

            EnemyHit(enemy);
        }
    }

    protected virtual void EnemyHit(IEnemy enemy)
    {
        if(enemy != null)
        {
            //enemy.TakeDamage(weapon.Damage);
        }

        DisableProjectile();
    }

    // ����, �ӵ� ����
    public void SetDirection(Vector3 dir)
    {
        if (dir.magnitude > 0)
        {
            rigid.velocity = dir * speed;
        }
    }

    // ��ȯ
    private void DisableProjectile()
    {
        weapon.ReturnToPool(this);
    }
}
