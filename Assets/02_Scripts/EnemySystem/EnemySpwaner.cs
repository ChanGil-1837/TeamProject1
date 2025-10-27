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

    private List<Transform> spawnPoints; // 스폰 포인트	
    private float spawnerInterval; // 적 생성주기	
    private List<GameObject> activeEnemies = new List<GameObject>(); // 생성한 적 리스트
    private List<GameObject> suffleActiveEnemies = new List<GameObject>(); // 적 셔플 리스트

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

    }

    // 적 생성	
    private void SpawnEnemy()
    {

    }

    // 웨이브 종료시 삭제	
    private void ClearEnemies()
    {

    }

    //적 미리 생성
    private void CreateEnemy(int enemyNumber, string enemyName, int prefabN)
    {
        //적 번호
        int enemyN = 0;

        for (int i = 0; i < enemyNumber; i++)
        {
            GameObject enemyType = Instantiate(_enemyPrefabs[prefabN]);

            enemyType.name = $"{enemyName}-{enemyN}";
            enemyType.tag = "Enemy";
            enemyType.transform.parent = transform.Find("EnemySpwaner");

            activeEnemies.Add(enemyType);
            enemyType.SetActive(false);
            Debug.Log($"{activeEnemies[ListN].name}");
            enemyN++;
            ListN++;
        }
    }

    //리스트 순서 셔플
    private void ShuffleList()
    {
        System.Random random = new System.Random();

        Debug.Log($"적 개수 : {activeEnemies.Count}");

        // 생성한 적 리스트를 섞어서 새리스트에 저장.
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            random.Next(0, activeEnemies.Count);
            suffleActiveEnemies.Add(activeEnemies[1]);

            Debug.Log($"셔플리스트 : {i} : {suffleActiveEnemies[i].name}");
        }

        Debug.Log($"셔플 리스트 총 개수 : {suffleActiveEnemies.Count}");

    }
}
