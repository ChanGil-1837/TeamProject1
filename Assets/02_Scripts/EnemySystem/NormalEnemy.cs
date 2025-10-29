using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NormalEnemy : MonoBehaviour, IEnemy
{
    [Header("기본 최대 체력")]
    [SerializeField] private float maxHP = 10; // 최대

    [Header("기본 현재 체력")]
    [SerializeField] private float currentHP; // 현재

    [Header("기본 이동속도")]
    [SerializeField] private float moveSpeed = 1; // 이동 속도

    [Header("기본 공격력")]
    [SerializeField] private float damage = 10; // 공격력

    [Header("체력 증가량(test용))")]
    [SerializeField] private float _plusHP; // 최대 체력 

    [Header("이동속도 증가량(test용)")]
    [SerializeField] private float _plusMoveSpeed; // 이동속도

    [Header("공격력 증가량(test용)")]
    [SerializeField] private float _plusDamage; // 공격력

    private float reward; // 보상

    public bool IsDead { get; set; }
    public Transform Transform { get { return transform; } }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        MoveToPlayer();
    }

    // 초기화
    public void Init()
    {
        // 웨이브에 따라 증가되는 수치만큼 반영
        maxHP = maxHP + _plusHP;
        moveSpeed = moveSpeed + _plusMoveSpeed;
        damage = damage + _plusDamage;

        currentHP = maxHP;
    }

    // 플레이어로 이동
    public void MoveToPlayer()
    {
        // 중앙으로 설정 속도 만큼 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            GameObject.Find("Player").transform.position,
            moveSpeed * Time.deltaTime
            );
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
            Debug.Log($"{gameObject.name} 접촉!");
            //GameObject.Find("Player").GetComponent<Player>().Gold += _reward;
            EnemyDie();
        }

        else if (other.tag == "Projectile")
        {
            Debug.Log($"{gameObject.name} 접촉!");

            // 현재 체력이 0이면 사망처리
            if (currentHP <= 0)
            {
                EnemyDie();
            }
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
        Debug.Log("Enemy 비활성화");
    }
}
