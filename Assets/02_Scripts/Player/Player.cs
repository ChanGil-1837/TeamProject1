using System;
using System.Collections;
using System.Collections.Generic;
using TeamProject.GameSystem; //아마도 게임매니저쪽 네임스페이스
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace JHJ
{
    [RequireComponent(typeof(LineRenderer3D))]
    public class Player : MonoBehaviour
    {
        [Header("Ingame Stats")]
        [SerializeField] private float _baseMaxHp = 100f;//최대체력
        [SerializeField] private float _baseHpRegen = 0.1f; //기본체력리젠
        [SerializeField] private float _baseDef = 0f; //방어력
        [SerializeField] private float _detectRange = 5f;//적 탐색 범위

        [Header("Player Gold")]
        [SerializeField] private int _gold;//보유 골드

        [Header("Runtime State")]
        [SerializeField] private SphereCollider _detectCollider;//탐색용 콜라이더
        private LineRenderer3D _visualRange;
        private float _currentHp;//변동된 체력(현재 체력임 변동 되기에 따로 이름을 바꾼것)
        private bool _isDead = false;//사망여부

        [Header("Weapon")]
        [SerializeField] private List<Weapon> _weapons = new List<Weapon>();//무기 넣을것들(추가식으로 하니까 리스트로 작성)

        //[Header("Enemy Spawn Data")]
        //[SerializeField] private EnemySpawner _enemySpawner;//인스펙터로 연결해서 리스트받을까함(보류)

        private const float RegenInterval = 1f; //1초마다 회복
        private float _regenTimer = 0f; //재생 누적용 타이머
        private float _scanInterval = 0.1f;  //0.1초마다 탐색(0.5초는 반응이 늦음)
        private float _scanTimer = 0f; //스캔 누적용타이머
        private List<IEnemy> _detectedEnemies = new List<IEnemy>(); //탐지된 적 리스트


        //읽기전용 프로퍼티 아직은 어떻게 돌아갈지 몰라서 대충 지정해 둠
        public float CurrentHp => _currentHp;//변동된 체력
        public float MaxHp => _baseMaxHp;
        public float DetectRange => _detectRange;//이름 수정함
        public float HpRegen => _baseHpRegen;
        public float Defense => _baseDef;
        public int Gold => _gold;
        public bool IsDead => _isDead;
        public List<IEnemy> DetectedEnemies => _detectedEnemies;//적위치 프로퍼티로 열람

        public event Action OnStatsChanged; //스텟 수치 변화시 호출
        public event Action<int> OnGoldChanged; //골드 변화시 호출

        private void Awake()
        {
            //방어코드
            if (_detectCollider == null)
            {
                _detectCollider = GetComponent<SphereCollider>();//없는지 확인
            }
            if (_detectCollider == null)
            {
                _detectCollider = gameObject.AddComponent<SphereCollider>();//없다면 찾아서 연결
            }
            _detectCollider.isTrigger = true; // 트리거로 강제 설정
            _detectCollider.radius = _detectRange; //초기 범위 동기화
            _visualRange = GetComponentInChildren<LineRenderer3D>();
            if (_visualRange != null)
            {
                _visualRange.target = transform;
                _visualRange.radius = _detectRange;
            }
        }
        private void Start()
        {
            Init();
        }
        private void Update()
        {
            if (_isDead == true)//|| GameManager.Instance?.IsPlaying() == false)//정지 혹은 사망이라면
            {
                return;
            }
            _regenTimer += Time.deltaTime; //프레임당 시간 누적

            if (_regenTimer >= RegenInterval) //누적시간 >= 1초
            {
                _regenTimer = 0f; //초기화
                HpRegenTime(RegenInterval); //리젠타임 1초간격
            }
        }
        private void LateUpdate()
        {
            if (_isDead)
            {
                return;
            }
            _scanTimer += Time.deltaTime; //프레임당 시간 누적
            if (_scanTimer >= _scanInterval) //누적시간 >= 0.1초
            {
                _scanTimer = 0f;
                Scan();
            }
            if (_detectedEnemies.Count > 0)
            {
                IEnemy target = FindClosestEnemy(_detectedEnemies);
                if (target != null)
                {
                    Vector3 dir = (target.Transform.position - transform.position).normalized;

                    transform.rotation = Quaternion.LookRotation(dir);

                    foreach (Weapon weapon in _weapons)
                    {
                        weapon?.Fire(target);
                    }
                }
            }
        }
        /// <summary>
        /// 플레이어 초기화
        /// </summary>
        public void Init()
        {
            _baseMaxHp = 100f;
            _currentHp = _baseMaxHp;
            _detectRange = 5f;
            _baseDef = 0f;
            _baseHpRegen = 0.1f;
            _isDead = false;
            _regenTimer = 0f;
            _scanTimer = 0f;
            OnStatsChanged?.Invoke();
        }
        /// <summary>
        /// 골드 획득
        /// </summary>
        /// <param name="amount">획득량</param>
        public void AddGold(int amount)
        {
            _gold += amount;
            OnGoldChanged?.Invoke(_gold);
        }
        /// <summary>
        /// 플레이어가 입은 피해
        /// </summary>
        /// <param name="damage">피해량</param>
        public void TakeDamage(float damage)
        {
            if(_isDead)
            {
                return;
            }
            damage = Mathf.Max(damage - _baseDef, 1f);//방어력이 공격력보다 높다면 무적상태를 줄것인가? 기본1은 받도록 해둠
            _currentHp -= damage;
            _currentHp = Mathf.Max(_currentHp, 0f);
            Debug.Log($"플레이어 남은 체력{_currentHp} ");
            OnStatsChanged?.Invoke();
            if (_currentHp <= 0)
            {
                Die();
                GameManager.Instance?.GameOver();
            }
        }
        /// <summary>
        /// 플레이어 스텟 업그레이드
        /// </summary>
        /// <param name="type">업그레이드 타입</param>
        public void UpgradeStats(UI.UpgradeType type, float value)
        {
            switch (type)//효과 적용
            {
                case UI.UpgradeType.MaxHp:
                    MaxHpUp(value);
                    break;
                case UI.UpgradeType.Defense:
                    DefUp(value);
                    break;
                case UI.UpgradeType.HpRegen:
                    HpRegenUp(value);
                    break;
                case UI.UpgradeType.DetectRange:
                    DetectRangeUp(value);
                    break;
            }
            OnStatsChanged?.Invoke();
        }
        /// <summary>
        /// 골드 사용 가능 여부
        /// </summary>
        /// <param name="cost">비용</param>
        /// <returns></returns>
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
        /// <summary>
        /// 무기 추가
        /// </summary>
        /// <param name="weapon"></param>
        private void AddWeapon(Weapon weapon)
        {
            if (weapon == null)
            {
                return;
            }
            if (_weapons.Contains(weapon))//같은 무기면 중복 방지
            {
                return;
            }
            weapon.transform.SetParent(transform); //플레이어 자식으로
            weapon.transform.localPosition = Vector3.zero; //위치 초기화
            weapon.transform.localRotation = Quaternion.identity;
            _weapons.Add(weapon);
        }

        private void HpRegenTime(float deltaTime)//알기쉽게 메서드명 변경
        {
            _currentHp = Mathf.Min(_currentHp + _baseHpRegen * deltaTime, _baseMaxHp);
            OnStatsChanged?.Invoke();
        }
        /// <summary>
        /// 가까운적 찾기용
        /// </summary>
        /// <param name="enemies"></param>
        /// <returns></returns>
        private IEnemy FindClosestEnemy(List<IEnemy> enemies)//가까운 적 찾기
        {
            if (enemies == null || enemies.Count == 0)//null 방지용
            {
                return null;
            }
            IEnemy closestEnemy = null;
            float closestDistance = float.MaxValue; //거리 비교용(최대값)
            foreach (IEnemy enemy in enemies) //전달받은 적 순회
            {
                if (enemy == null || enemy.IsDead) //null 이거나 죽은 적 무시
                {
                    continue;
                }
                float enemyDistance = (transform.position - enemy.Transform.position).sqrMagnitude;//적 거리 계산
                if (enemyDistance < closestDistance)//적 거리가 탐지범위 작다면
                {
                    closestEnemy = enemy; //가장 가까운 적 갱신
                    closestDistance = enemyDistance; //가장 가까운 적 거리 갱신
                }
            }
            return closestEnemy; //가장 가까운 적 리턴
        }

        /// <summary>
        /// 적 탐지용
        /// </summary>
        private void Scan()
        {
            if (_isDead)//플레이어 사망시 정지
            {
                return;
            }
            _detectedEnemies.RemoveAll(IsEnemyClear);//리스트에서 null,죽은적 제외
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");//적 태그를 가진 모든 오브젝트(수정해야 될 수도있음)
            foreach (GameObject obj in enemies)
            {
                if (obj.activeInHierarchy && obj.TryGetComponent<IEnemy>(out IEnemy enemy))//활성화,적인터페이스가진 적만
                {
                    if (enemy.IsDead)
                    {
                        continue;
                    }
                    float sqrDist = (transform.position - enemy.Transform.position).sqrMagnitude;
                    if (sqrDist <= _detectRange * _detectRange)
                    {
                        if (_detectedEnemies.Contains(enemy) == false)
                        {
                            _detectedEnemies.Add(enemy);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 죽은적 처리용
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private bool IsEnemyClear(IEnemy enemy)//null이거나 죽은적은 true
        {
            return enemy == null || enemy.IsDead;
        }

        private void Die()
        {
            _isDead = true;
            gameObject.SetActive(false);
            Debug.Log("플레이어 사망");
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
            if (_detectCollider != null)
            {
                _detectCollider.radius = _detectRange;//범위 갱신
            }
        }
        /// <summary>
        /// 수정될 때마다 호출되는 함수
        /// </summary>
        private void OnValidate()//콜라이더 실시간 반영
        {
            if (_detectCollider == null)//방어코드
            {
                _detectCollider = GetComponent<SphereCollider>();//없다면 복구
            }
            if (_detectCollider != null)
            {
                _detectCollider.radius = _detectRange;//탐색범위로
            }
            if (_visualRange == null)
            {
                _visualRange = GetComponentInChildren<LineRenderer3D>();
            }
                if (_visualRange != null)
            {
                _visualRange.radius = _detectRange;
                _visualRange.UpdateSync();
            }
        }
        private void OnDrawGizmosSelected()//기즈모로 탐지범위 시각화
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectRange);
        }

    }

}
