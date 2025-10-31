using System;
using System.Collections;
using System.Collections.Generic;
using JHJ;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // DOTween 사용을 위해 추가

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

            // 씬 로드 이벤트 구독
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // 씬 로드 이벤트 구독 해지
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
        private IEnemy _currentBoss;

        void Start()
        {
            // 씬이 처음 로드될 때도 카메라 효과를 적용하기 위해 호출
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            GameStart();
        }
        
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
                        UI.UIManager.Instance.ShowEventText("Boss Encounter", 2f);
                        enemySpawner.SpawnBoss();
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
                enemySpawner.StartSpawn(currentWave);            //혜주님 작업 완료 되면 확인.
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
            UI.UIManager.Instance.ShowEventText("Wave Clear!", 2f);

            if (_currentBoss != null)
            {
                _currentBoss = null;
            }
            isBossSpawned = false;
            NextWave();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // "02_InGame" 씬에서만 카메라 줌 아웃 효과 실행
            if (scene.name == "02_InGame")
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    // FOV를 즉시 0으로 설정 (이전 씬에서 넘어온 값)
                    mainCamera.fieldOfView = 0;
                    
                    // FOV를 85로 1.5초에 걸쳐 애니메이션
                    mainCamera.DOFieldOfView(85f, 1.5f).SetEase(Ease.OutSine);
                }
            }
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
