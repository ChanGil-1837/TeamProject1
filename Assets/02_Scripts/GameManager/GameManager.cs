// ===============================
// GameManager.cs (�� ���� ��ũ��Ʈ)
// �ۼ���: �ӻ�ȣ (���� �Ŵ���)
// ����: ������ ��ü �帧(Ready �� Playing �� Pause �� GameOver)�� ����
// ��� ���: �� ���� (UI, Player, Enemy, Spawner ��)
// ===============================

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamProject.GameSystem
{
    [DefaultExecutionOrder(-100)] // �ٸ� ��ũ��Ʈ���� ���� ����ǰ�
    public class GameManager : MonoBehaviour
    {
        // --------------- [1] �̱��� �⺻ ���� ---------------
        // ��� ��ũ��Ʈ���� GameManager.Instance �� ���� �����ϰ� ����
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

            Boot(); // �ʱ� ���� (���� ���� �� �� ����)
        }

        // --------------- [2] ���� ���� ���� ---------------
        // �������� �̰� �����ؼ� ������ ���°� ����?�� �Ǵ��ϸ� ��
        public enum GameState { Boot, Ready, Playing, Paused, GameOver }
        public GameState State { get; private set; } = GameState.Boot;

        // ������ �̺�Ʈ
        // ����(+=) �ؼ� UI, ����, �� ������ ���� �����ϵ��� ����
        public event Action<GameState> OnStateChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnLifeChanged;

        // --------------- [3] �ν����Ϳ��� ������ �� ---------------
        [Header("���� �⺻ ����")]
        [Tooltip("���� ���� �� ī��Ʈ�ٿ� �ð�(��)")]
        [SerializeField] private int startCountdown = 3;
        [Tooltip("���� ü��(�Ǵ� ��� ��)")]
        [SerializeField] private int startLife = 3;
        [Tooltip("���� �� �̸� - ����θ� ���� �� �״�� ���")]
        [SerializeField] private string mainGameSceneName = "";

        // --------------- [4] ��Ÿ�� �� (���� ���� �� �ٲ�� ������) ---------------
        public int Score { get; private set; }
        public int Life { get; private set; }

        private Coroutine countdownRoutine;

        // --------------- [5] �ʱ�ȭ �� ���� ��ȯ ���� ---------------
        private void Boot()
        {
            // �� �ε� �ʿ� �� (��: Ÿ��Ʋ �� Game ��)
            if (!string.IsNullOrEmpty(mainGameSceneName) &&
                SceneManager.GetActiveScene().name != mainGameSceneName)
            {
                SceneManager.sceneLoaded += OnSceneLoadedThenReady;
                SceneManager.LoadScene(mainGameSceneName);
                return;
            }

            ToReady(); // ���� ���̶�� �ٷ� Ready ���·� ����
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

            // UI : ��READY�� �ؽ�Ʈ ���� ī��Ʈ�ٿ� ǥ�� ����
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
            // UI : ���⼭ ���� �ð� ǥ�� ����
            for (int t = seconds; t > 0; t--)
            {
                Debug.Log($"���� ���۱��� {t}��");
                yield return new WaitForSeconds(1f);
            }
            StartGame();
        }

        // --------------- [6] ���� ��ȯ�� �Լ��� ---------------
        public void StartGame()
        {
            if (State == GameState.Playing) return;
            State = GameState.Playing;
            OnStateChanged?.Invoke(State);
            Time.timeScale = 1f;

            // Player : ���� ������/���� ���� ����
            // Enemy/Spawner : �� ���� ��ƾ ����
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

            // UI : ��GAME OVER�� �г� ����
            // Player : ���� ���߱�
            // Enemy : �� ���� ���߱�
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // ���� ���ε�Ǹ� Boot() �� Ready�� �ٽ� ����
        }

        // --------------- [7] �������� ���� �θ� �� �ִ� ���� �Լ� ---------------
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
        // �׽�Ʈ�� ����Ű (���� �� ���ŵ�)
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