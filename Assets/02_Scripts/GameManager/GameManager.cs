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

        public event Action OnWaveChanged;

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

        public int currentWave = 0;
        public float waveDuration = 30f;
        public float waveRemain = 30f;

        public float nowInterest = 0.0f;
        public int playerGold = 0;
        private bool isWaving;
        private bool isBossSpawned = false;
        public Player player;

        // [추가] 보스 관련 필드
        //[Header("Boss Settings")]
        //public GameObject[] bossPrefabs;       // 웨이브별 보스 프리팹(없으면 마지막걸로 고정)
        //public Transform bossSpawnPoint;       // 보스 스폰 위치(없으면 (0,0,0))
        //private GameObject _currentBoss;       // 현 보스 인스턴스
        //private bool _bossPhase;               // 보스 페이즈 진행 중?
        //public bool IsBossPhase => _bossPhase; // UI가 읽어 쓸 수 있게

        // 1. 게임 시작 
        public void GameStart()
        {
            isWaving = false;
            NextWave();
        }

        // 2. 게임 웨이브 진행
        public void Update()
        {

            if (!IsPlaying) return; // Pause/GameOver면 아래 로직 전부 차단

            if (IsGameOver)
                return;
            if (isWaving)
            {
                TickTime();
                if(waveRemain < 0.0f)
                {
                    if(!isBossSpawned)
                    {
                        //enemySpawner.SpawnBoss();
                        isBossSpawned = true;
                        isWaving = false;
                    }
                }
            }
        

        }

        public void TickTime()
        {
            waveRemain -= Time.deltaTime;
            OnWaveChanged?.Invoke();
        }

        // 3. 웨이브 진행에 따른 처리
        public void NextWave() // 스타트에서 이걸로 호출되게
        {
            currentWave++;
            waveRemain = 30f;
            isWaving = true;
            if (enemySpawner != null)
            {
                enemySpawner.SetWaveLevel(currentWave);
                //enemySpawner.StartSpawn();            //혜주님 작업 완료 되면 확인.
            }
            OnWaveChanged?.Invoke();
            InterestPayment();
        }

        // 4. 이자 지급
        public void InterestPayment()
        {
            if (player == null)
                return;

            int currentMoney = player.Gold;
            int interest = (int)(currentMoney * nowInterest);
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

            int reward = (int)enemy.Reward;  // 팀 기준이 int라면 단순 캐스팅으로 충분
            player.AddGold(reward);
        }

        public void BossEliminated()
        {
            
        }

        public bool IsGameOver { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsPlaying => !IsGameOver && !IsPaused;

        public void GameOver()
        {
            IsGameOver = true;
        }

    }
}