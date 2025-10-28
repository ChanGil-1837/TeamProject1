// ================================================================
// [GameManagerPrototype.cs]
// ���� : Player, Enemy, UI, Spawner�� �������ִ� �׽�Ʈ�� ��� ��ũ��Ʈ.
//       - ���� ���� ������ ����. (���� Ȯ�� ����)
// ���� : 
//    1) ���� �� ��ũ��Ʈ�� �ְ� Player, Spawner, UIManager�� ����.
//    2) ����Ű(1~5), W, G, O, K �� �׽�Ʈ ����.
//    3) ��� ��Ʈ �̺�Ʈ�� ���� ����Ǹ� ���� GameManager.cs�� ����.
// ================================================================
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Pool;

public enum GameState { Ready, Playing, Paused, GameOver }
public enum EnemyType { Basic, Fast, Tank }

public class GameManagerPrototype : MonoBehaviour
{
    // ���� �̱��� ������������������������������������������������������������������������������������������������������������������������������
    public static GameManagerPrototype Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _enemies = new List<IEnemy>(64);
        State = GameState.Ready;
    }

    // ���� Player �̺�Ʈ ���� ������������������������������������������������������������������������������������������������������
    [SerializeField] private Player _player;
    public Player Player
    {
        get => _player;
        set
        {
            if (_player != null) { _player.Damaged -= OnPlayerDamaged; _player.Died -= OnPlayerDied; }
            _player = value;
            if (_player != null) { _player.Damaged += OnPlayerDamaged; _player.Died += OnPlayerDied; }
        }
    }

    // ���� Spawner �̺�Ʈ ���� ����������������������������������������������������������������������������������������������������
    [SerializeField] private EnemySpawner _enemySpawner;
    public EnemySpawner EnemySpawner
    {
        get => _enemySpawner;
        set
        {
            if (_enemySpawner != null) _enemySpawner.Spawned -= OnEnemySpawned;
            _enemySpawner = value;
            if (_enemySpawner != null) _enemySpawner.Spawned += OnEnemySpawned;
        }
    }

    // ���� ����/������ ��������������������������������������������������������������������������������������������������������������������
    private List<IEnemy> _enemies;
    public IReadOnlyList<IEnemy> Enemies => _enemies;

    [SerializeField] private int _score;
    public int Score
    {
        get => _score;
        private set { _score = Mathf.Max(0, value); UIManager.Instance?.UpdateScore(_score); }
    }

    [SerializeField] private bool _isGameOver;
    public bool IsGameOver
    {
        get => _isGameOver;
        private set
        {
            _isGameOver = value;
            if (_isGameOver)
            {
                State = GameState.GameOver;
                EnemySpawner?.Stop();
                UIManager.Instance?.ShowGameOver();
            }
        }
    }

    [SerializeField] private int _waveLevel;
    public int WaveLevel
    {
        get => _waveLevel;
        private set { _waveLevel = Mathf.Max(0, value); }
    }

    [SerializeField] private GameState _state = GameState.Ready;
    public GameState State
    {
        get => _state;
        private set { _state = value; UIManager.Instance?.UpdateState(_state); }
    }

    private float _timeElapsed;
    public float TimeElapsed
    {
        get => _timeElapsed;
        private set { _timeElapsed = Mathf.Max(0, value); UIManager.Instance?.UpdateTimer(_timeElapsed); }
    }

    // ���� Unity Loop ����������������������������������������������������������������������������������������������������������������������
    private void Update()
    {
        if (State == GameState.Playing)
            TimeElapsed += Time.deltaTime;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        HandleTestHotkeys();   // ����/�����Ϳ����� �����ϴ� �׽�Ʈ Ű
#endif
    }

    // ���� �ܺ� ���� API ��������������������������������������������������������������������������������������������������������������
    public void StartGame()
    {
        Score = 0;
        TimeElapsed = 0f;
        WaveLevel = 1;
        IsGameOver = false;
        State = GameState.Playing;
        EnemySpawner?.Begin(WaveLevel);
        Debug.Log($"[GM] StartGame  wave={WaveLevel}");
    }

    public void RequestPause(bool pause)
    {
        if (State == GameState.GameOver) return;
        State = pause ? GameState.Paused : GameState.Playing;
        if (pause) EnemySpawner?.Stop(); else EnemySpawner?.Begin(WaveLevel);
        Debug.Log($"[GM] Pause={pause}  state={State}");
    }

    public void SpawnEnemy(EnemyType type)
    {
        if (EnemySpawner == null || State != GameState.Playing) return;
        var enemy = EnemySpawner.Spawn(type);
        if (enemy == null) return;
        RegisterEnemy(enemy);
        Debug.Log($"[GM] SpawnEnemy type={type} count={Enemies.Count}");
    }

    public void ClearEnemies()
    {
        for (int i = _enemies.Count - 1; i >= 0; --i)
        {
            var e = _enemies[i];
            if (e == null) { _enemies.RemoveAt(i); continue; }

            // Ǯ�� �� ���� Destroy(e.GO); �θ� ó���ص� ��.
            if (!ObjectPool.TryReturn(e.GO))
                Destroy(e.GO);

            _enemies.RemoveAt(i);
        }
        Debug.Log("[GM] ClearEnemies �� count=0");
    }

    // ���� ��ȣ ���ź� ��������������������������������������������������������������������������������������������������������������������
    private void OnPlayerDamaged(float currentHP)
    {
        // UIManager.Instance?.UpdateHP(currentHP);
        Debug.Log($"[GM] PlayerDamaged hp={currentHP}");
    }

    private void OnPlayerDied()
    {
        Debug.Log("[GM] PlayerDied �� GameOver");
        IsGameOver = true;
    }

    private void OnEnemySpawned(IEnemy enemy)
    {
        if (enemy == null) return;
        RegisterEnemy(enemy);
        Debug.Log($"[GM] EnemySpawned �� count={Enemies.Count}");
    }

    private void OnEnemyKilled(IEnemy enemy, int earnScore)
    {
        Score += earnScore;
        if (enemy != null) _enemies.Remove(enemy);
        Debug.Log($"[GM] EnemyKilled +{earnScore} score={Score} count={Enemies.Count}");
    }

    // ���� ���� ��ƿ ������������������������������������������������������������������������������������������������������������������������
    private void RegisterEnemy(IEnemy enemy)
    {
        if (enemy == null) return;
        enemy.Killed -= OnEnemyKilled; // �ߺ� ����
        enemy.Killed += OnEnemyKilled;
        _enemies.Add(enemy);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // ���� �׽�Ʈ Ű ������������������������������������������������������������������������������������������������������������������������
    // 1: StartGame, 2: Pause, 3: Resume, 4: Spawn Basic, 5: Clear
    // W: Wave+1 & Begin,   G: ���� GameOver,  O: Basic 5���� ����
    // K: ù �� ���� ���(�ش� IEnemy.Die() ���� �ʿ�)
    private void HandleTestHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartGame();
        if (Input.GetKeyDown(KeyCode.Alpha2)) RequestPause(true);
        if (Input.GetKeyDown(KeyCode.Alpha3)) RequestPause(false);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SpawnEnemy(EnemyType.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ClearEnemies();

        if (Input.GetKeyDown(KeyCode.W))
        {
            WaveLevel++;
            if (State == GameState.Playing) EnemySpawner?.Begin(WaveLevel);
            Debug.Log($"[GM] Wave++ �� {WaveLevel}");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            // �÷��̾� �̺�Ʈ�� ��ġ�� �ʰ� �ٷ� ���ӿ��� ���� ����(�׽�Ʈ��)
            IsGameOver = true;
            Debug.Log("[GM] Force GameOver");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < 5; i++) SpawnEnemy(EnemyType.Basic);
            Debug.Log("[GM] Spawn x5");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Enemies.Count > 0)
            {
                // IEnemy�� Die() �����Ǿ� �ְ� ���ο��� Killed �̺�Ʈ�� �߻���Ų�ٴ� ����
                Enemies[0].Die();
                Debug.Log("[GM] Force kill first enemy");
            }
        }
    }
#endif
}