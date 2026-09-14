using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private int _maxHp = 100;
    private int _hp;
    [SerializeField] private float _maxMoveSpeed = 5f;
    private float _moveSpeed;
    [SerializeField] private float _turnTimeRate = 5.0f;
    private CameraController _cameraScript;
    [SerializeField] private Transform _magicParent;
    [SerializeField] private Slider _hpUI;

    [SerializeField] private Inventory _inventory;
    [SerializeField] private MyAttack[] _myAttack = new MyAttack[3];

    // 同一魔法が連射されるのを防ぐための最小インターバル（秒）
    [SerializeField] private float _sameMagicInterval = 0.2f;

    // 同一魔法の最終発射時刻を記録する辞書
    private Dictionary<MagicBaseData, float> _lastCastTimes = new Dictionary<MagicBaseData, float>();

    public MyAttack[] GetMyAttack => _myAttack;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (Camera.main != null)
        {
            _cameraScript = Camera.main.GetComponent<CameraController>();
        }

        _hp = _maxHp;
        _moveSpeed = _maxMoveSpeed;

        if (_hpUI != null)
        {
            _hpUI.maxValue = _maxHp;
            _hpUI.value = _hp;
        }

        InitializeAttacksFromInventory();
    }

    private void Update()
    {
        // 攻撃タイマーの更新（DeltaTimeで正確にカウント）
        for (int c = 0; c < _myAttack.Length; c++)
        {
            if (_myAttack[c]._attackData != null)
            {
                _myAttack[c]._attackTimer += Time.deltaTime;
            }
        }

        Move();

        // 攻撃判定
        if (_cameraScript != null && _cameraScript._rock)
        {
            Atack();
        }
    }

    private void FixedUpdate()
    {
        // 物理回転処理
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

        // HPバー更新
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
                _myAttack[c]._attackTimer = _myAttack[c]._attackMaxCooltime; // 初回即時発射可能に設定
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

        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        _moveForward = cameraForward * _move.z + Camera.main.transform.right * _move.x;
        _moveForward = _moveForward.normalized;

        if (_move.magnitude > 0)
        {
            _rb.linearVelocity = _moveForward * _moveSpeed * _move.magnitude + new Vector3(0, _rb.linearVelocity.y, 0);
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
            MagicBaseData currentData = _myAttack[c]._attackData;
            if (currentData == null) continue;

            // 1. 各スロットの個別クールタイムチェック
            if (_myAttack[c]._attackTimer >= _myAttack[c]._attackMaxCooltime)
            {
                // 2. 同一魔法の連続/同時発射防止チェック
                if (_lastCastTimes.TryGetValue(currentData, out float lastCastTime))
                {
                    if (Time.time - lastCastTime < _sameMagicInterval)
                    {
                        // 前の同一魔法から時間が経っていないため発射を見送る
                        continue;
                    }
                }

                GameObject attackObject = currentData.GetMagicParticle();
                if (attackObject == null || _myAttack[c]._attackInstantiate == null) continue;

                // タイマーのリセットと最終発射時刻の記録
                _myAttack[c]._attackTimer = 0f;
                _lastCastTimes[currentData] = Time.time;

                Transform targetTransform = (_cameraScript != null && _cameraScript._rockonTarget != null)
                    ? _cameraScript._rockonTarget.transform
                    : null;

                // 魔法の生成
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
        _hp = Mathf.Max(0, _hp - damage);
    }
}