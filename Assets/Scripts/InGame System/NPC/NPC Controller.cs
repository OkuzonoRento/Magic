using UnityEngine;
using UnityEngine.AI;
using BuffSystem.Core;

public class NPCController : MonoBehaviour, IDamageble
{
    public enum NPC_State
    {
        Stop,
        Idle,
        Alert,
        Chase,
        Freeze,
        Return,
        Flee,
    };

    [SerializeField] private EnemyStatusBaseData _baseData;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private SensorController _sensor;

    [Header("Status (Read Only)")]
    [SerializeField, Min(0)] private int _baseMaxHp;
    [SerializeField, Min(0)] private int _maxHp;
    [SerializeField, Min(0)] private int _hp;
    [SerializeField, Min(0)] private int _defRate;

    [SerializeField] private DropTable _dropTable;
    [SerializeField, Min(0)] private int _credit;
    private bool _isBoss;
    private CapsuleCollider _collider;
    public NPC_State _state;
    public int _tactic = 1;
    private Transform _targetTransform;
    private NavMeshAgent _agent;
    private Vector3 _destination;

    private float _backDis;
    private Vector3 _spawnPos;

    // 個体別のランダムウォーク用設定
    [Header("ランダムウォーク設定")]
    [SerializeField] private float _wanderRadius = 5f;
    [SerializeField] private Vector2 _wanderIntervalRange = new Vector2(3f, 7f);
    [SerializeField] private Vector2 _wanderDurationRange = new Vector2(2f, 5f);

    private float _wanderTimer;
    private float _currentWanderInterval;
    private float _wanderStateTimer;
    private float _currentWanderDuration;
    private bool _isWandering;

    // 保険用タイマー
    private float _freezeTimer = 0f;
    [SerializeField] private float _maxFreezeDuration = 4.0f;

    private BuffHandler _buffHandler;

    private void Awake()
    {
        _buffHandler = GetComponent<BuffHandler>();
        if (_buffHandler == null)
        {
            _buffHandler = gameObject.AddComponent<BuffHandler>();
        }

        // 初期化：BaseDataからの読み込みはAwakeで先に行う
        if (_baseData != null)
        {
            _baseMaxHp = _baseData.GetHP();
            _defRate = _baseData.GetDefRate();
            _isBoss = _baseData.GetIsBoss();
            _backDis = _baseData.GetBackDis();
        }
    }

    private void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _collider = gameObject.GetComponent<CapsuleCollider>();
        if (_agent != null)
        {
            _agent.enabled = true;
            _agent.stoppingDistance = 1.5f;
        }

        _spawnPos = transform.position;

        // ★ HPの初期化（初回のみ_baseMaxHpをそのまま現在HPにする）
        float hpMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("MaxHP") : 1.0f;
        _maxHp = Mathf.RoundToInt(_baseMaxHp * hpMult);
        _hp = _maxHp; // 生成時は満タンでセット

