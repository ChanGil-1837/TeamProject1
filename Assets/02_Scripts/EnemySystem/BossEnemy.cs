using System;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using JHJ;
using TeamProject.GameSystem;

public class BossEnemy : BaseEnemy
{
    //BossEnemy
    public event Action OnBossDied;

    public override void Init()
    {
        maxHP = 2 * EnemySpawnerObject.GetComponent<EnemySpawner>().MaxHP;
        currentHP = maxHP;
        moveSpeed = 2 * EnemySpawnerObject.GetComponent<EnemySpawner>().MoveSpeed;
        damage = 2 * EnemySpawnerObject.GetComponent<EnemySpawner>().Damage;
        reward = 2 * EnemySpawnerObject.GetComponent<EnemySpawner>().Reward;
        plus = EnemySpawnerObject.GetComponent<EnemySpawner>().Plus;
    }

    public override void EnemyDie(bool giveReward = true)
    {
        OnBossDied?.Invoke();

        if (EnemySpawnerObject != null)
        {
            // 자기 자신을 제외하고 모든 적을 죽임
            EnemySpawnerObject.GetComponent<EnemySpawner>().KillAll(this);
        }
        GameManager.Instance.BossEliminated();
        base.EnemyDie(giveReward);
        
    }
}
