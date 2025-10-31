using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;
    private Collider colli;
    protected Weapon weapon;

    [Header("��")]
    [SerializeField] private GameObject model;

    [Header("�浹 ��ƼŬ")]
    [SerializeField] private List<ParticleSystem> particles;

    [Header("��ƼŬ ��� ��")]
    [SerializeField] private int emmisionCount = 10;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        colli = GetComponent<Collider>();
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
        colli.enabled = true;
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

    // ��Ȱ��ȭ
    protected void DisableProjectile()
    {
        rigid.velocity = Vector3.zero;
        colli.enabled = false;
        model.SetActive(false);

        // ��ƼŬ
        EmissionParticle();

        // ��ȯ (��ƼŬ ��� �Ϸ� �ð�)
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
