using System;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using JHJ;
using TeamProject.GameSystem;

public class SpeedEnemy : MonoBehaviour, IEnemy
{
    //SpeedEnemy

    [SerializeField] private GameObject EnemySpawnerObject;

    private float maxHP; // 최대
    private float currentHP; // 현재
    private float moveSpeed; // 이동 속도
    private float damage; // 공격력
    private float plus; // 증가량

    private float reward; // 보상

    public bool IsDead { get; set; }
    public float Reward { get { return reward; } }

    public Transform Transform { get { return transform; } }

    private void Awake()
    {
        EnemySpawnerObject = GameObject.Find("EnemySpawner");
    }

    private void Start()
    {
        Init();
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

    // 초기화
    public void Init()
    {
        maxHP = EnemySpawnerObject.GetComponent<EnemySpawner>().MaxHP;
        currentHP = maxHP;
        //스피드형 = 두배 빠르게
        moveSpeed = 2 * EnemySpawnerObject.GetComponent<EnemySpawner>().MoveSpeed;
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

    // 플레이어 공격, 플레이어에게 접촉 시 데미지를 입히고 파괴
    //public void AttackToPlayer()
    //{
    //    OnTriggerEnter(GameObject.Find("Player").GetComponent<Collider>());
    //}

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"{gameObject.name} 접촉함.");
            other.GetComponentInParent<Player>().TakeDamage(damage);

            EnemyDie();
        }

        else if (other.tag == "Projectile")
        {
            GameManager.Instance.EnemyKill(this);
            Debug.Log($"{gameObject.name} 공격당함.");
        }
    }

    // 적 체력 감소
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
    }

    // 적 체력이 0 이하일 때, 오브젝트 파괴됨
    public void EnemyDie()
    {
        gameObject.SetActive(false);
        IsDead = true;
        Debug.Log($"{gameObject.name} 비활성화");
    }

    // internal : 같은 assembly 에서만 public으로 접근 가능
    // 웨이브 증가 시, 강화됨
    internal void SetWaveLevel(int level)
    {
        // 레벨의 0.5배 증가
        maxHP = maxHP + plus * level;
        damage = damage + plus * level;
        moveSpeed = moveSpeed + plus * level;
    }
}
