using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using BuffSystem.Core;

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
    [SerializeField] private int _baseMaxHp = 100;
    private int _maxHp;
    private int _hp;
    [SerializeField] private float _maxMoveSpeed = 5f;
    [SerializeField] private float _turnTimeRate = 5.0f;
    private CameraController _cameraScript;
    [SerializeField] private Transform _magicParent;
    [SerializeField] private Slider _hpUI;

    [SerializeField] private Inventory _inventory;
    [SerializeField] private MyAttack[] _myAttack = new MyAttack[3];

    [SerializeField] private float _sameMagicInterval = 0.2f;
    private Dictionary<MagicBaseData, float> _lastCastTimes = new Dictionary<MagicBaseData, float>();

    private BuffHandler _buffHandler;

    public MyAttack[] GetMyAttack => _myAttack;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _buffHandler = GetComponent<BuffHandler>();
        if (_buffHandler == null) _buffHandler = gameObject.AddComponent<BuffHandler>();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (Camera.main != null)
        {
            _cameraScript = Camera.main.GetComponent<CameraController>();
        }

        // 最大HPの補正
        float hpMult = _buffHandler.GetStatMultiplier("MaxHP");
        _maxHp = Mathf.RoundToInt(_baseMaxHp * hpMult);
        _hp = _maxHp;

        if (_hpUI != null)
        {
            _hpUI.maxValue = _maxHp;
            _hpUI.value = _hp;
        }

        InitializeAttacksFromInventory();
    }

    private void Update()
    {
        for (int c = 0; c < _myAttack.Length; c++)
        {
            if (_myAttack[c]._attackData != null)
            {
                _myAttack[c]._attackTimer += Time.deltaTime;
            }
        }

        Move();

        if (_cameraScript != null && _cameraScript._rock)
        {
            Atack();
        }
    }

    private void FixedUpdate()
    {
        if (_cameraScript != null && _cameraScript._rock && _cameraScript._rockonTarget != null)
        {
            Vector3 dir = _cameraScript._rockonTarget.transform.position - transform.position;
            dir.y = 0.0f;
            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _turnTimeRate);
            }
        }
        else
        {
            Rotation();
        }

        if (_hpUI != null)
        {
            _hpUI.value = _hp;
        }
    }

    private void InitializeAttacksFromInventory()
    {
        if (_inventory == null) return;

        var attackDataList = _inventory.GetAttackData();
        for (int c = 0; c < _myAttack.Length; c++)
        {
            if (c < attackDataList.Count() && attackDataList[c] != null)
            {
                _myAttack[c]._attackData = attackDataList[c];
                _myAttack[c]._attackMaxCooltime = _myAttack[c]._attackData.GetMagicCoolTime();
                _myAttack[c]._attackInstantiate = _myAttack[c]._attackData.GetInstantiate();
                _myAttack[c]._attackCooltime = 0.0f;
                _myAttack[c]._attackTimer = _myAttack[c]._attackMaxCooltime;
            }
            else
            {
                _myAttack[c]._attackData = null;
            }
        }
    }

    private void Move()
    {
        if (Camera.main == null) return;

        float speedMult = _buffHandler.GetStatMultiplier("MoveSpeed");
        float currentSpeed = _maxMoveSpeed * speedMult;

        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        _moveForward = cameraForward * _move.z + Camera.main.transform.right * _move.x;
        _moveForward = _moveForward.normalized;

        if (_move.magnitude > 0)
        {
            _rb.linearVelocity = _moveForward * currentSpeed * _move.magnitude + new Vector3(0, _rb.linearVelocity.y, 0);
        }
        else
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
        }
    }

    private void Rotation()
    {
        if (_move.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _turnTimeRate);
        }
    }

    private void Atack()
    {
        for (int c = 0; c < _myAttack.Length; c++)
        {
            // データが null の場合は発射しない（ショップで代入された null で自動回避）
            MagicBaseData currentData = _myAttack[c]._attackData;
            if (currentData == null) continue;

            float cdMult = _buffHandler.GetStatMultiplier("Cooldown");
            float finalCooltime = _myAttack[c]._attackMaxCooltime * cdMult;

            if (_myAttack[c]._attackTimer >= finalCooltime)
            {
                if (_lastCastTimes.TryGetValue(currentData, out float lastCastTime))
                {
                    if (Time.time - lastCastTime < _sameMagicInterval)
                    {
                        continue;
                    }
                }

                _myAttack[c]._attackTimer = 0f;
                _lastCastTimes[currentData] = Time.time;

                // 確率不発の判定
                if (_buffHandler.ShouldFailAction())
                {
                    Debug.Log("魔法が不発に終わりました。");
                    continue;
                }

                GameObject attackObject = currentData.GetMagicParticle();
                if (attackObject == null || _myAttack[c]._attackInstantiate == null) continue;

                // 自傷処理
                _buffHandler.TriggerSelfDamage(_hp, _maxHp);

                Transform targetTransform = (_cameraScript != null && _cameraScript._rockonTarget != null)
                    ? _cameraScript._rockonTarget.transform
                    : null;

                _myAttack[c]._attackInstantiate.MagicInstantiate(
                    attackObject,
                    transform.position,
                    transform.rotation,
                    _magicParent,
                    currentData.GetMultiShotCount(),
                    currentData.GetShotAngle(),
                    currentData,
                    targetTransform
                );
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _move = new Vector3(input.x, 0, input.y);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Item"))
        {
            ItemController item = col.GetComponent<ItemController>();
            if (item != null)
            {
                _inventory.AddInventory(item._data);
                Destroy(col.gameObject);
            }
        }
    }

    public void AddDamage(int damage)
    {
        float takenMult = _buffHandler.GetDamageTakenMultiplier();
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage * takenMult));

        _hp = Mathf.Max(0, _hp - finalDamage);
    }
}