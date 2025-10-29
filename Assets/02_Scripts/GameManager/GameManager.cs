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

        // 1. ���� ���� 
        public void GameStart()
        {
            currentWave = 1;
            waveRemain = 30f;
        }

        // 2. ���� ���̺� ����
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

        // 3. ���̺� ���࿡ ���� ó��
        public void NextWave()
        {
            currentWave++;
            // �����ϴٸ� �ڷ�ƾ���� ������ �����Ǵ� ���� UI�� �ݿ��ؼ� �ð��� �������� ���� ǥ��.
            waveRemain = 30f;

            if (enemySpawner != null)
            {
                enemySpawner.SetWaveLevel(currentWave);
            }

            InterestPayment();
        }

        // 4. ���� ����
        public void InterestPayment()
        {
            if (player == null)
                return;

            // 1. �÷��̾ �����ؼ� ���� ���� �� �ִ��� �����´�.
            int currentMoney = player.Gold;

            // 2. ������ ���� ���ڸ� ���ؼ� �󸶸� ��� ���� ����Ѵ�.
            int interest = (int)(currentMoney * nowInterest);

            // 3. ���� ������ ���� �÷��̾��� ���� ���ڸ� ���Ѵ�.
            player.AddGold(interest);
        }

        // 5. ������ ���׷��̵�
        public void UpgradeInterest()
        {
            nowInterest += 0.1f;
        }

        // 6. �� óġ �� ������ ����
        public void EnemyKill(IEnemy enemy)
        {
            if (enemy == null || player == null)
                return;

            player.AddGold(enemy.Reward);
        }

        public void DamagePlayer(IEnemy enemy)
        {
            // �ٲ� ����������� �� �ʿ��� �޼���
        }

        public bool IsGameOver { get; private set; }

        public void GameOver()
        {
            IsGameOver = true;
        }
    }
}