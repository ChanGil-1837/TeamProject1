using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float _baseHp = 100;//체력
    [SerializeField] private float _baseMaxHp = 100;//최대체력
    [SerializeField] private float _detectRange = 5;//적 탐색 범위
    [SerializeField] private float _baseHpRegen = 0.1f; //기본체력리젠
    [SerializeField] private float _baseDef = 0f; //방어력
    [SerializeField] private int _gold;
    [SerializeField] private float _baseGoldMultiplier = 0.1f; //골드획득비율
    //베이스능력치를 어떻게 할것인가...?
    [SerializeField] private Weapon _weapon;//무기 넣을것
    private float _currentHp;//
    //읽기전용 프로퍼티 아직은 어떻게 돌아갈지 몰라서 대충 지정해둠
    public float Hp => _baseHp;
    public float MaxHp => _baseMaxHp;
    public float AttackRange => _detectRange;
    public float HpRegen => _baseHpRegen;
    public float Defense => _baseDef;
    public int Gold => _gold;
    public float GoldMultiplier => _baseGoldMultiplier;

    public event Action OnStatsChanged;
    public event Action<int> OnGoldChanged;

    public void AddGold(int amount)
    {
        int finalGold = Mathf.RoundToInt(amount * (1 + _baseGoldMultiplier));
        _gold += finalGold;
        OnGoldChanged?.Invoke(_gold);
    }
    public bool SpendGold(int cost)
    {
        if (_gold < cost)
        {
            return false;
        }
        _gold -= cost;
        OnGoldChanged?.Invoke(_gold);
        return true;
    }
    private void AddWeapon(Weapon weapon)
    {

    }

    private void Attack(List<Enemy> enemies)//가까운적 찾아서 공격
    {
        Enemy target = FindClosestEnemy(enemies);
        //~~~무기의 공격을 호출
    }

    public void TakeDamage(float damage)
    {
        _currentHp -= damage;
        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void TickTime(float deltaTime)
    {

    }

    private void UpgradeStats(UpgradeType type, int cost, float value)
    {
        if (!SpendGold(cost))
        {
            return;
        }
        switch(type)
        {
            case UpgradeType.MaxHp:
                _baseMaxHp += value;
                break;
            case UpgradeType.Defense:
                _baseDef += value;
                break;
            case UpgradeType.HpRegen:
                _baseHpRegen += value;
                break;
            case UpgradeType.DetectRange:
                _detectRange += value;
                break;
            case UpgradeType.GoldMultiplier:
                _baseGoldMultiplier += value;
                break;
            case 

            
        }
        OnStatsChanged?.Invoke();
​    }
    public bool TryUpgradeWeapon(Weapon weapon, int cost)//무기 업그레이드가 가능한가
    {
        if (_gold < cost)
        {
            return false;
        }
        _gold -= cost;
        weapon.UpgradeWeapon();
        return true;
    }

    private Enemy FindClosestEnemy(List<Enemy> enemies)//가까운 적 찾기
    {
        
        return;
    }
    private void Die()
    {
        gameObject.SetActive(false);
    }

    private void RegenHp()
    {

    }
    public enum UpgradeType
    {
        MaxHp,
        Defense,
        HpRegen,
        DetectRange,
        GoldMultiplier,
        TryUpgradeWeapon
    }
}
