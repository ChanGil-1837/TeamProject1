using JHJ;
using System.Collections;
using System.Collections.Generic;
using TeamProject.GameSystem;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IEnemy
{
    protected GameObject EnemySpawnerObject;

    protected float maxHP; // 최대
    protected float currentHP; // 현재
    protected float moveSpeed; // 이동 속도
    protected float damage; // 공격력
    protected float plus; // 증가량
    protected float reward; // 보상
    protected bool isDead;

    public bool IsDead { get { return isDead; } set { isDead = value; } }
    public float CurrentHP { get { return currentHP; }}
    public float Reward { get { return reward; } }
    public Transform Transform { get { return transform; } }
    public Collider Collider { get { return GetComponent<Collider>(); } }

    private void Awake()
    {
        EnemySpawnerObject = GameObject.Find("EnemySpawner");
    }
    private void Update()
    {
        MoveToPlayer();
        // 현재 체력이 0이면 사망처리
        if (currentHP <= 0)
        {
            EnemyDie();
        }
    }

    private void OnEnable()
    {
        Collider.enabled = false;
        Collider.enabled = true;
    }

    // 초기화
    public virtual void Init()
    {
        maxHP = EnemySpawnerObject.GetComponent<EnemySpawner>().MaxHP;
        currentHP = maxHP;
        moveSpeed = EnemySpawnerObject.GetComponent<EnemySpawner>().MoveSpeed;
        damage = EnemySpawnerObject.GetComponent<EnemySpawner>().Damage;
        reward = EnemySpawnerObject.GetComponent<EnemySpawner>().Reward;
        plus = EnemySpawnerObject.GetComponent<EnemySpawner>().Plus;
    }


    // 플레이어로 이동
    public void MoveToPlayer()
    {
        Transform playerTrans = GameObject.Find("Player").transform;

        // 중앙으로 설정 속도 만큼 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            GameObject.Find("Player").transform.position,
            moveSpeed * Time.deltaTime
            );

        Vector3 dir = (playerTrans.position - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"HP : {currentHP}/{maxHP}");
            Debug.Log($"{gameObject.name} 접촉함.");
            other.GetComponentInParent<Player>().TakeDamage(damage);
            EnemyDie();
        }
        if (other.tag == "Projectile")
        {
            Debug.Log($"HP : {currentHP}/{maxHP}");
        }
    }

    public void EnemyDie()
    {
        gameObject.SetActive(false);
        IsDead = true;
        
        Debug.Log($"{gameObject.name} 비활성화");
    }

    // 적 체력 감소
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
    }

    public void SetWaveLevel(int level)
    {
        // 레벨의 0.5배 증가
        maxHP = maxHP + plus * level;
        damage = damage + plus * level;
        moveSpeed = moveSpeed + plus * level;
    }
}
