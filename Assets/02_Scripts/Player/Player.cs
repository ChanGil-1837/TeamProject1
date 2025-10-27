using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float _baseHp = 100;//현재체력
    [SerializeField] private float _baseMaxHp = 100;//최대체력
    [SerializeField] private float _baseAttackRange = 5;//기본공격범위
    [SerializeField] private float _baseAttackInterval = 1f;//기본공속(공격주기)
    [SerializeField] private float _baseAtt = 10f; //기본공격력
    [SerializeField] private float _baseCriChance = 0.1f; //기본크리확률
    [SerializeField] private float _baseCriMultiplier = 1.5f; //기본크리배율
    [SerializeField] private float _baseHpRegen = 0.1f; //기본체력리젠
    [SerializeField] private float _baseDef = 0f; //방어력
    [SerializeField] private float _baseGoldMultiplier = 0.1f; //골드획득비율
    //베이스능력치를 어떻게 할것인가...?
    [SerializeField] private Weapon _weapon;//무기 넣을것
    //읽기전용 프로퍼티 아직은 어떻게 돌아갈지 몰라서 대충 지정해둠
    public float Hp => _baseHp;
    public float MaxHp => _baseMaxHp;
    public float AttackRange => _baseAttackRange;
    public float AttackInterval => _baseAttackInterval;
    public float Att => _baseAtt;
    public float CriticalChance => _baseCriChance;
    public float CriticalMultiplier => _baseCriMultiplier;
    public float HpRegen => _baseHpRegen;
    public float Defense => _baseDef;
    public float GoldMultiplier => _baseGoldMultiplier;


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

    }

    private void TickTime(float deltaTime)
    {

    }

    private void UpgradeStats()//스텟?무기? 어떤걸 업그레이드 하는식으로 할것인가...?
    {

​    }

    private Transform FindClosestEnemy(List<Enemy> enemies)//가까운 적 찾기
    {
        return;
    }

    private void RegenHp()
    {

    }
}
