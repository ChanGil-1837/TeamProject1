using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;
    private Collider colli;
    protected Weapon weapon;

    [Header("모델")]
    [SerializeField] private GameObject model;

    [Header("충돌 파티클")]
    [SerializeField] private List<ParticleSystem> particles;

    [Header("파티클 방사 수")]
    [SerializeField] private int emmisionCount = 10;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        colli = GetComponent<Collider>();
    }

    // 생성 시 초기화
    public void Init(Weapon weapon)
    {
        // 투사체의 무기
        this.weapon = weapon;
    }


    //활성화 시
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

    // 적 충돌
    protected virtual void EnemyHit(IEnemy enemy)
    {
        if(enemy != null)
        {
            enemy.TakeDamage(weapon.Damage);
        }

        DisableProjectile();
    }

    // 방향 설정
    public void SetVelocity(Vector3 dir)
    {
        if (dir.magnitude > 0)
        {
            rigid.velocity = dir * weapon.Speed;
        }
    }

    // 비활성화
    protected void DisableProjectile()
    {
        rigid.velocity = Vector3.zero;
        colli.enabled = false;
        model.SetActive(false);

        // 파티클
        EmissionParticle();

        // 반환 (파티클 재생 완료 시간)
        Invoke("AfterEffect", weapon.LifeTime);
    }

    // 파티클 방사
    private void EmissionParticle()
    {
        foreach(var p in particles)
        {
            p.Emit(emmisionCount);
        }
    }


    // 이펙트 끝난 후 반환
    private void AfterEffect()
    {
        weapon.ReturnToPool(this);
    }
}
