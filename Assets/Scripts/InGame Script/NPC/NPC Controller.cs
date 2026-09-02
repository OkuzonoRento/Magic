using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour, IDamageble
{
    public enum NPC_State   //ìGÇÃèÛë‘
    {
        Stop,   //  í‚é~
        Idle,   //  ë“ã@
        Alert,  //  åxâ˙
        Chase,  //  í«ê’
        Freeze, //Å@çdíº
        Return, //  ñﬂÇÈ
    };

    [SerializeField] private EnemyStatusBaseData _baseData;
    [SerializeField] private Inventory _inventory;
    [SerializeField,Min(0)] private int _hp;
    [SerializeField,Min(0)] private int _defRate;
    [SerializeField] private DropTable _dropTable;
    [SerializeField,Min(0)] private int _credit;
    private bool _isBoss;
    private CapsuleCollider _collider;
    public NPC_State _state;
    public int _tactic = 1;
    private Transform _targetTransform;
    private NavMeshAgent _agent;
    private Vector3 _destination;

    //private bool _attack = false;
    private int _attackCount = 1;
    private float _backDis;
    private Vector3 _spawnPos;


    private void Start()
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _collider = gameObject.GetComponent<CapsuleCollider>();
        _agent.speed = _baseData.GetMoveSpeed();
        _agent.enabled = true;
        SetState(NPC_State.Idle);
        _spawnPos = transform.position;
        _backDis = _baseData.GetBackDis();
        _hp = _baseData.GetHP();
        _defRate = _baseData.GetDefRate();
        _isBoss = _baseData.GetIsBoss();
    }

    private void Update()
    {
        if (_state == NPC_State.Stop)
        {
            _agent.isStopped = true;
        }
        else if (_state == NPC_State.Idle)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
        else if (_state == NPC_State.Chase)
        {
            if (_targetTransform == null)
            {
                SetState(NPC_State.Idle);
            }
            else
            {
                SetDestination(_targetTransform.position);
                _agent.SetDestination(GetDestination());

                if (_backDis <= Mathf.Abs((_spawnPos - transform.position).magnitude))
                {
                    SetState(NPCController.NPC_State.Return);
                }
            }

            var dir = (GetDestination() - transform.position).normalized;
            dir.y = 0;
            Quaternion setRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, _agent.angularSpeed * 0.1f * Time.deltaTime);
        }
        else if (_state == NPC_State.Return)
        {
            SetDestination(_spawnPos);
            _agent.SetDestination(GetDestination());

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                SetState(NPC_State.Idle);
            }
        }

        if(_hp <= 0)
        {
            Destroy(_collider);
            _agent.isStopped = true;
            if (_dropTable != null)
            {
                DropItem();
            }
            Destroy(gameObject);

            if(_isBoss)
            {

            }
        }
    }

    public void SetState(NPC_State tempState, Transform targetObject = null)
    {
        _state = tempState;

        if (_agent == null || !_agent.isOnNavMesh)
        {
            return;
        }

        if (tempState == NPC_State.Idle)
        {
            _agent.isStopped = true;
        }
        else if (tempState == NPC_State.Chase)
        {
            _targetTransform = targetObject;
            _agent.isStopped = false;
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
        //_attack = false;

        if(GetState() == NPC_State.Freeze)
        {
            SetState(NPC_State.Chase);
        }
        else
        {
            float percent = 0.0f;

            switch (_tactic)
            {
                case 1:
                    percent = 100.0f;
                    break;

                    case 2:
                    if(_attackCount == 1)
                    {
                    percent = 50.0f;
                    }
                    else if(_attackCount == 2)
                    {
                        percent = 100.0f;
                    }
                    break;

                    case 3:
                    if (_attackCount == 1)
                    {
                        percent = 30.0f;
                    }
                    else if(_attackCount == 2)
                    {
                        percent = 70.0f;
                    }
                    else if(_attackCount == 3)
                    {
                        percent = 100.0f;
                    }
                    break;
            }

            if(Probability(percent))
            {
                SetState(NPC_State.Freeze);
                _attackCount = 1;
            }
            else
            {
                SetState(NPC_State.Chase);
                _attackCount++;
            }

        }
    }

    public void AddDamage(int damage)
    {
        damage -= damage * (_defRate / 100);
        if(damage < 1)
        {
            damage = 1;
        }
        _hp -= damage;
    }

    public static bool Probability(float fPercent)
    {
        float fProbabilityRate = UnityEngine.Random.value * 100.0f;

        if(fPercent == 100.0f &&  fProbabilityRate == fPercent)
        {
            return true;
        }
        else if(fProbabilityRate < fPercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropItem()
    {
        _inventory.AddCredit(_credit);
        foreach (Item item in _dropTable.GetItemTable())
        {
            float roll = Random.value;

            if(roll < item.GetDropRate())
            {
                Vector3 clonePos = transform.position;
                clonePos.y = 0.75f;

                Instantiate(item.GetDropObject(), clonePos, Quaternion.identity);
            }
        }
    }
}
