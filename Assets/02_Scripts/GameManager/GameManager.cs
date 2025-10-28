// ===============================
// GameManager.cs (팀 공용 스크립트)
// 작성자: 임상호 (게임 매니저)
// 역할: 게임의 전체 흐름(Ready → Playing → Pause → GameOver)을 관리
// 사용 대상: 전 팀원 (UI, Player, Enemy, Spawner 등)
// ===============================

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamProject.GameSystem
{
    [DefaultExecutionOrder(-100)] // 다른 스크립트보다 먼저 실행되게
    public class GameManager : MonoBehaviour
    {
        // --------------- [1] 싱글톤 기본 구조 ---------------
        // 모든 스크립트에서 GameManager.Instance 로 접근 가능하게 만듦
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

            Boot(); // 초기 실행 (게임 시작 시 한 번만)
        }

        // --------------- [2] 게임 상태 정의 ---------------
        // 팀원들은 이걸 참고해서 “현재 상태가 뭐냐?” 판단하면 됨
        public enum GameState { Boot, Ready, Playing, Paused, GameOver }
        public GameState State { get; private set; } = GameState.Boot;

        // 팀원용 이벤트
        // 구독(+=) 해서 UI, 사운드, 적 스포너 등이 반응하도록 구성
        public event Action<GameState> OnStateChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnLifeChanged;

        // --------------- [3] 인스펙터에서 조정할 값 ---------------
        [Header("게임 기본 설정")]
        [Tooltip("게임 시작 전 카운트다운 시간(초)")]
        [SerializeField] private int startCountdown = 3;
        [Tooltip("시작 체력(또는 목숨 수)")]
        [SerializeField] private int startLife = 3;
        [Tooltip("메인 씬 이름 - 비워두면 현재 씬 그대로 사용")]
        [SerializeField] private string mainGameSceneName = "";

        // --------------- [4] 런타임 값 (실제 게임 중 바뀌는 데이터) ---------------
        public int Score { get; private set; }
        public int Life { get; private set; }

        private Coroutine countdownRoutine;

        // --------------- [5] 초기화 및 상태 전환 로직 ---------------
        private void Boot()
        {
            // 씬 로드 필요 시 (예: 타이틀 → Game 씬)
            if (!string.IsNullOrEmpty(mainGameSceneName) &&
                SceneManager.GetActiveScene().name != mainGameSceneName)
            {
                SceneManager.sceneLoaded += OnSceneLoadedThenReady;
                SceneManager.LoadScene(mainGameSceneName);
                return;
            }

            ToReady(); // 같은 씬이라면 바로 Ready 상태로 진입
        }

        private void OnSceneLoadedThenReady(Scene _, LoadSceneMode __)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedThenReady;
            ToReady();
        }

        private void ToReady()
        {
            State = GameState.Ready;
            Score = 0;
            Life = startLife;
            OnScoreChanged?.Invoke(Score);
            OnLifeChanged?.Invoke(Life);
            OnStateChanged?.Invoke(State);

            // UI : “READY” 텍스트 띄우고 카운트다운 표시 가능
            if (startCountdown > 0)
            {
                if (countdownRoutine != null) StopCoroutine(countdownRoutine);
                countdownRoutine = StartCoroutine(StartCountdown(startCountdown));
            }
            else
            {
                StartGame();
            }
        }

        private IEnumerator StartCountdown(int seconds)
        {
            // UI : 여기서 남은 시간 표시 가능
            for (int t = seconds; t > 0; t--)
            {
                Debug.Log($"게임 시작까지 {t}초");
                yield return new WaitForSeconds(1f);
            }
            StartGame();
        }

        // --------------- [6] 상태 전환용 함수들 ---------------
        public void StartGame()
        {
            if (State == GameState.Playing) return;
            State = GameState.Playing;
            OnStateChanged?.Invoke(State);
            Time.timeScale = 1f;

            // Player : 이제 움직임/공격 시작 가능
            // Enemy/Spawner : 적 생성 루틴 시작
        }

        public void PauseGame()
        {
            if (State != GameState.Playing) return;
            State = GameState.Paused;
            OnStateChanged?.Invoke(State);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (State != GameState.Paused) return;
            State = GameState.Playing;
            OnStateChanged?.Invoke(State);
            Time.timeScale = 1f;
        }

        public void GameOver()
        {
            if (State == GameState.GameOver) return;
            State = GameState.GameOver;
            OnStateChanged?.Invoke(State);
            Time.timeScale = 1f;

            // UI : “GAME OVER” 패널 띄우기
            // Player : 조작 멈추기
            // Enemy : 적 스폰 멈추기
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // 씬이 리로드되면 Boot() → Ready로 다시 진입
        }

        // --------------- [7] 팀원들이 직접 부를 수 있는 공개 함수 ---------------
        public void AddScore(int amount)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }

        public void DamagePlayer(int damage = 1)
        {
            if (State != GameState.Playing) return;
            Life = Mathf.Max(0, Life - damage);
            OnLifeChanged?.Invoke(Life);
            if (Life <= 0) GameOver();
        }

        public bool IsPlaying() => State == GameState.Playing;

#if UNITY_EDITOR
        // 테스트용 단축키 (빌드 시 제거됨)
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) StartGame();
            if (Input.GetKeyDown(KeyCode.F2)) GameOver();
            if (Input.GetKeyDown(KeyCode.F5)) Restart();
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (State == GameState.Playing) PauseGame();
                else if (State == GameState.Paused) ResumeGame();
            }
        }
#endif
    }
}