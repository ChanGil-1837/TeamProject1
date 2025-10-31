using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamProject.GameSystem;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 프리팹 리스트")]
    [SerializeField] private List<GameObject> _enemyPrefabs;// 프리팹

    [Header("적 생성(개수)")]
    [SerializeField] private int _normalEnemyNumber;
    [SerializeField] private int _tankEnemyNumber;
    [SerializeField] private int _speedEnemyNumber;
    [SerializeField] private int _bossEnemyNumber;

    [Header("적 스폰 거리(플레이어 기준)")]
    [SerializeField] private float _attackRange;

    [Header("적 생성 주기")]
    [SerializeField] private float _spawnerInterval; // 적 생성주기
                                                            //
                                                            // [Header("기본 최대 체력")]
    [Header("적 기본정보")]
    [SerializeField] private float _maxHP; // 최대
    [SerializeField] private float _moveSpeed = 0.5f; // 이동 속도
    [SerializeField] private float _damage; // 공격력
    [SerializeField] private float _reward; // 보상

    [Header("증가량(test용))")]
    [SerializeField] private float _plus = 0.5f; // 증가량	


    #region 프로퍼티
    public float MaxHP { get { return _maxHP; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float Damage { get { return _damage; } }
    public float Plus { get { return _plus; } }
    public float Reward { get { return _reward; } }
    #endregion

    private List<GameObject> activeEnemies = new List<GameObject>(); // 생성한 적 리스트
    private List<GameObject> suffleActiveEnemies = new List<GameObject>(); // 적 셔플 리스트

    private List<GameObject> normalEnemies = new List<GameObject>(); // Normal 적
    private List<GameObject> tankEnemies = new List<GameObject>(); // Normal 적
    private List<GameObject> speedEnemies = new List<GameObject>(); // Normal 적
    private List<GameObject> bossEnemies = new List<GameObject>(); // Normal 적

    #region 변수
    //적 생성 횟수
    private int enemyTurn = 0;
    //리스트 번호
    private int ListN = 0;
    //코루틴 저장 변수
    private Coroutine activeCoroutine;
    //웨이브-레벨 변수
    private int level=0;

    #endregion

    //public struct EnemyData
    //{
    //    public GameObject gameOJ;
    //    public IEnemy enemyInterface;

    //    public EnemyData(GameObject GO, IEnemy IF)
    //    {
    //        gameOJ = GO;
    //        enemyInterface = IF;
    //    }
    //}


    private void Awake()
    {
        GameObject EnemyBox = new GameObject("EnemyBox");

        Debug.Log($"입력 이속{MoveSpeed}");
    }

    // private void Start()
    // {
    //     SetWaveEnemy(1);
    //     StartSpawn(1);
    // }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 웨이브가 종료되면 모두 비활 성화 + 코루틴 종료
        {
            StopCoroutine(activeCoroutine);
            Debug.Log($"웨이브 종료. enemy 생성 중지됨");
        }
        //ClearEnemies();
    }

    public void KillAll(BaseEnemy ignoreEnemy = null)
    {
        // Stop spawning new enemies
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            Debug.Log("적 생성이 중지되었습니다.");
        }

        // Combine all enemy lists
        List<GameObject> allEnemies = new List<GameObject>();
        allEnemies.AddRange(normalEnemies);
        allEnemies.AddRange(tankEnemies);
        allEnemies.AddRange(speedEnemies);
        allEnemies.AddRange(bossEnemies);

        // Kill all active enemies
        foreach (var enemy in allEnemies)
        {
            if (enemy != null && enemy.activeSelf)
            {
                BaseEnemy enemyComponent = enemy.GetComponent<BaseEnemy>();
                if (enemyComponent == ignoreEnemy)
                {
                    continue;
                }
                enemyComponent.EnemyDie();
            }
        }
        Debug.Log("모든 활성화된 적을 제거했습니다.");
    }

    // 웨이브 enemy 설정
    public void SetWaveEnemy(int wave)
    {
        CreateEnemy(normalEnemies, _normalEnemyNumber * wave, "Normal", 0);
        CreateEnemy(tankEnemies, _tankEnemyNumber * wave, "Tank", 1);
        CreateEnemy(speedEnemies, _speedEnemyNumber * wave, "Speed", 2);
        CreateEnemy(bossEnemies, 1, "Boss", 3);
        ShuffleList();
        
        if (suffleActiveEnemies.Count < 25)
        {
            _normalEnemyNumber++;
        }

        if (wave % 2 == 0 )
        {
            _tankEnemyNumber++;
            _normalEnemyNumber--;
        }
        if (wave % 3 == 0)
        {
            _speedEnemyNumber++;
            _normalEnemyNumber--;
        }
    }

    // 보스 몹 처리
    public void BossEliminated(int wave)
    {
        // 보스 상태가 죽은 상태면
        if (GetComponent<BossEnemy>().CurrentHP <= 0 )
        {
            // 적 스텟 강화
            SetWaveLevel(wave);

        }
    }


    // 적 소환
    public void StartSpawn(int wave)
    {
        activeCoroutine = StartCoroutine(SpawnEnemy(wave));
    }

    public void SpawnBoss()
    {
        if (bossEnemies.Count > 0)
        {
            GameObject boss = bossEnemies[0];
            if (boss != null && !boss.activeSelf)
            {
                // Find a valid spawn position
                Vector3 spawnPosition;
                do
                {
                    spawnPosition = GameObject.Find("Player").transform.position + (Random.insideUnitSphere * _attackRange);
                    spawnPosition.y = 0;
                } while (spawnPosition.magnitude < _attackRange - 1);

                boss.GetComponent<BaseEnemy>().Init();
                boss.GetComponent<BaseEnemy>().IsDead = false;
                boss.transform.position = spawnPosition;
                boss.SetActive(true);
            }
        }
    }

        IEnumerator SpawnEnemy(int wave)
        {
            float delayTime = _spawnerInterval;
            //대기

            while (true)
            {
                yield return new WaitForSecondsRealtime(_spawnerInterval);
                {
                    // 적 활성화 코드
                    if (enemyTurn < suffleActiveEnemies.Count)
                    {
                        // 비활성화된 상태에서만 위치 부여 + 활성화
                        while (suffleActiveEnemies[enemyTurn].activeSelf == false )
                        {
                            // 웨이브 진행중이고 보스몹이면 소환 안되게
         // (WaveRemain 프로퍼티 추가되면 활성화)--------------------------------------------------
                            //if(suffleActiveEnemies[enemyTurn].name == "Boss-4000" && TeamProject.GameSystem.GameManager.Instance.WaveRemain <=0)
                            //{
                            //    enemyTurn++;
                            //    break;
                            //}

                            // 반지름 길이 = 사정거리 조건 맞을 때까지 랜덤
                            Vector3 direction = GameObject.Find("Player").transform.position + (Random.insideUnitSphere * _attackRange);
                            direction.y = 0;

                            // 랜덤 위치가 유효한 거리인지
                            if (direction.magnitude >= _attackRange - 1)
                            {
                                suffleActiveEnemies[enemyTurn].GetComponent<BaseEnemy>().Init();
                                suffleActiveEnemies[enemyTurn].GetComponent<BaseEnemy>().SetWaveLevel(wave);
                                suffleActiveEnemies[enemyTurn].GetComponent<BaseEnemy>().IsDead = false;

                                suffleActiveEnemies[enemyTurn].transform.position = direction;
                                suffleActiveEnemies[enemyTurn].SetActive(true);

                                enemyTurn++;
                                Debug.Log($"{enemyTurn}번 적 활성화");
                                break;
                            }
                        }
                    }
                    // 리스트의 끝 번호까지 가면 반복
                    if (suffleActiveEnemies.Count == enemyTurn)
                    {
                        enemyTurn = 0;
                    }

                }
            }

        }
    
    

    // 웨이브 레벨에 따른 적 능력치 처리
    public void SetWaveLevel(int wave)
    {
        SetWaveEnemy(wave);
    }

    // 오브젝트 이름 중복을 피하기 위해 부여
    private int EnemyNameID(List<GameObject> enemyTypeint)
    {
        int IDN = 0;
        if (enemyTypeint == normalEnemies)
        {
            IDN = 1000 + enemyTypeint.Count;
        }
        else if (enemyTypeint == tankEnemies)
        {
            IDN = 2000 + enemyTypeint.Count;
        }
        else if (enemyTypeint == speedEnemies)
        {
            IDN = 3000 + enemyTypeint.Count;
        }
        else if (enemyTypeint == bossEnemies)
        {
            IDN = 4000 + enemyTypeint.Count;
        }

        return IDN;
    }

    //적 미리 생성
    private void CreateEnemy(List<GameObject> enemyTypeint, int enemyNumber, string enemyTypeName, int prefabN)
    {
        int nameID = EnemyNameID(enemyTypeint);

        // 희망생성 수보다 리스트가 적으면 새로 만들어서 추가
        if (enemyTypeint.Count < enemyNumber)
        {
            for (int i = 0; i < enemyNumber; i++)
            {
                GameObject enemyType = Instantiate(_enemyPrefabs[prefabN], GameObject.Find("EnemyBox").transform);
                //IEnemy enemy = enemyType.GetComponent<IEnemy>();

                enemyType.name = $"{enemyTypeName}-{nameID}";
                enemyType.tag = "Enemy";

                enemyTypeint.Add(enemyType);

                enemyType.SetActive(false);
                Debug.Log($"{enemyTypeint[i].name}");
                nameID++;
            }
        }
        // 희망생성 수보다 리스트가 많으면 요소 삭제(뒤에서부터)
        else if (enemyTypeint.Count > enemyNumber)
        {
            // 삭제할 개수
            int deleteEnemy = enemyTypeint.Count - enemyNumber;

            for (int i = 0; i < deleteEnemy; i++)
            {
                // 제거할 요소
                string name = enemyTypeint[enemyTypeint.Count - 1].name;

                // 리스트 맨 뒤 요소 삭제
                enemyTypeint.Remove(enemyTypeint[enemyTypeint.Count - 1]);
                // 오브젝트 제거
                GameObject.Destroy(GameObject.Find("EnemyBox").transform.Find($"{name}").gameObject);

                Debug.Log($"{name} 삭제");
            }

        }
    }

    //리스트 순서 셔플
    private void ShuffleList()
    {
        // 적 리스트 병함
        activeEnemies.Clear();
        suffleActiveEnemies.Clear();
        activeEnemies.AddRange(normalEnemies);
        activeEnemies.AddRange(tankEnemies);
        activeEnemies.AddRange(speedEnemies);
        //activeEnemies.AddRange(bossEnemies); // 보스는 SpawnBoss로 따로 소환

        System.Random random = new System.Random();
        // 리스트 전체 길이 저장
        int n = activeEnemies.Count;

        Debug.Log($"적 개수 : {activeEnemies.Count}");
        List<GameObject> temp = new List<GameObject>();
        temp = activeEnemies;

        // 생성한 적 리스트를 섞어서 새리스트에 저장.
        for (int i = 0; i < n; i++)
        {
            // 랜덤 번호 생성
            int randomN = random.Next(0, activeEnemies.Count);
            // 셔플리스트에 랜덤 적 추가
            suffleActiveEnemies.Add(activeEnemies[randomN]);
            temp.Add(activeEnemies[randomN]);
            // 셔플리스트에 추가한 항목 제거
            activeEnemies.Remove(activeEnemies[randomN]);

            Debug.Log($"셔플리스트 : {i} : {suffleActiveEnemies[i].name}");
        }

        activeEnemies = temp;

        Debug.Log($"셔플 리스트 총 개수 : {suffleActiveEnemies.Count}");

    }
}
