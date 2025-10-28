using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 프리팹")]
    [SerializeField] private List<GameObject> _enemyPrefabs;// 프리팹

    [Header("Normal 적 생성")]
    [SerializeField] private int _normalEnemyNumber;

    [Header("Tank 적 생성")]
    [SerializeField] private int _tankEnemyNumber;

    [Header("Speed 적 생성")]
    [SerializeField] private int _speedEnemyNumber;

    [Header("Boss 적 생성")]
    [SerializeField] private int _bossEnemyNumber;

    [Header("적 생성 거리")]
    [SerializeField] private float _attackRange = 10f;
    //private float _spawnerInterval = 1.0f; // 적 생성주기	



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
        GameObject EnemyBox = new GameObject("EnemyBox");

        CreateEnemy(normalEnemies, _normalEnemyNumber, "Normal", 0);
        CreateEnemy(tankEnemies, _tankEnemyNumber, "Tank", 1);
        CreateEnemy(speedEnemies, _speedEnemyNumber, "Speed", 2);
        CreateEnemy(bossEnemies, _bossEnemyNumber, "Boss", 3);

        ShuffleList();
    }

    // 타이머 갱신
    private void Update()
    {
        SpawnEnemy();
        //ClearEnemies();
    }

    // 적 소환
    private void SpawnEnemy()
    {
        if (Input.GetMouseButtonDown(0)) //추후에 웨이브 기준으로 변경
        {
            if (enemyTurn < activeEnemies.Count)
            {
                // 비활성화된 상태에서만 위치 부여 + 활성화
                while (activeEnemies[enemyTurn].activeSelf == false)
                {
                    // 반지름 길이 = 사정거리 에서 몹 스폰
                    Vector3 direction = GameObject.Find("Player").transform.position + (Random.insideUnitSphere * _attackRange);
                    direction.y = 0;

                    // 랜덤 위치가 유효한 거리인지
                    if (direction.magnitude >= _attackRange - 1)
                    {
                        activeEnemies[enemyTurn].transform.position = direction;
                        activeEnemies[enemyTurn].SetActive(true);
                        enemyTurn++;
                        Debug.Log($"{enemyTurn}번 적 활성화");
                        break;
                    }
                }
            }
            else if (activeEnemies.Count == enemyTurn)
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
    //private void ClearEnemies()
    //{
    //    if (GetComponent<GameManagerPrototype>().IsGameOver == true) // 웨이브 종료 조건 추가해야 함.
    //    {
    //        for (int i = 0; i < activeEnemies.Count; i++)
    //        {
    //            activeEnemies[i].SetActive(false);
    //        }
    //    }
    //}


    // 오브젝트 이름 중복을 피하기 위해 부여
    private int EnemyNameID (List<GameObject> enemyTypeint)
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
