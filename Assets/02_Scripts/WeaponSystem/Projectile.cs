using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;
    private Weapon weapon;

    [SerializeField] private float speed;   //�ӵ�

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
            //if(other.TryGetComponent<Enemy>(out Enemy enemy)) { }
            DisableProjectile();
        }
    }


    // ����, �ӵ� ����
    public void SetDirection(Vector3 dir)
    {
        if (dir.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            rigid.velocity = dir * speed;
        }
    }

    // ��ȯ
    private void DisableProjectile()
    {
        weapon.ReturnToPool(this);
    }
}
