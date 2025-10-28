using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NormalEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private float _maxHP; // 최대 체력
    [SerializeField] private float _moveSpeed; // 이동 속도
    [SerializeField] private int _damage; // 공격력

    private float currentHP; // 현재 체력
    // private Reward reward; // 보상???

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
        currentHP = _maxHP;
    }

    // 플레이어로 이동
    public void MoveToPlayer()
    {
        // 중앙으로 설정 속도 만큼 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            GameObject.Find("Player").transform.position,
            _moveSpeed * Time.deltaTime
            );
    }

    // 플레이어 공격, 플레이어에게 접촉 시 데미지를 입히고 파괴
    public void AttackToPlayer()
    {
        OnTriggerEnter(GameObject.Find("Player").GetComponent<Collider>());
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"{gameObject.name}접촉!");
            Die();
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
