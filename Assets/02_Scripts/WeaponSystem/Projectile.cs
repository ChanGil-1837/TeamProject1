using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;
    protected Weapon weapon;

    [Header("��")]
    [SerializeField] private GameObject model;

    [Header("�浹 ��ƼŬ")]
    [SerializeField] private List<ParticleSystem> particles;

    [Header("��ƼŬ ��� ��")]
    [SerializeField] private int emmisionCount = 10;
    public Weapon GetWeapon()
    {
        return weapon;
    }
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


    //Ȱ��ȭ ��
    private void OnEnable()
    {
        model.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IEnemy enemy;

            other.TryGetComponent<IEnemy>(out enemy);

            EmissionParticle();

            EnemyHit(enemy);
        }
        else if (other.CompareTag("Wall"))
        {
            DisableProjectile();
        }
    }

    // �� �浹
    protected virtual void EnemyHit(IEnemy enemy)
    {
        if(enemy != null)
        {
            enemy.TakeDamage(weapon.Damage);
        }

        DisableProjectile();
    }

    // ���� ����
    public void SetVelocity(Vector3 dir)
    {
        if (dir.magnitude > 0)
        {
            rigid.velocity = dir * weapon.Speed;
        }
    }

    // �� ��Ȱ��ȭ, ��ƼŬ, ��ȯ �ɾ�α�
    protected void DisableProjectile()
    {
        model.SetActive(false);
        EmissionParticle();
        Invoke("AfterEffect", weapon.LifeTime);
    }

    // ��ƼŬ ���
    private void EmissionParticle()
    {
        foreach(var p in particles)
        {
            p.Emit(emmisionCount);
        }
    }


    // ����Ʈ ���� �� ��ȯ
    private void AfterEffect()
    {
        weapon.ReturnToPool(this);
    }
}
