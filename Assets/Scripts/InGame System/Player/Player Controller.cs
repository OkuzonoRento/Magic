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

    [Header("Base Settings")]
    [SerializeField] private int _baseMaxHp = 100;
    private int _maxHp;
    private int _hp;
    [SerializeField] private float _maxMoveSpeed = 5f;
    [SerializeField] private float _turnTimeRate = 5.0f;

    [Header("References")]
    private CameraController _cameraScript;
    [SerializeField] private Transform _magicParent;
    [SerializeField] private Slider _hpUI;
    [SerializeField] private Inventory _inventory;

    [Header("Attacks & Targeting")]
    [SerializeField] private MyAttack[] _myAttack = new MyAttack[3];
    [SerializeField] private float _sameMagicInterval = 0.2f;
    private Dictionary<MagicBaseData, float> _lastCastTimes = new Dictionary<MagicBaseData, float>();

    private BuffHandler _buffHandler;

    public MyAttack[] GetMyAttack => _myAttack;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _buffHandler = GetComponent<BuffHandler>();
        if (_buffHandler == null)
        {
            _buffHandler = gameObject.AddComponent<BuffHandler>();
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (Camera.main != null)
        {
            _cameraScript = Camera.main.GetComponent<CameraController>();
        }

        // 初期ステータス計算（HP）
        RefreshHPStatus();

        // インベントリから装備魔法を同期
        InitializeAttacksFromInventory();
    }

    private void Update()
    {
        // 1. バフ状態を反映したステータスの更新
        UpdateBuffAppliedStats();

        // 2. クールタイムタイマーの進行
        for (int c = 0; c < _myAttack.Length; c++)
        {
            if (_myAttack[c]._attackData != null)
            {
                _myAttack[c]._attackTimer += Time.deltaTime;
            }
        }

        // 3. 移動処理
        Move();

        // 4. センサー/カメラでターゲットを捕捉していれば自動攻撃を実行
        if (_cameraScript != null && _cameraScript._rock && _cameraScript._rockonTarget != null)
        {
            Atack();
        }
    }

    private void FixedUpdate()
    {
        // ターゲットを捕捉している時は敵の方向へ向く
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

    /// <summary>
    /// インベントリから魔法データを読み込み、初期化する
    /// </summary>
    public void InitializeAttacksFromInventory()
    {
        if (_inventory == null) return;

        // スロット封印バフが適応されている場合は反映
        if (_buffHandler != null)
        {
            _inventory.ApplyDisabledSlots(_buffHandler);
        }

        var attackDataList = _inventory.GetAttackData();

        for (int c = 0; c < _myAttack.Length; c++)
        {
            if (c < attackDataList.Length && attackDataList[c] != null)
            {
                _myAttack[c]._attackData = attackDataList[c];
                _myAttack[c]._attackInstantiate = _myAttack[c]._attackData.GetInstantiate();
                _myAttack[c]._attackCooltime = 0.0f;

                // 基礎クールタイムを取得
                float baseCooltime = _myAttack[c]._attackData.GetMagicCoolTime();
                float cdMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("Cooldown") : 1.0f;

                // バフ適用後のクールタイム値を Inspector 用変数にセット
                _myAttack[c]._attackMaxCooltime = baseCooltime * cdMult;
                _myAttack[c]._attackTimer = _myAttack[c]._attackMaxCooltime; // 最初から攻撃可能状態にセット
            }
            else
            {
                _myAttack[c]._attackData = null;
                _myAttack[c]._attackInstantiate = null;
                _myAttack[c]._attackMaxCooltime = 0;
                _myAttack[c]._attackTimer = 0;
            }
        }
    }

    /// <summary>
    /// バフを毎フレーム反映し、クールタイム表示などをリアルタイムに同期
    /// </summary>
    private void UpdateBuffAppliedStats()
    {

        float cdMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("Cooldown") : 1.0f;

        for (int c = 0; c < _myAttack.Length; c++)
        {
            if (_myAttack[c]._attackData != null)
            {
                // 基礎データから算出した最大クールタイムをInspector表示・判定用に常時更新
                float baseCooltime = _myAttack[c]._attackData.GetMagicCoolTime();
                _myAttack[c]._attackMaxCooltime = baseCooltime * cdMult;
            }
        }
    }

    /// <summary>
    /// バフ適用済みの最大HPおよび現在のHPの再計算
    /// </summary>
    public void RefreshHPStatus()
    {
        float hpMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("MaxHP") : 1.0f;
        _maxHp = Mathf.RoundToInt(_baseMaxHp * hpMult);
        _hp = Mathf.Clamp(_hp > 0 ? _hp : _maxHp, 0, _maxHp);

        if (_hpUI != null)
        {
            _hpUI.maxValue = _maxHp;
            _hpUI.value = _hp;
        }
    }

    private void Move()
    {
        if (Camera.main == null) return;

        float speedMult = _buffHandler != null ? _buffHandler.GetStatMultiplier("MoveSpeed") : 1.0f;
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

    /// <summary>
    /// 自動ターゲット攻撃処理
    /// </summary>
    private void Atack()
    {
        for (int c = 0; c < _myAttack.Length; c++)
        {
            MagicBaseData currentData = _myAttack[c]._attackData;
            if (currentData == null) continue;

            // クールタイムを満たしていれば発射
            if (_myAttack[c]._attackTimer >= _myAttack[c]._attackMaxCooltime)
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

                // バフによる確率不発の判定
                if (_buffHandler != null && _buffHandler.ShouldFailAction())
                {
                    Debug.Log("バフ効果により魔法が不発になりました。");
                    continue;
                }

                GameObject attackObject = currentData.GetMagicParticle();
                if (attackObject == null || _myAttack[c]._attackInstantiate == null) continue;

                // バフによる自傷ダメージ処理
                if (_buffHandler != null)
                {
                    _buffHandler.TriggerSelfDamage(_hp, _maxHp);
                }

                // センサー/カメラで取得されたターゲットを取得
                Transform targetTransform = (_cameraScript != null && _cameraScript._rockonTarget != null)
                    ? _cameraScript._rockonTarget.transform
                    : null;

                // 魔法の生成実行
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
        float takenMult = _buffHandler != null ? _buffHandler.GetDamageTakenMultiplier() : 1.0f;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage * takenMult));

        _hp = Mathf.Max(0, _hp - finalDamage);
    }
}