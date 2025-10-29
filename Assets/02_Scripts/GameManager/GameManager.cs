using System;
using System.Collections;
using System.Collections.Generic;
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

        // 1. ���� ���� 
        public void GameStart()
        {
            currentWave = 1;
            waveRemain = 30f;
        }

        // 2. ���� ���̺� ����
        public void Update()
        {
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
            enemySpawner.SetWaveLevel(currentWave);

            InterestPayment();

            if (enemySpawner != null)
            {
                enemySpawner.SetWaveLevel(currentWave);
            }
        }

        public void InterestPayment()
        {
            // 1. �÷��̾ �����ؼ� ���� ���� ���ִ��� �����´�.
            int currentMoney = playerGold;

            // 2. ������ ���� ���ڸ� ���ؼ� �󸶸� ������� ����Ѵ�.
            int interest = Mathf.FloorToInt(currentMoney * nowInterest);

            // 3. ���� ������ ���� �÷��̾��� ���� ���ڸ� ���Ѵ�.
            playerGold += interest;

            Debug.Log($"[���� ����] Wave {currentWave}: +{interest}G �� �� {playerGold}G");
        }

        public void UpgradeInterest()
        {
            nowInterest += 0.1f;
        }

    }
}