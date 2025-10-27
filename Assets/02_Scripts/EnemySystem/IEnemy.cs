using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    private enum Type { normal, tank, speed, boss } // 적 타입	

    // 초기화
    void Init();

    // 플레이어로 이동
    void MoveToPlayer();

    // 공격주기 갱신
    void AttackToPlayer();

    // 적 체력이 0 이하일 때, 오브젝트 파괴됨
    void Die();
}