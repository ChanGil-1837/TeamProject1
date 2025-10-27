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
    [Header("���ݷ�")]
    [SerializeField] private float damage;

    [Header("���� �ֱ�")]
    [SerializeField] private float baseInterval;

    [Header("����")]
    [SerializeField] private int level;

    [Header("����ü")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] protected int projectileCount;

    [Header("Ǯ")]
    [SerializeField] private int poolSize;

    public float Damage => damage;

    private Queue<Projectile> projectilePool = new();
    private Coroutine attackCoroutine;
    private WaitForSeconds attackInterval;
    private WaitUntil fireState;

    // �׽�Ʈ��
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

    #region Ǯ��
    // ���ο� ����ü ����
    private Projectile NewProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab, transform);
        newProjectile.Init(this);
        newProjectile.gameObject.SetActive(false);
        projectilePool.Enqueue(newProjectile);

        return newProjectile;
    }

    // Ǯ���� ����ü ���
    protected Projectile GetFromPool()
    {
        Projectile projectile;

        // ���� ����ü ����
        if (projectilePool.Count <= 0)
        {
            projectile = NewProjectile();
        }

        // Ǯ ����ü ���
        projectile = projectilePool.Dequeue();

        // Ȱ��ȭ
        projectile.gameObject.SetActive(true);

        return projectile;
    }
    // ����ü Ǯ ��ȯ
    public void ReturnToPool(Projectile projectile)
    {
        projectilePool.Enqueue(projectile);
        projectile.gameObject.SetActive(false);

    }
    #endregion

    //---------------------------------------------------

    #region ����

    // �߻�
    public abstract void Fire();

    // ���� �ڷ�ƾ
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

    // ���׷��̵�
    public void UpgradeWeapon() { }
}
