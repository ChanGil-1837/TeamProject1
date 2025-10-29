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
        // public List<IEnemy> enemyPool;

        //1. ���� ���� 

        //2. ���� ���̺� ����

        //3. ���̺� ���࿡ ���� ó��
        public void GameStart()
        {
            currentWave = 1;
            waveRemain = 30f;
        }

        public void Update()
        {
            TickTime();

            if(waveRemain < 0.0f)
            {
                NextWave();
            }
        }

        public void TickTime()
        {
            waveRemain -= Time.deltaTime;
        }

        public void NextWave()
        {
            currentWave++;
            //�����ϴٸ� �ڷ�ƾ���� ������ �����Ǵ� ���� UI�� �ݿ��ؼ� �ð��� �������� ���� ǥ��.
            waveRemain = 30f;
            enemySpawner.SetWaveLevel(currentWave);


        }


        public void UpgradeInterest()
        {
            nowInterest += 0.1f;
        }
     
    }
}