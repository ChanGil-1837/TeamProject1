// ===============================
// GameManagerPrototype.cs (���ο� �׽�Ʈ��)
// ����: ����/���/����/���ӿ��� + �� ���/���� + ���� �帧 ����
// ������: ����(Spawner/UI/Player���� �޼��塤�̺�Ʈ�� ����/ȣ��)
// ===============================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TeamProject
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameManagerPrototype : MonoBehaviour
    {
        // �������������������������� �̱��� ��������������������������
        public static GameManagerPrototype Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ResetRuntime();
        }

        // �������������������������� �ν����� �� ��������������������������
        [Header("�ʱⰪ")]
        [SerializeField] private int startLife = 3;

        [Header("�̺�Ʈ ��(�ڵ� ������ ���� �ν����ͷ� ����)")]
        public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
        public UnityEvent<int> OnGoldChanged = new UnityEvent<int>();
        public UnityEvent<int> OnLifeChanged = new UnityEvent<int>();
        public UnityEvent<bool> OnGameOver = new UnityEvent<bool>();

        // �������������������������� ��Ÿ�� ���� ��������������������������
        private readonly List<GameObject> _enemies = new List<GameObject>();
        public IReadOnlyList<GameObject> Enemies => _enemies;

        public int Score { get; private set; }
        public int Gold { get; private set; }
        public int Life { get; private set; }
        public bool IsGameOver { get; private set; }

        private void ResetRuntime()
        {
            Score = 0;
            Gold = 0;
            Life = Mathf.Max(0, startLife);
            IsGameOver = false;

            OnScoreChanged?.Invoke(Score);
            OnGoldChanged?.Invoke(Gold);
            OnLifeChanged?.Invoke(Life);
            OnGameOver?.Invoke(IsGameOver);

            Time.timeScale = 1f;
        }

        // �������������������������� ���� API (������ ȣ��) ��������������������������

        // [Spawner]���� �� ���� ���� ȣ��
        public void RegisterEnemy(GameObject enemy)
        {
            if (enemy == null) return;
            if (_enemies.Contains(enemy)) return;
            _enemies.Add(enemy);
            // �ʿ� ��: Debug.Log($"[GM] Enemy registered: {enemy.name}");
        }

        // [Enemy]�� �װų� ����� �� ȣ��(�ڱ� �ڽ� ����)
        public void UnregisterEnemy(GameObject enemy)
        {
            if (enemy == null) return;
            _enemies.Remove(enemy);
        }

        // ���� óġ���� �� ����/��� ���� �� ���� ó��
        public void EnemyKilled(GameObject enemy, int score = 1, int gold = 0)
        {
            if (enemy != null) _enemies.Remove(enemy);
            AddScore(score);
            AddGold(gold);
        }

        // [Enemy]�� �÷��̾ ����� ��: ���� �ְ� ���� ����(��û �ݿ�)
        public void EnemyHitPlayer(GameObject enemy, int damage = 1)
        {
            DamagePlayer(damage);
            if (enemy != null)
            {
                _enemies.Remove(enemy);
                Destroy(enemy);
            }
        }

        public void AddScore(int amount)
        {
            if (amount == 0) return;
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }

        // [UI ���� ��� ����]
        // - ��û��: UI ��� (����ȣ)
        // - ����: ���� ȹ��/���׷��̵� �� ���̺� ���� �� ���� ���� ���� ��ɰ� ���� ����
        // - ó�� ��ġ: AddGold(), OnGoldChanged �̺�Ʈ
        // - ����: GameManagerPrototype�� ��� ������ �̺�Ʈ ��������� ����ϰ�,
        //         ���� ���� ���� ��� �� ǥ�� ������ UIManager�� ���� ��ũ��Ʈ����
        //         OnGoldChanged.AddListener(...) ���·� ���� ����.

        public void AddGold(int amount)
        {
            if (amount == 0) return;
            Gold += amount;
            OnGoldChanged?.Invoke(Gold);
        }

        public void DamagePlayer(int damage = 1)
        {
            if (IsGameOver) return;
            Life = Mathf.Max(0, Life - Mathf.Max(0, damage));
            OnLifeChanged?.Invoke(Life);

            if (Life <= 0) EndGame();
        }

        public void HealPlayer(int amount = 1)
        {
            if (IsGameOver) return;
            Life = Mathf.Max(0, Life + Mathf.Max(0, amount));
            OnLifeChanged?.Invoke(Life);
        }

        public void EndGame()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            Time.timeScale = 0f;            // �ʿ� �� ����
            OnGameOver?.Invoke(true);
        }

        public void RestartPrototype()
        {
            // ������Ÿ��: �� ���ε� ��� ��Ÿ�� ���¸� �ʱ�ȭ
            foreach (var e in _enemies)
            {
                if (e != null) Destroy(e);
            }
            _enemies.Clear();
            ResetRuntime();
        }

#if UNITY_EDITOR
        // ������ ����Ű(�׽�Ʈ ����)
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) AddScore(1);
            if (Input.GetKeyDown(KeyCode.F2)) AddGold(1);
            if (Input.GetKeyDown(KeyCode.F3)) DamagePlayer(1);
            if (Input.GetKeyDown(KeyCode.F5)) RestartPrototype();
        }
#endif
    }
}