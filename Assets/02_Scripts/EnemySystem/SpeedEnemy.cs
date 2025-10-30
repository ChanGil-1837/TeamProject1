using System;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using JHJ;
using TeamProject.GameSystem;

public class SpeedEnemy : BaseEnemy
{
    //SpeedEnemy

    public override void Init()
    {
        maxHP = EnemySpawnerObject.GetComponent<EnemySpawner>().MaxHP;
        currentHP = maxHP;
        moveSpeed = 2 * EnemySpawnerObject.GetComponent<EnemySpawner>().MoveSpeed;
        damage = EnemySpawnerObject.GetComponent<EnemySpawner>().Damage;
        reward = EnemySpawnerObject.GetComponent<EnemySpawner>().Reward;
        plus = EnemySpawnerObject.GetComponent<EnemySpawner>().Plus;
    }
}
