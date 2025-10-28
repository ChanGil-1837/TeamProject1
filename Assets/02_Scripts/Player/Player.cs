using System;
using System.Collections;
using System.Collections.Generic;
using TeamProject.GameSystem;
using UnityEngine;

public enum UpgradeType
{
    MaxHp,
    Defense,
    HpRegen,
    DetectRange,
    GoldMultiplier,
    Weapon
}

public class Player : MonoBehaviour
{
    [Header("Ingame Stats")]
    [SerializeField] private float _baseMaxHp = 100f;//최대체력
    [SerializeField] private float _baseHpRegen = 0.1f; //기본체력리젠
    [SerializeField] private float _baseDef = 0f; //방어력
    [SerializeField] private float _baseGoldMultiplier = 0f; //골드획득비율
    [SerializeField] private float _detectRange = 5f;//적 탐색 범위

    [Header("Player Gold")]
    [SerializeField] private int _gold;//보유 골드

    [Header("Runtime State")]
    private float _currentHp;//변동된 체력(현재 체력임 변동 되기에 따로 이름을 바꾼것)
    private bool _isDead = false;//사망여부

    [Header("Weapon")]
    [SerializeField] private List<Weapon> _weapon = new List<Weapon>();//무기 넣을것들(추가식으로 하니까 리스트로 작성)

    //딕셔너리로 비용, 능력치 상승 관리
    [Header("Upgrade Data")]
    private readonly Dictionary<UpgradeType, (int cost, float value)> _upgradeValue = new Dictionary<UpgradeType, (int cost, float value)>
    {
        {UpgradeType.MaxHp, (100, 10f) }, //비용, 상승치
        {UpgradeType.HpRegen, (120, 0.1f) },
        {UpgradeType.DetectRange, (200, 0.5f) },
        {UpgradeType.Defense, (150, 0.5f) },
        {UpgradeType.GoldMultiplier, (500, 0.05f) }
    };
    private const float CostMultiplier = 1.2f; //비용 비율 20퍼
    private const float RegenInterval = 1f; //1초마다 회복
    private float _regenTimer = 0f;//누적용 타이머


    //읽기전용 프로퍼티 아직은 어떻게 돌아갈지 몰라서 대충 지정해둠
    public float CurrentHp => _currentHp;//변동된 체력
    public float MaxHp => _baseMaxHp;
    public float AttackRange => _detectRange;
    public float HpRegen => _baseHpRegen;
    public float Defense => _baseDef;
    public int Gold => _gold;
    public float GoldMultiplier => _baseGoldMultiplier;
    public bool IsDead => _isDead;


    public event Action OnStatsChanged; //스텟 수치 변화시 호출
    public event Action<int> OnGoldChanged; //골드 변화시 호출

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (_isDead == true || GameManager.Instance.IsPlaying() == false)//정지 혹은 사망이라면
        {
            return;
        }
        _regenTimer += Time.deltaTime; //프레임당 시간 누적

        if (_regenTimer >= RegenInterval) //누적시간 >= 1초
        {
            TickTime(RegenInterval); //틱타임 1초간격
            _regenTimer = 0f; //다시 초기화
        }
    }
    public void Init()
    {
        _baseMaxHp = 100f;
        _currentHp = _baseMaxHp;
        _detectRange = 5f;
        _baseDef = 0f;
        _baseGoldMultiplier = 0f;
        _baseHpRegen = 0.1f;
        _isDead = false;
        _regenTimer = 0f;
        OnStatsChanged?.Invoke();
    }
    public void AddGold(int amount)//골드 획득부분은 적리워드 필요(이야기할 필요가 있음),게임매니저의 개입이 낫다 생각됨(적죽음관련)
    {
        int finalGold = Mathf.RoundToInt(amount * (1 + _baseGoldMultiplier));
        _gold += finalGold;
        OnGoldChanged?.Invoke(_gold);
    }
    
    public void Attack(List<IEnemy> enemies)//가까운적 찾아서 공격
    {
        if (_weapon == null)
        {
            return;
        }
        IEnemy target = FindClosestEnemy(enemies);//가까운 적의 위치를 찾고
        if (target != null)//타겟이 있다면
        {
            //무기가 위치정보 받을 메서드 필요(이야기할 필요가 있음)
            //_weapon.Fire();
        }

    }
    public void TakeDamage(float damage)
    {
        damage = Mathf.Max(damage - _baseDef, 1f);//방어력이 공격력보다 높다면 무적상태를 줄것인가? 기본1은 받도록 해둠
        _currentHp -= damage;
        OnStatsChanged?.Invoke();
        if (_currentHp <= 0)
        {
            Die();
            GameManager.Instance.GameOver();
        }
    }
    public void UpgradeStats(UpgradeType type)
    {
        if (_upgradeValue.TryGetValue(type, out (int cost, float value) data) == false)//딕셔너리에 해당 업그레이드 타입 정보가 없다면
        {
            return;
        }
        int cost = data.cost;
        float value = data.value;
        if (SpendGold(cost) == false)//골드가 부족하면
        {
            return;
        }
        switch (type)//효과 적용
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
                    //_weapon.UpgradeWeapon();//무기 업그레이드 호출
                }
                break;
        }
        int newCost = Mathf.RoundToInt(cost * CostMultiplier); //상승된 비용 적용
        _upgradeValue[type] = (newCost, value); //딕셔너리에 반영
        OnStatsChanged?.Invoke();
    }
    public void Reset()//상점을 만들어서 기본능력치를 더 올릴것이라고 한다면 추가할 예정,아니라고 하면 삭제
    {
        Init();//현재 동작은 Init과 같음
    }
    private bool SpendGold(int cost)
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
        if (weapon == null)
        {
            return;
        }
        //_weapon = weapon; //무기 저장
        //_weapon.transform.SetParent(transform); //플레이어 자식으로
        //_weapon.transform.localPosition = Vector3.zero; //위치 초기화
        //_weapon.transform.localRotation = Quaternion.identity;

    }

    private void TickTime(float deltaTime)
    {
        _currentHp = Mathf.Min(_currentHp + _baseHpRegen * deltaTime, _baseMaxHp);
        OnStatsChanged?.Invoke();
    }

    private IEnemy FindClosestEnemy(List<IEnemy> enemies)//가까운 적 찾기
    {
        IEnemy closestEnemy = null;
        //float closestDistance = float.MaxValue; //거리 비교용(최대값)
        //foreach (IEnemy enemy in enemies) //전달받은 적 순회
        //{
        //    if (enemy == null)// || enemy.IsDead) //null 이거나 죽은 적 무시
        //    {
        //        continue;
        //    }
        //    float enemyDistance = (transform.position - enemy.Transform.position).sqrMagnitude;//적 거리 계산
        //    if (enemyDistance < (_detectRange * _detectRange) && enemyDistance < closestDistance)//적 거리가 탐지범위 그리고 가장 가까운 적 범위보다 작다면
        //    {
        //        closestEnemy = enemy; //가장 가까운 적 갱신
        //        closestDistance = enemyDistance; //가장 가까운 적 거리 갱신
        //    }
        //}
        return closestEnemy; //가장 가까운 적 리턴
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
        _baseMaxHp += value; //최대 체력 상승
        _currentHp = Mathf.Min(_currentHp, _baseMaxHp);//현재 체력은 변동 없도록
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

}