using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;

    private Weapon weapon;

    public void Init(Weapon weapon)
    {
        // 투사체의 무기
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌 시 
    }


    private void Move()
    {
        // 이동
    }

    private void DisableProjectile()
    {
        // 비활성화
    }
}
