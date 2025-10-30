using System;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using JHJ;
using TeamProject.GameSystem;

public class TankEnemy : BaseEnemy
{
    //TankEnemy

    private void Awake()
    {
        EnemySpawnerObject = GameObject.Find("EnemySpawner");
    }
    private void Update()
    {
        MoveToPlayer();
    }

    public override void Init()
    {
        maxHP = 2 * EnemySpawnerObject.GetComponent<EnemySpawner>().MaxHP;
        currentHP = maxHP;
        moveSpeed = EnemySpawnerObject.GetComponent<EnemySpawner>().MoveSpeed;
        damage = EnemySpawnerObject.GetComponent<EnemySpawner>().Damage;
        reward = EnemySpawnerObject.GetComponent<EnemySpawner>().Reward;
        plus = EnemySpawnerObject.GetComponent<EnemySpawner>().Plus;
    }

}
