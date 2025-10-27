using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum WeaponType
{
    Normal,
    Multi,
    Bounce,
}

public abstract class Weapon : MonoBehaviour
{
    [Header("공격력")]
    [SerializeField] private float damage;

    [Header("공격 주기")]
    [SerializeField] private float baseInterval;

    [Header("레벨")]
    [SerializeField] private int level;

    [Header("투사체")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] protected int projectileCount;

    [Header("풀")]
    [SerializeField] private int poolSize;

    public float Damage => damage;

    private Queue<Projectile> projectilePool = new();
    private Coroutine attackCoroutine;
    private WaitForSeconds attackInterval;
    private WaitUntil fireState;

    // 테스트용
    [SerializeField] protected Transform target;
    private bool canFire;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        fireState = new WaitUntil(() => canFire);
        attackInterval = new WaitForSeconds(baseInterval);

        attackCoroutine = StartCoroutine(Attack());

        for (int i = 0; i < poolSize; i++)
        {
            Projectile projectile = NewProjectile();
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canFire = !canFire;
        }
    }

    //---------------------------------------------------

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
    protected Projectile GetFromPool()
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

    //---------------------------------------------------

    #region 공격

    // 발사
    public abstract void Fire();

    // 공격 코루틴
    IEnumerator Attack()
    {
        while (true)
        {
            yield return fireState;

            Fire();

            yield return attackInterval;
        }
    }
    #endregion

    //---------------------------------------------------

    // 업그레이드
    public void UpgradeWeapon() { }
}
