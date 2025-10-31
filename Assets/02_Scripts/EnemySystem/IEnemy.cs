using JHJ;
using System.Collections;
using System.Collections.Generic;
using TeamProject.GameSystem;
using UnityEngine;

public interface IEnemy 
{
    // 초기화
    void Init();

    // 플레이어로 이동
    void MoveToPlayer();

    // 적 체력이 0 이하일 때, 오브젝트 파괴됨
    void EnemyDie(bool reward);

    // 적 체력 감소
    void TakeDamage(int damage);
    
    // 적 체력이 0 이하일 때, 오브젝트 파괴됨
    void SetWaveLevel(int level);

    
    public Collider Collider { get; }
    public bool IsDead { get; }
    public Transform Transform { get; }
    public float Reward { get; }
}