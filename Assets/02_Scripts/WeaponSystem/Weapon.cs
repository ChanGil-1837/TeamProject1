using System.Collections;
using System.Collections.Generic;
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

    private float damage;

    private float baseInterval;
    private float currentInterval;

    private int level;

    private Projectile projectilePrefab;
    private int projectileCount;

    private int poolSize;
    private Queue<Projectile> projectilePool = new();


    void InitializePool()
    {
        // 투사체 풀 초기화
    }

    private Projectile NewProjectile()
    {
        // 새로운 투사체 생성
        return null;
    }
    private Projectile GetBulletFromPool()
    {
        // 풀에서 투사체 꺼냄
        return null;
    }
    public void ReturnToPool(Projectile projectile)
    {
        // 투사체 풀 반환
    }


    public void Attack(Vector2 dir)
    {
        // 투사체 발사
    }

    public void TickTime(float deltaTime)
    {
        // 공격 주기 갱신
    }

    public void UpgradeWeapon()
    {
        // 업그레이드 수치 적용
    }
}
