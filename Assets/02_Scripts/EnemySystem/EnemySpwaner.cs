using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemyPrefabs;// 프리팹
    [SerializeField] private int _normalEnemyNumber;
    [SerializeField] private int _tankEnemyNumber;
    [SerializeField] private int _speedEnemyNumber;
    [SerializeField] private int _bossEnemyNumber;
    [SerializeField] private float _attackRange = 10f; //GameObject.Find("Player").GetComponent<Player>().AttackRange;
    [SerializeField] private float _spawnerInterval = 1.0f; // 적 생성주기	



    private List<Transform> spawnPoints; // 스폰 포인트	
    private List<GameObject> activeEnemies = new List<GameObject>(); // 생성한 적 리스트
    private List<GameObject> suffleActiveEnemies = new List<GameObject>(); // 적 셔플 리스트
                                                                           //private List<EnemyData> activeEnemies = new List<EnemyData>();
                                                                           //private List<EnemyData> suffleActiveEnemies = new List<EnemyData>();


    private List<GameObject> normalEnemies = new List<GameObject>(); // Normal 적
    private List<GameObject> tankEnemies = new List<GameObject>(); // Normal 적
    private List<GameObject> speedEnemies = new List<GameObject>(); // Normal 적
    private List<GameObject> bossEnemies = new List<GameObject>(); // Normal 적

    //적 생성 횟수
    int enemyTurn = 0;
    //리스트 번호
    private int ListN = 0;

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
        new GameObject();

        CreateEnemy(normalEnemies, _normalEnemyNumber, "Normal", 0);
        CreateEnemy(normalEnemies, 5, "Normal", 0);
        //CreateEnemy(tankEnemies, _tankEnemyNumber, "Tank", 1);
        //CreateEnemy(speedEnemies, _speedEnemyNumber, "Speed", 2);
        //CreateEnemy(bossEnemies, _bossEnemyNumber, "Boss", 3);

        ShuffleList();
    }

    // 타이머 갱신
    private void Update()
    {
        SpawnEnemy();
        ClearEnemies();
    }

    // 적 소환
    private void SpawnEnemy()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (enemyTurn < activeEnemies.Count)
            {
                while (true)
                {
                    // 반지름 길이 = 사정거리 에서 몹 스폰
                    Vector3 direction = GameObject.Find("Player").transform.position + (Random.insideUnitSphere * _attackRange);
                    direction.y = 0;

                    if (direction.magnitude >= _attackRange - 1)
                    {
                        activeEnemies[enemyTurn].transform.position = direction;
                        activeEnemies[enemyTurn].SetActive(true);
                        enemyTurn++;
                        Debug.Log($"{enemyTurn}번 적 생성");
                        break;
                    }
                }
            }
            else if (activeEnemies.Count ==enemyTurn )
            {
                enemyTurn = 0;
            }
            else
            {
                Debug.Log($"전부 소환함");
            }
        }
    }

    // 웨이브 종료시 삭제	
    private void ClearEnemies()
    {
        if (false) // 웨이브 종료 조건
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].SetActive(false);
            }
        }
    }

    //적 미리 생성
    private void CreateEnemy(List<GameObject> enemyTypeint, int enemyNumber, string enemyName, int prefabN)
    {
        // 희망생성 수보다 리스트가 적으면 새로 만들어서 추가
        if (enemyTypeint.Count < enemyNumber)
        {
            for (int i = 0; i < enemyNumber; i++)
            {
                GameObject enemyType = Instantiate(_enemyPrefabs[prefabN]);
                //IEnemy enemy = enemyType.GetComponent<IEnemy>();

                enemyType.name = $"{enemyName}-{i}";
                enemyType.tag = "Enemy";
                enemyType.transform.parent = transform.Find("EnemySpwaner");

                enemyTypeint.Add(enemyType);

                enemyType.SetActive(false);
                Debug.Log($"{enemyTypeint[i].name}");
            }
        }
        // 희망생성 수보다 리스트가 많으면 첫번째 요소 삭제
        else if (enemyTypeint.Count > enemyNumber)
        {
            // 삭제할 개수
            int deleteEnemy = enemyTypeint.Count - enemyNumber;

            for (int i = 0; i < deleteEnemy; i++)
            {
                // 제거할 요소
                string name = enemyTypeint[enemyTypeint.Count - 1].name;
                // 리스트 맨 뒤 요소 삭제
                enemyTypeint.Remove(enemyTypeint[enemyTypeint.Count-1]);
                // 오브젝트 제거
                GameObject.Destroy(GameObject.Find($"{name}"));

                Debug.Log($"{name} 삭제");
            }
            
        }
    }

    //리스트 순서 셔플
    private void ShuffleList()
    {
        // 적 리스트 병함
        activeEnemies.AddRange(normalEnemies);
        activeEnemies.AddRange(tankEnemies);
        activeEnemies.AddRange(speedEnemies);
        activeEnemies.AddRange(bossEnemies);


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
