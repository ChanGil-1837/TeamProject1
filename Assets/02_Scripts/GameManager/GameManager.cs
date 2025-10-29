using System;
using System.Collections;
using System.Collections.Generic;
using JHJ;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamProject.GameSystem
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameStart();
        }

        public EnemySpawner enemySpawner;

        public int currentWave = 1;
        public float waveDuration = 30f;
        public float waveRemain = 30f;

        public float nowInterest = 0.0f;
        public int playerGold = 0;

        public Player player;

        // 1. 게임 시작 
        public void GameStart()
        {
            currentWave = 1;
            waveRemain = 30f;
        }

        // 2. 게임 웨이브 진행
        public void Update()
        {
            if (IsGameOver)
                return;

            TickTime();

            if (waveRemain < 0.0f)
            {
                NextWave();
            }
        }

        public void TickTime()
        {
            waveRemain -= Time.deltaTime;
        }

        // 3. 웨이브 진행에 따른 처리
        public void NextWave()
        {
            currentWave++;
            // 가능하다면 코루틴으로 서서히 증가되는 것을 UI에 반영해서 시간이 차오르는 것을 표현.
            waveRemain = 30f;

            if (enemySpawner != null)
            {
                enemySpawner.SetWaveLevel(currentWave);
            }

            InterestPayment();
        }

        // 4. 이자 지급
        public void InterestPayment()
        {
            if (player == null)
                return;

            // 1. 플레이어를 참조해서 현재 돈이 얼마 있는지 가져온다.
            int currentMoney = player.Gold;

            // 2. 가져온 돈에 이자를 곱해서 얼마를 줘야 할지 계산한다.
            int interest = (int)(currentMoney * nowInterest);

            // 3. 이자 지급을 위해 플레이어의 돈에 이자를 더한다.
            player.AddGold(interest);
        }

        // 5. 이자율 업그레이드
        public void UpgradeInterest()
        {
            nowInterest += 0.1f;
        }

        // 6. 적 처치 시 리워드 지급
        public void EnemyKill(IEnemy enemy)
        {
            if (enemy == null || player == null)
                return;

            player.AddGold(enemy.Reward);
        }

        public void DamagePlayer(IEnemy enemy)
        {
            // 바뀐 사양으로인해 불 필요한 메서드
        }

        public bool IsGameOver { get; private set; }

        public void GameOver()
        {
            IsGameOver = true;
        }
    }
}