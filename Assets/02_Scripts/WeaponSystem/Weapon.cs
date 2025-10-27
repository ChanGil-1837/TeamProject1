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

    [Header("���ݷ�")]
    [SerializeField] private float damage;

    [Header("���� �ֱ�")]
    [SerializeField] private float baseInterval;
    private Coroutine attackCoroutine;
    private WaitForSeconds attackInterval;
    private WaitUntil fireState;

    [Header("����")]
    [SerializeField] private int level;

    [Header("����ü")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int projectileCount;

    [Header("Ǯ")]
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
    private Projectile GetFromPool()
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

    // �߻�
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
        // ���׷��̵� ��ġ ����
    }
}
