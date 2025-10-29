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
    [SerializeField] private float velocity = 5;

    [Space]
    [SerializeField] private float interval = 0.5f;

    [Space]
    [SerializeField] private int poolSize = 30;

    [Space]
    [SerializeField] private Projectile projectilePrefab;


    public int Damage => damage;
    public float Velocity => velocity;
    public int DamageLevel => damageLevel;
    public int IntervalLevel => intervalLevel;


    private int damageLevel;
    private int intervalLevel;


    private Queue<Projectile> projectilePool = new();
    private Coroutine attackCoroutine;
    private WaitForSeconds attackInterval;
    private WaitUntil fireState;
    private bool canFire;

    protected Player player;



    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        player = GetComponentInParent<Player>();

        fireState = new WaitUntil(() => canFire);
        attackInterval = new WaitForSeconds(interval);

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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpgradeFireRate(1.1f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpgradeFireRate(0.9f);
        }
    }

    private void OnDisable()
    {
        canFire = false;
    }
    private void OnDestroy()
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
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

    public abstract void Fire(IEnemy enemy);

    IEnumerator Attack()
    {
        while (true)
        {
            yield return fireState;

            IEnemy enemy = player.ClosestEnemy;

            if(enemy != null)
            {
                Fire(enemy);
            }

            yield return attackInterval;
        }
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
        attackInterval = new WaitForSeconds(interval);
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


    public void ChangeFireState()
    {
        canFire = !canFire;
    }

}
