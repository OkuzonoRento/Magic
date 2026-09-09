using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SensorController : MonoBehaviour
{
    [SerializeField] private EnemyStatusBaseData _baseData;

    [Header("回転の設定")]
    [SerializeField, Tooltip("プレイヤーの方向を向く速度")] private float _turnSpeed = 5.0f;

    private SphereCollider _searchArea = default;
    private float _searchAngle;
    private float _proximityRadius;

    private NPCController _NPC_controller;
    private float _attackCoolTimer = 0f;
    public Animator _animator;

    private Transform _playerTransform;
    private IDamageble _playerDamageble;

    // 現在選択されている攻撃パターン
    private AttackPattern _currentAttackPattern;
    private bool _hasSelectedPattern = false;

    // コンボ制御用
    private int _targetComboCount = 1;
    private int _currentComboIndex = 0;
    private bool _isChasingForCombo = false; // コンボ中の歩み寄りフラグ

    private void Start()
    {
        _NPC_controller = transform.parent.GetComponent<NPCController>();

        _searchArea = gameObject.GetComponent<SphereCollider>();

        _searchAngle = _baseData.GetSearchAngle();
        _searchArea.radius = _baseData.GetSearchRadius();
        _proximityRadius = _baseData.GetProximityRadius();

        _animator = transform.parent?.GetComponent<Animator>();
        _attackCoolTimer = _baseData.GetAttackCoolTime();
    }

    private void Update()
    {
        if (_attackCoolTimer < _baseData.GetAttackCoolTime())
        {
            _attackCoolTimer += Time.deltaTime;
        }

        // ★ コンボ追尾中の移動チェック（プレイヤーに近づいたら次の攻撃を発動）
        if (_isChasingForCombo && _playerTransform != null)
        {
            float dis = Vector3.Distance(transform.position, _playerTransform.position);

            // 攻撃判定距離内に到達したら歩みを止めて次の攻撃を開始
            if (dis <= _currentAttackPattern.attackRadius)
            {
                _isChasingForCombo = false;
                _animator.SetBool("Walk", false);
                _animator.SetTrigger(_currentAttackPattern.triggerName);
                _NPC_controller.SetState(NPCController.NPC_State.Freeze);
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && _searchArea != null)
        {
            _playerTransform = col.transform;
            _playerDamageble = col.gameObject.GetComponent<IDamageble>();

            Vector3 playerDirection = col.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);
            float dis = Vector3.Distance(col.transform.position, transform.position);

            float effectiveAngle = _searchAngle * 0.5f;

            bool isDetected = _baseData.GetIsBoss() || (dis <= _proximityRadius) || (angle <= effectiveAngle);

            if (isDetected)
            {
                // コンボ移動中やFreeze中でなければ通常の判断を行う
                if (_NPC_controller.GetState() != NPCController.NPC_State.Freeze && !_isChasingForCombo)
                {
                    LookAtPlayer(col.transform.position);

                    // 1. クールタイムが明けている場合
                    if (_attackCoolTimer >= _baseData.GetAttackCoolTime())
                    {
                        if (TrySelectAttackPattern(dis, out AttackPattern selectedPattern))
                        {
                            _currentAttackPattern = selectedPattern;
                            _hasSelectedPattern = true;

                            // コンボ回数の決定
                            _currentComboIndex = 0;
                            Vector2Int comboRange = new Vector2Int(1, 3);
                            _targetComboCount = Random.Range(comboRange.x, comboRange.y + 1);

                            _attackCoolTimer = 0f;
                            _animator.SetBool("Walk", false);
                            _animator.SetTrigger(_currentAttackPattern.triggerName);
                            _NPC_controller.SetState(NPCController.NPC_State.Freeze);
                        }
                        else
                        {
                            _NPC_controller.SetState(NPCController.NPC_State.Chase, col.transform);
                        }
                    }
                    // 2. クールタイム中の場合
                    else
                    {
                        float halfSearchRadius = _searchArea.radius * 0.5f;
                        if (dis <= halfSearchRadius && !_baseData.GetIsBoss())
                        {
                            _NPC_controller.SetState(NPCController.NPC_State.Flee, col.transform);
                        }
                        else
                        {
                            _NPC_controller.SetState(NPCController.NPC_State.Stop);
                        }
                    }
                }
            }
        }
    }

    private bool TrySelectAttackPattern(float distance, out AttackPattern result)
    {
        List<AttackPattern> availablePatterns = new List<AttackPattern>();

        foreach (var pattern in _baseData.GetAttackPatterns())
        {
            if (distance >= pattern.minDistance && distance <= pattern.maxDistance)
            {
                availablePatterns.Add(pattern);
            }
        }

        if (availablePatterns.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePatterns.Count);
            result = availablePatterns[randomIndex];
            return true;
        }

        result = default;
        return false;
    }

    private void LookAtPlayer(Vector3 targetPosition)
    {
        Transform parentTransform = transform.parent;
        if (parentTransform == null) return;

        Vector3 direction = (targetPosition - parentTransform.position);
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, targetRotation, Time.deltaTime * _turnSpeed);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _playerTransform = null;
            _playerDamageble = null;
            _isChasingForCombo = false;
            if (_NPC_controller.GetState() != NPCController.NPC_State.Freeze)
            {
                _NPC_controller.SetState(NPCController.NPC_State.Return);
            }
        }
    }

    // アニメーションイベントから実行される攻撃ヒット＆コンボ判定
    public void PlayerAttack()
    {
        if (!_hasSelectedPattern) return;

        // --- 1. ダメージ判定 ---
        if (_playerTransform != null && _playerDamageble != null)
        {
            Vector3 dirToPlayer = (_playerTransform.position - transform.position);
            dirToPlayer.y = 0;

            float distance = dirToPlayer.magnitude;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (angle <= _currentAttackPattern.attackAngle && distance <= _currentAttackPattern.attackRadius)
            {
                _playerDamageble.AddDamage(_currentAttackPattern.damage);
            }
        }

        _currentComboIndex++;

        // --- 2. 複数回攻撃（コンボ）判定 ---
        if (_currentComboIndex < _targetComboCount && _playerTransform != null)
        {
            float currentDistance = Vector3.Distance(transform.position, _playerTransform.position);

            // 索敵半径内にターゲットが残っている場合
            if (currentDistance <= _baseData.GetSearchRadius())
            {
                // A. すでに攻撃届く距離にいるなら、連続で次の攻撃を実行
                if (currentDistance <= _currentAttackPattern.attackRadius)
                {
                    LookAtPlayer(_playerTransform.position);
                    _animator.SetTrigger(_currentAttackPattern.triggerName);
                    return;
                }
                // B. 距離が離れているなら、歩いて近づいてから攻撃（Chase状態へ移行）
                else
                {
                    _isChasingForCombo = true;
                    _NPC_controller.SetState(NPCController.NPC_State.Chase, _playerTransform);
                    return;
                }
            }
        }

        // コンボ終了処理
        _hasSelectedPattern = false;
        _isChasingForCombo = false;
        _currentComboIndex = 0;
        if (_NPC_controller != null)
        {
            _NPC_controller.AttackStop();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_baseData == null || _searchArea == null) return;

        float effectiveAngle = _baseData.GetSearchAngle() * 0.5f;
        float searchRadius = _baseData.GetSearchRadius();
        Vector3 searchFromDir = Quaternion.Euler(0.0f, -effectiveAngle, 0.0f) * transform.forward;

        Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.05f);
        Handles.DrawSolidArc(transform.position, Vector3.up, searchFromDir, effectiveAngle * 2.0f, searchRadius);

        if (_baseData.GetAttackPatterns() != null)
        {
            foreach (var pattern in _baseData.GetAttackPatterns())
            {
                Vector3 attackFromDir = Quaternion.Euler(0.0f, -pattern.attackAngle, 0.0f) * transform.forward;
                Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
                Handles.DrawWireArc(transform.position, Vector3.up, attackFromDir, pattern.attackAngle * 2.0f, pattern.attackRadius);
            }
        }
    }
#endif
}