// ================================================================
// [GameManagerPrototype.cs]
// 목적 : Player, Enemy, UI, Spawner를 연결해주는 테스트용 허브 스크립트.
//       - 실제 게임 로직은 없음. (연결 확인 전용)
// 사용법 : 
//    1) 씬에 이 스크립트를 넣고 Player, Spawner, UIManager를 연결.
//    2) 숫자키(1~5), W, G, O, K 로 테스트 가능.
//    3) 모든 파트 이벤트가 정상 연결되면 실제 GameManager.cs로 이전.
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
    // ── 싱글톤 ───────────────────────────────────────────────────────────────
    public static GameManagerPrototype Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _enemies = new List<IEnemy>(64);
        State = GameState.Ready;
    }

    // ── Player 이벤트 연결 ───────────────────────────────────────────────────
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

    // ── Spawner 이벤트 연결 ──────────────────────────────────────────────────
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

    // ── 상태/데이터 ──────────────────────────────────────────────────────────
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

    // ── Unity Loop ───────────────────────────────────────────────────────────
    private void Update()
    {
        if (State == GameState.Playing)
            TimeElapsed += Time.deltaTime;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        HandleTestHotkeys();   // 개발/에디터에서만 동작하는 테스트 키
#endif
    }

    // ── 외부 공개 API ───────────────────────────────────────────────────────
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

            // 풀을 안 쓰면 Destroy(e.GO); 로만 처리해도 됨.
            if (!ObjectPool.TryReturn(e.GO))
                Destroy(e.GO);

            _enemies.RemoveAt(i);
        }
        Debug.Log("[GM] ClearEnemies → count=0");
    }

    // ── 신호 수신부 ──────────────────────────────────────────────────────────
    private void OnPlayerDamaged(float currentHP)
    {
        UIManager.Instance?.UpdateHP(currentHP);
        Debug.Log($"[GM] PlayerDamaged hp={currentHP}");
    }

    private void OnPlayerDied()
    {
        Debug.Log("[GM] PlayerDied → GameOver");
        IsGameOver = true;
    }

    private void OnEnemySpawned(IEnemy enemy)
    {
        if (enemy == null) return;
        RegisterEnemy(enemy);
        Debug.Log($"[GM] EnemySpawned → count={Enemies.Count}");
    }

    private void OnEnemyKilled(IEnemy enemy, int earnScore)
    {
        Score += earnScore;
        if (enemy != null) _enemies.Remove(enemy);
        Debug.Log($"[GM] EnemyKilled +{earnScore} score={Score} count={Enemies.Count}");
    }

    // ── 내부 유틸 ────────────────────────────────────────────────────────────
    private void RegisterEnemy(IEnemy enemy)
    {
        if (enemy == null) return;
        enemy.Killed -= OnEnemyKilled; // 중복 방지
        enemy.Killed += OnEnemyKilled;
        _enemies.Add(enemy);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // ── 테스트 키 ────────────────────────────────────────────────────────────
    // 1: StartGame, 2: Pause, 3: Resume, 4: Spawn Basic, 5: Clear
    // W: Wave+1 & Begin,   G: 강제 GameOver,  O: Basic 5마리 스폰
    // K: 첫 적 강제 사망(해당 IEnemy.Die() 구현 필요)
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
            Debug.Log($"[GM] Wave++ → {WaveLevel}");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            // 플레이어 이벤트를 거치지 않고 바로 게임오버 상태 진입(테스트용)
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
                // IEnemy가 Die() 구현되어 있고 내부에서 Killed 이벤트를 발생시킨다는 전제
                Enemies[0].Die();
                Debug.Log("[GM] Force kill first enemy");
            }
        }
    }
#endif
}