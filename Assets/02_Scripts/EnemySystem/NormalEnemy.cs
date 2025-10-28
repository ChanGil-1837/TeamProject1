using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NormalEnemy : MonoBehaviour, IEnemy
{
    private float maxHP = 10; // 최대
    private float currentHP; // 현재
    private float moveSpeed = 1; // 이동 속도
    private float damage = 10; // 공격력

    [SerializeField] private float _plusHP; // 최대 체력 
    [SerializeField] private float _plusMoveSpeed; // 이동 속도
    [SerializeField] private float _plusDamage; // 공격력

    private float reward; // 보상

    public bool IsDead { get; set; }
    public Transform Transform { get; set; }

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
        moveSpeed = moveSpeed+ _plusMoveSpeed;
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
            Die();
        }

        else if (other.tag == "Projectile")
        {
            Debug.Log($"{gameObject.name} 접촉!");
            // 공격력만큼 차감
            currentHP -= 0;

            // 현재 체력이 0이면 사망처리
            if (currentHP <= 0)
            {
                Die();
                //플레이어 보상지급메서드 호출
                //GameObject.Find("Player").GetComponent<Player>().Reward();
            }
        }
    }

    // 적 체력이 0 이하일 때, 오브젝트 파괴됨
    public void Die()
    {
        gameObject.SetActive(false);
        IsDead = true;
        Debug.Log("Enemy 비활성화");
    }


    // 생성 적 위치
    public void RendomPos()
    {

    }
}
