using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Normal,
    Multi,
    Bounce,
}


public abstract class Weapon : MonoBehaviour
{
    public WeaponType weaponType;

    [Space]
    [SerializeField] private int damage = 1;

    [Space]
    [SerializeField] private float speed = 5f;

    [Space]
    [SerializeField] private float interval = 0.5f;

    [Space]
    [SerializeField] private float lifeTime = 0.5f;

    [Space]
    [SerializeField] private int poolSize = 30;

    [Space]
    [SerializeField] private Projectile projectilePrefab;


    public int Damage => damage;
    public float Speed => speed;
    public float LifeTime => lifeTime;
    public int DamageLevel => damageLevel;
    public int IntervalLevel => intervalLevel;

    // Level
    private int damageLevel;
    private int intervalLevel;


    private Queue<Projectile> projectilePool;

    protected bool canFire;
    protected float intervalTimer;




    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        projectilePool = new(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            Projectile projectile = NewProjectile();
        }
    }


    private void Update()
    {
        if (canFire == true) return;

        if(intervalTimer < interval)
        {
            intervalTimer += Time.deltaTime;

            if (intervalTimer >= interval)
            {
                canFire = true;
            }
        }
    }

    //---------------------------------------------------

    #region Pooling


    private Projectile NewProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab, transform);
        newProjectile.Init(this);
        newProjectile.gameObject.SetActive(false);
        projectilePool.Enqueue(newProjectile);

        return newProjectile;
    }


    protected Projectile GetFromPool()
    {
        Projectile projectile;

        if (projectilePool.Count <= 0)
        {
            projectile = NewProjectile();
        }

        projectile = projectilePool.Dequeue();

        projectile.gameObject.SetActive(true);

        return projectile;
    }
    public void ReturnToPool(Projectile projectile)
    {
        projectilePool.Enqueue(projectile);
        projectile.gameObject.SetActive(false);

    }


    #endregion

    //---------------------------------------------------

    #region Attack


    // �߻�
    public abstract void Fire(IEnemy enemy);


    // ���� üũ
    protected bool CheckCondition(IEnemy enemy)
    {
        if (enemy == null) return false;
        if (canFire == false) return false;

        return true;
    }

    // ���� ���
    protected Vector3 GetDirection(IEnemy enemy)
    {
        if (enemy == null)
            return transform.forward; // 혹시 모를 null 방지용

        // 적의 현재 위치
        Vector3 enemyPos = enemy.transform.position;

        // 현재 오브젝트 위치
        Vector3 selfPos = transform.position;

        // 두 점의 차이를 방향으로 계산
        Vector3 dir = (enemyPos - selfPos);
        dir.y = 0f; // 수평면 기준으로만 쏘고 싶을 때

        // 0벡터 방지
        if (dir.sqrMagnitude < 0.0001f)
            return transform.forward;

        return dir.normalized;
    }


    // �߻� �� ����
    protected void AfterFire()
    {
        canFire = false;
        intervalTimer = 0f;
    }


    #endregion

    //---------------------------------------------------

    #region Upgrade


    public virtual void UpgradeDamage(int upgradeAmount)
    {
        damage += upgradeAmount;
        damageLevel++;
    }

    public virtual void UpgradeFireRate(float fireRateMultiplier)
    {
        interval *= fireRateMultiplier;
        intervalLevel++;
    }


    #endregion

    //---------------------------------------------------


    protected Projectile FireProjectile(Vector3 direction)
    {
        Projectile projectile = GetFromPool();

        SetProjectileTransform(projectile, direction);

        return projectile;
    }


    // Set Projectile Active Transform 
    private void SetProjectileTransform(Projectile projectile, Vector3 direction)
    {
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        projectile.SetVelocity(direction);
    }

}
