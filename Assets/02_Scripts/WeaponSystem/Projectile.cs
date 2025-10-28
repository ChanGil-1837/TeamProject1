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

    // 생성 시 초기화
    public void Init(Weapon weapon)
    {
        // 투사체의 무기
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
        else if (other.CompareTag("Wall"))
        {
            DisableProjectile();
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

    // 방향, 속도 설정
    public void SetDirection(Vector3 dir)
    {
        if (dir.magnitude > 0)
        {
            rigid.velocity = dir * speed;
        }
    }

    // 반환
    protected void DisableProjectile()
    {
        weapon.ReturnToPool(this);
    }
}
