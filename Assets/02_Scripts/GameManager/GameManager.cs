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

        //1. 게임 시작 

        //2. 게임 웨이브 진행

        //3. 웨이브 진행에 따른 처리
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
            //가능하다면 코루틴으로 서서히 증가되는 것을 UI에 반영해서 시간이 차오르는 것을 표현.
            waveRemain = 30f;
            enemySpawner.SetWaveLevel(currentWave);


        }


        public void UpgradeInterest()
        {
            nowInterest += 0.1f;
        }
     
    }
}