        SetState(NPC_State.Idle);
        ResetWanderInterval();
    }

    private void Update()
    {
        // バフ状態を反映（移動速度・最大HPの同期）
        UpdateBuffAppliedStats();

        if (_state == NPC_State.Stop)
        {
            if (_agent != null) _agent.isStopped = true;
            if (_sensor != null && _sensor._animator != null) _sensor._animator.SetBool("Walk", false);
        }
        else if (_state == NPC_State.Freeze)
        {
            if (_agent != null) _agent.isStopped = true;
            if (_sensor != null && _sensor._animator != null) _sensor._animator.SetBool("Walk", false);

            _freezeTimer += Time.deltaTime;
            if (_freezeTimer >= _maxFreezeDuration)
            {
                _freezeTimer = 0f;
                SetState(NPC_State.Chase);
            }
        }
        else if (_state == NPC_State.Idle)
        {
            HandleRandomWander();
        }
        else if (_state == NPC_State.Chase)
        {
            if (_targetTransform == null)
            {
                SetState(NPC_State.Idle);
            }
            else if (_agent != null)
            {
                SetDestination(_targetTransform.position);
                _agent.isStopped = false;
                _agent.SetDestination(GetDestination());
                if (_sensor != null && _sensor._animator != null) _sensor._animator.SetBool("Walk", true);

                if (_backDis <= Mathf.Abs((_spawnPos - transform.position).magnitude))
                {
                    SetState(NPC_State.Return);
                }

                var dir = (GetDestination() - transform.position).normalized;
                dir.y = 0;
                if (dir != Vector3.zero)
                {
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, _agent.angularSpeed * 0.1f * Time.deltaTime);
                }
            }
        }
        else if (_state == NPC_State.Flee)
        {
            if (_targetTransform == null)
            {
                SetState(NPC_State.Idle);
            }
            else if (_agent != null)
            {
                Vector3 fleeDir = (transform.position - _targetTransform.position).normalized;
                Vector3 fleeDestination = transform.position + fleeDir * 3.0f;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(fleeDestination, out hit, 3.0f, NavMesh.AllAreas))
                {
                    _agent.isStopped = false;
                    _agent.SetDestination(hit.position);
                    if (_sensor != null && _sensor._animator != null) _sensor._animator.SetBool("Walk", true);
                }

                var dir = (_targetTransform.position - transform.position).normalized;
                dir.y = 0;
                if (dir != Vector3.zero)
                {
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, _agent.angularSpeed * 0.1f * Time.deltaTime);
                }
            }
        }
        else if (_state == NPC_State.Return)
        {
            if (_agent != null)
            {
                _agent.isStopped = false;
                SetDestination(_spawnPos);
                _agent.SetDestination(GetDestination());
                if (_sensor != null && _sensor._animator != null) _sensor._animator.SetBool("Walk", true);

                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {
                    SetState(NPC_State.Idle);
                }
            }
        }

        if (_hp <= 0)
        {
            if (_collider != null) Destroy(_collider);
            if (_agent != null) _agent.isStopped = true;
            if (_dropTable != null)
            {
                DropItem();
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 移動速度や最大HPなどのバフ効果を毎フレーム同期
    /// </summary>
    private void UpdateBuffAppliedStats()
    {
        if (_baseData == null) return;

        // 移動速度バフ反映
        if (_agent != null)
        {
            float speedMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("MoveSpeed") : 1.0f;
            _agent.speed = _baseData.GetMoveSpeed() * speedMult;
        }

        // 最大HPバフ反映
        float hpMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("MaxHP") : 1.0f;
        _maxHp = Mathf.RoundToInt(_baseMaxHp * hpMult);
        _hp = Mathf.Clamp(_hp, 0, _maxHp);
    }

    /// <summary>
    /// 外部からバフ更新時などに呼び出せるHP同期
    /// </summary>
    public void RefreshHPStatus()
    {
        float hpMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("MaxHP") : 1.0f;
        _maxHp = Mathf.RoundToInt(_baseMaxHp * hpMult);
        _hp = Mathf.Clamp(_hp, 0, _maxHp);
    }

    private void HandleRandomWander()
    {
        if (_agent == null) return;

        if (!_isWandering)
        {
            _wanderTimer += Time.deltaTime;
            _agent.isStopped = true;
            if (_sensor != null && _sensor._animator != null) _sensor._animator.SetBool("Walk", false);

            if (_wanderTimer >= _currentWanderInterval)
            {
                _isWandering = true;
                _wanderTimer = 0f;
                _wanderStateTimer = 0f;
                _currentWanderDuration = Random.Range(_wanderDurationRange.x, _wanderDurationRange.y);

                Vector3 randomDestination = GetRandomNavMeshPoint(_spawnPos, _wanderRadius);
                _agent.isStopped = false;
                _agent.SetDestination(randomDestination);
            }
        }
        else
        {
            _wanderStateTimer += Time.deltaTime;
            bool isWalking = _agent.velocity.sqrMagnitude > 0.1f;
            if (_sensor != null && _sensor._animator != null) _sensor._animator.SetBool("Walk", isWalking);

            if (_wanderStateTimer >= _currentWanderDuration || (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance))
            {
                _isWandering = false;
                ResetWanderInterval();
            }
        }
    }

    private void ResetWanderInterval()
    {
        _currentWanderInterval = Random.Range(_wanderIntervalRange.x, _wanderIntervalRange.y);
        _wanderTimer = 0f;
    }

    private Vector3 GetRandomNavMeshPoint(Vector3 center, float radius)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius + center;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }

    public void SetState(NPC_State tempState, Transform targetObject = null)
    {
        _state = tempState;

        if (tempState != NPC_State.Freeze)
        {
            _freezeTimer = 0f;
        }

        if (_agent == null || !_agent.isOnNavMesh) return;

        if (tempState == NPC_State.Idle)
        {
            _isWandering = false;
            ResetWanderInterval();
        }
        else if (tempState == NPC_State.Chase || tempState == NPC_State.Flee)
        {
            _targetTransform = targetObject;
        }
    }

    public NPC_State GetState()
    {
        return _state;
    }

    public void SetDestination(Vector3 position)
    {
        _destination = position;
    }

    public Vector3 GetDestination()
    {
        return _destination;
    }

    public void AttackStop()
    {
        if (GetState() == NPC_State.Freeze)
        {
            SetState(NPC_State.Chase, _targetTransform);
        }
    }

    public void AddDamage(int damage)
    {
        damage -= damage * (_defRate / 100);

        float takenMult = _buffHandler != null ? _buffHandler.GetDamageTakenMultiplier() : 1.0f;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage * takenMult));

        _hp = Mathf.Max(0, _hp - finalDamage);
    }

    public void PlayerAttack()
    {
        if (_sensor != null) _sensor.PlayerAttack();
    }

    public void DropItem()
    {
        if (_inventory != null)
        {
            _inventory.AddCredit(_credit);
        }

        if (_dropTable == null) return;

        foreach (Item item in _dropTable.GetItemTable())
        {
            float roll = Random.value;
            if (roll < item.GetDropRate())
            {
                Vector3 clonePos = transform.position;
                clonePos.y = 0.75f;

                GameObject spawnedItem = Instantiate(item.GetDropObject(), clonePos, Quaternion.identity);

                ItemController itemController = spawnedItem.GetComponent<ItemController>();
                if (itemController == null)
                {
                    itemController = spawnedItem.AddComponent<ItemController>();
                }
                itemController._data = item;
            }
        }
    }
}