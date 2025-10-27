using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float _baseHp = 100;//체력
    [SerializeField] private float _baseMaxHp = 100;//최대체력
    [SerializeField] private float _baseHpRegen = 0.1f; //기본체력리젠
    [SerializeField] private float _baseDef = 0f; //방어력
    [SerializeField] private int _gold;//보유 골드
    [SerializeField] private float _baseGoldMultiplier = 0f; //골드획득비율



    [SerializeField] private float _detectRange = 5;//적 탐색 범위
    [SerializeField] private Weapon _weapon;//무기 넣을것
    private float _currentHp;//변동된 현재 체력
    private bool _isDead = false;//사망여부
    private readonly Dictionary<UpgradeType, float> _upgradeValue = new()//딕셔너리로 상승폭 관리
    {
        {UpgradeType.MaxHp, 10f },
        {UpgradeType.HpRegen, 0.1f },
        {UpgradeType.DetectRange, 0.5f },
        {UpgradeType.Defense, 0.5f },
        {UpgradeType.GoldMultiplier, 0.05f }
    };
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
    private void Start()
    {
        _currentHp = _baseMaxHp;
        _isDead = false;
    }
    private void Update()
    {
        if (GameManager.Instance.IsPlaying() == false)
        {
            return;
        }
    }
    public void AddGold(int amount)
    {
        int finalGold = Mathf.RoundToInt(amount * (1 + _baseGoldMultiplier));
        _gold += finalGold;
        OnGoldChanged?.Invoke(_gold);
    }
    public bool SpendGold(int cost)
    {
        if (_gold < cost)//금액 부족시 false
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

    private void Attack(List<IEnemy> enemies)//가까운적 찾아서 공격
    {
        IEnemy target = FindClosestEnemy(enemies);
        //~~~무기의 공격을 호출
    }

    public void TakeDamage(float damage)
    {
        _currentHp -= damage;
        OnStatsChanged?.Invoke();
        if (_currentHp <= 0)
        {
            Die();
            GameManager.Instance.GameOver();
        }
    }

    private void TickTime(float deltaTime)
    {

    }

    private void UpgradeStats(UpgradeType type)
    {
        int cost = GetUpgradeCost(type);

        if (SpendGold(cost) == false)//골드가 부족하면
        {
            return;
        }
        if (_upgradeValue.ContainsKey(type) == false)//딕셔너리에 타입이 없다면
        {
            return;
        }
        float value = _upgradeValue[type]; //딕셔너리에 저장된 상승폭
        switch (type)
        {
            case UpgradeType.MaxHp:
                MaxHpUp(value);
                break;
            case UpgradeType.Defense:
                DefUp(value);
                break;
            case UpgradeType.HpRegen:
                HpRegenUp(value);
                break;
            case UpgradeType.DetectRange:
                DetectRangeUp(value);
                break;
            case UpgradeType.GoldMultiplier:
                GoldMultiplierUp(value);
                break;
            case UpgradeType.Weapon:
                if (_weapon != null)
                {
                    _weapon.UpgradeWeapon();//무기 업그레이드 호출
                }
                break;
        }
        OnStatsChanged?.Invoke();
    }
    private int GetUpgradeCost(UpgradeType type)//업그레이드 비용 관련
    {
        switch (type)
        {
            case UpgradeType.MaxHp:
                return 100;
            case UpgradeType.Defense:
                return 150;
            case UpgradeType.HpRegen:
                return 120;
            case UpgradeType.DetectRange:
                return 200;
            case UpgradeType.GoldMultiplier:
                return 300;
            case UpgradeType.Weapon:
                return 500;
            default:
                return 0;
        }
    }
    private IEnemy FindClosestEnemy(List<IEnemy> enemies)//가까운 적 찾기
    {
        IEnemy ClosestEnemy = null;
        float ClosestDistance = float.MaxValue;
        foreach (IEnemy enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, enemy.Transform.position);
            if (distance < _detectRange && distance < ClosestDistance)
            {
                ClosestEnemy = enemy;
                ClosestDistance = distance;
            }
        }
        return ClosestEnemy;
    }
    private void Die()
    {
        _isDead = true;
        gameObject.SetActive(false);
    }

    private void HpRegenUp(float value)
    {
        _baseHpRegen += value;
    }
    private void MaxHpUp(float value)
    {
        _baseMaxHp += value;
    }
    private void DefUp(float value)
    {
        _baseDef += value;
    }
    private void DetectRangeUp(float value)
    {
        _detectRange += value;
    }
    private void GoldMultiplierUp(float value)
    {
        _baseGoldMultiplier += value;
    }

    public enum UpgradeType
    {
        MaxHp,
        Defense,
        HpRegen,
        DetectRange,
        GoldMultiplier,
        Weapon

    }
}