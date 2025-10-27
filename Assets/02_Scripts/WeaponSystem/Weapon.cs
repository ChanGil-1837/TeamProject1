using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;


enum WeaponType
{
    Normal,
    Multi,
    Bounce,
}

public class Weapon : MonoBehaviour
{
    //[Header("")]
    //[SerializeField]

    private WeaponType type;

    [Header("공격력")]
    [SerializeField] private float damage;

    [Header("공격 주기")]
    [SerializeField] private float baseInterval;
    private Coroutine attackCoroutine;
    private WaitForSeconds attackInterval;
    private WaitUntil fireState;

    [Header("레벨")]
    [SerializeField] private int level;

    [Header("투사체")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int projectileCount;

    [Header("풀")]
    [SerializeField] private int poolSize;
    private Queue<Projectile> projectilePool = new();

    [SerializeField] Transform target;
    private bool canFire;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canFire = !canFire;
        }
    }

    void Init()
    {
        fireState = new WaitUntil(() => canFire);
        attackInterval = new WaitForSeconds(baseInterval);

        attackCoroutine = StartCoroutine(Attack());    

        for (int i = 0; i < poolSize; i++)
        {
            Projectile projectile = NewProjectile();
        }
    }

    #region 풀링
    // 새로운 투사체 생성
    private Projectile NewProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab, transform);
        newProjectile.Init(this);
        newProjectile.gameObject.SetActive(false);
        projectilePool.Enqueue(newProjectile);

        return newProjectile;
    }

    // 풀에서 투사체 사용
    private Projectile GetFromPool()
    {
        Projectile projectile;

        // 남는 투사체 없음
        if (projectilePool.Count <= 0)
        {
            projectile = NewProjectile();
        }

        // 풀 투사체 사용
        projectile = projectilePool.Dequeue();

        // 활성화
        projectile.gameObject.SetActive(true);

        return projectile;
    }
    // 투사체 풀 반환
    public void ReturnToPool(Projectile projectile)
    {
        projectilePool.Enqueue(projectile);
        projectile.gameObject.SetActive(false);

    }
    #endregion

    // 발사
    public void Fire()
    {
        Projectile projectile = GetFromPool();

        projectile.transform.position = transform.position;

        Vector3 direction = (target.position - projectile.transform.position).normalized;

        projectile.SetDirection(direction);
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return fireState;

            Fire();

            yield return attackInterval;
        }
    }

    public void UpgradeWeapon()
    {
        // 업그레이드 수치 적용
    }
}
