using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;
    private Weapon weapon;

    [SerializeField] private float speed;   //속도

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
            //if(other.TryGetComponent<Enemy>(out Enemy enemy)) { }
            DisableProjectile();
        }
    }


    // 방향, 속도 설정
    public void SetDirection(Vector3 dir)
    {
        if (dir.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            rigid.velocity = dir * speed;
        }
    }

    // 반환
    private void DisableProjectile()
    {
        weapon.ReturnToPool(this);
    }
}
