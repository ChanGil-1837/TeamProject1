using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpwaner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemyPrefabs;// 프리팹
    [SerializeField] private int _normalEnemyNumber;
    [SerializeField] private int _tankEnemyNumber;
    [SerializeField] private int _speedEnemyNumber;
    [SerializeField] private int _bossEnemyNumber;
    [SerializeField][Range(5f, 10f)] private float _attackRange;

    private List<Transform> spawnPoints; // 스폰 포인트	
    private float spawnerInterval; // 적 생성주기	
    //private List<GameObject> activeEnemies = new List<GameObject>(); // 생성한 적 리스트
    //private List<GameObject> suffleActiveEnemies = new List<GameObject>(); // 적 셔플 리스트
    private List<EnemyData> activeEnemies = new List<EnemyData>();
    private List<EnemyData> suffleActiveEnemies = new List<EnemyData>();

    int enemyTurn = 0;
    public struct EnemyData
    {
        public GameObject gameOJ;
        public IEnemy enemyInterface;

        public EnemyData(GameObject GO, IEnemy IF)
        {
            gameOJ = GO;
            enemyInterface = IF;
        }
    }


    //리스트 번호
    private int ListN = 0;



    private void Awake()
    {
        CreateEnemy(_normalEnemyNumber, "Normal", 0);
        CreateEnemy(_tankEnemyNumber, "Tank", 1);
        CreateEnemy(_speedEnemyNumber, "Speed", 2);
        CreateEnemy(_bossEnemyNumber, "Boss", 3);

        ShuffleList();

    }

    // 타이머 갱신
    private void Update()
    {
        SpawnEnemy();
        ClearEnemies();
    }

    // 적 생성	
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

                    if (direction.magnitude >= _attackRange-1)
                    {
                        activeEnemies[enemyTurn].gameOJ.transform.position = direction;
                        activeEnemies[enemyTurn].gameOJ.SetActive(true);
                        enemyTurn++;
                        Debug.Log($"{enemyTurn}번 적 생성");
                        break;
                    }
                }
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
                activeEnemies[i].gameOJ.SetActive(false);
            }
        }
    }

    //적 미리 생성
    private void CreateEnemy(int enemyNumber, string enemyName, int prefabN)
    {
        //적 번호
        int enemyN = 0;

        for (int i = 0; i < enemyNumber; i++)
        {
            GameObject enemyType = Instantiate(_enemyPrefabs[prefabN]);
            IEnemy enemy = enemyType.GetComponent<IEnemy>();

            enemyType.name = $"{enemyName}-{enemyN}";
            enemyType.tag = "Enemy";
            enemyType.transform.parent = transform.Find("EnemySpwaner");

            activeEnemies.Add(new EnemyData(enemyType, enemy));

            enemyType.SetActive(false);
            Debug.Log($"{activeEnemies[ListN].gameOJ.name}");
            enemyN++;
            ListN++;
        }
    }

    //리스트 순서 셔플
    private void ShuffleList()
    {
        System.Random random = new System.Random();
        // 리스트 전체 길이 저장
        int n = activeEnemies.Count;

        Debug.Log($"적 개수 : {activeEnemies.Count}");
        List<EnemyData> temp = new List<EnemyData>();
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

            Debug.Log($"셔플리스트 : {i} : {suffleActiveEnemies[i].gameOJ.name}");
        }

        activeEnemies = temp;

        Debug.Log($"셔플 리스트 총 개수 : {suffleActiveEnemies.Count}");

    }
}
