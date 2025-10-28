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

    [Header("Damage")]
    [SerializeField] private float damage;

    [Header("Interval")]
    [SerializeField] private float baseInterval;

    [Header("Level")]
    [SerializeField] private int level;

    [Header("Prefab")]
    [SerializeField] private Projectile projectilePrefab;

    [Header("Pool")]
    [SerializeField] private int poolSize;

    public float Damage => damage;

    private Queue<Projectile> projectilePool = new();
    private Coroutine attackCoroutine;
    private WaitForSeconds attackInterval;
    private WaitUntil fireState;

    // Test
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

    public abstract void Fire();

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

    #region Upgrade

    public virtual void UpgradeDamage(int upgradeAmount)
    {
        damage += upgradeAmount;
    }
    public virtual void UpgradeFireRate(float fireRateMultiplier)
    {
        baseInterval *= fireRateMultiplier;
        attackInterval = new WaitForSeconds(baseInterval);
    }

    #endregion

    //---------------------------------------------------


    // Set Projectile Active Transform 
    protected void SetProjectileTransform(Projectile projectile, Vector3 direction)
    {
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        projectile.SetDirection(direction);
    }

}
