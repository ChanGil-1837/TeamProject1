using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { normal, tank, speed, boss } // 적 타입	

public class IEnemy : MonoBehaviour
{
    // 초기화
    public virtual void Init()
    {
        
    }

    // 플레이어로 이동
    public virtual void MoveToPlayer()
    {
        
    }

    // 공격주기 갱신
    //void AttackToPlayer();

    // 적 체력이 0 이하일 때, 오브젝트 파괴됨
    public virtual void EnemyDie()
    {
        
    }

    // 피격 시
    public virtual void TakeDamage(int damage)
    {

    }
    
    public virtual void OnTriggerEnter(Collider other)
    {
        
    }

    // 웨이브 증가 시, 강화됨
    //void SetWaveLevel(int level);

    // 적 상태
    public bool IsDead
    {
        get;
    }

    // 리워드
    public float Reward
    {
        get;
    }

}