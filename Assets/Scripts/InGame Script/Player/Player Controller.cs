using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour, IDamageble
{
    [System.Serializable]
    public class MyAttack
    {
        public MagicBaseData _attackData;
        public MagicSpawner _attackInstantiate;
        public float _attackMaxCooltime;
        public float _attackTimer;
        [System.NonSerialized] public float _attackCooltime;
    }

    private Rigidbody _rb;
    private Vector3 _move;
    private Vector3 _moveForward;
    [SerializeField] private int _maxHp;
    private int _hp;
    [SerializeField] private float _maxMoveSpeed;
    private float _moveSpeed;
    [SerializeField] private float _turnTimeRate = 0.5f;
    private CameraController _cameraScript;
    [SerializeField] private Transform _magicParent;
    private GameObject _AttackObject;
    [SerializeField] private Slider _hpUI;

    [SerializeField] private Inventory _inventory;

    [SerializeField] private MyAttack[] _myAttack = new MyAttack[3];

    public MyAttack[] GetMyAttack { get => _myAttack; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }


    private void OnEnable()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _cameraScript = Camera.main.GetComponent<CameraController>();
        _hp = _maxHp;
        _moveSpeed = _maxMoveSpeed;
        _hpUI.maxValue = _maxHp;

        if (_inventory != null)
        {
            for (int c = 0; c < _myAttack.Length; c++)
            {
                if (_inventory.GetAttackData()[c] == null)
                {
                    _myAttack[c]._attackData = null;
                    continue;
                }

                _myAttack[c]._attackData = _inventory.GetAttackData()[c];
                _myAttack[c]._attackMaxCooltime = _myAttack[c]._attackData.GetMagicCoolTime();
                _myAttack[c]._attackInstantiate = _myAttack[c]._attackData.GetInstantiate();
                _myAttack[c]._attackCooltime = 0.0f;
                _myAttack[c]._attackTimer = 0.0f;
            }
        }
    }

    void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        for(int c = 0; c < _myAttack.Length; c++)
        {
            _myAttack[c]._attackTimer += Time.deltaTime;
        }

        _hpUI.value = _hp;

        if (_cameraScript._rock)
        {
            var dir = _cameraScript._rockonTarget.transform.position - this.gameObject.transform.position;
            dir.y = 0.0f;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _turnTimeRate);
            Atack();
        }
        else
        {
            Rotation();
        }
    }
    private void Move()
    {
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        _moveForward = cameraForward * _move.z + Camera.main.transform.right * _move.x;
        _moveForward = _moveForward.normalized;

        if(_move.magnitude > 0)
        {
            _rb.linearVelocity = _moveForward * _moveSpeed * _move.magnitude + new Vector3(0, _rb.linearVelocity.y, 0);
        }
        else
        {
            _rb.linearVelocity = new Vector3(0,_rb.linearVelocity.y, 0);
        }
    }

    private void Rotation()
    {
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        _moveForward = cameraForward * _move.z + Camera.main.transform.right * _move.x;
        _moveForward = _moveForward.normalized;

        if(_move.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _turnTimeRate);
        }
        else
        {
            Quaternion targetRotation = transform.rotation;
            transform.rotation = targetRotation;
        }
    }

    private void Atack()
    {
        for (int c = 0; c < _myAttack.Length; c++)
        {
            if (_myAttack[c]._attackData == null) continue;

            if (_myAttack[c]._attackTimer >= _myAttack[c]._attackMaxCooltime)
            {
                if (_myAttack[c]._attackData.GetMagicParticle() == null) continue;
                _AttackObject = _myAttack[c]._attackData.GetMagicParticle();
                _myAttack[c]._attackTimer = 0;
                _myAttack[c]._attackInstantiate.MagicInstantiate(_AttackObject, transform.position, transform.rotation, _magicParent, _myAttack[c]._attackData.GetMultiShotCount(), _myAttack[c]._attackData.GetShotAngle(), _myAttack[c]._attackData, _cameraScript._rockonTarget.transform);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Item")
        {
            var ItemData = col.GetComponent<ItemController>()._data;
            _inventory.AddInventory(ItemData);
            Destroy(col.gameObject);
        }
    }

    public void AddDamage(int damage)
    {
        _hp -= damage;
    }
}
