using UnityEditor;
using UnityEngine;

public class SensorController : MonoBehaviour
{
    [SerializeField] private EnemyStatusBaseData _baseData;

    [Header("回転の設定")]
    [SerializeField, Tooltip("プレイヤーの方向を向く速度")] private float _turnSpeed = 5.0f;

    private SphereCollider _searchArea = default;
    private float _searchAngle;
    private float _attackRadius;
    private float _attackAngle;
    private Vector2Int _comboCountRange;
    private float _proximityRadius;

    private NPCController _NPC_controller;
    private float _attackCoolTimer = 0f;
    public Animator _animator;

    private Transform _playerTransform;
    private IDamageble _playerDamageble;

    // コンボ制御用
    private int _targetComboCount = 1;
    private int _currentComboIndex = 0;

    private void Start()
    {
        _NPC_controller = transform.parent.GetComponent<NPCController>();
        _searchArea = gameObject.GetComponent<SphereCollider>();

        // DB (EnemyStatusBaseData) から初期化
        _searchAngle = _baseData.GetSearchAngle();
        _searchArea.radius = _baseData.GetSearchRadius();
        _attackRadius = _baseData.GetAttackRadius();
        _attackAngle = _baseData.GetAttackAngle();
        _comboCountRange = _baseData.GetComboCountRange();
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

            // 全方位近接感知 または 扇形視野角内
            bool isDetected = (dis <= _proximityRadius) || (angle <= effectiveAngle);

            if (isDetected)
            {
                if (_NPC_controller.GetState() != NPCController.NPC_State.Freeze)
                {
                    // 感知エリアに入ったらまずプレイヤーの方向を向く
                    LookAtPlayer(col.transform.position);

                    // 1. クールタイムが明けている場合
                    if (_attackCoolTimer >= _baseData.GetAttackCoolTime())
                    {
                        if (dis <= _attackRadius)
                        {
                            _attackCoolTimer = 0f;
                            _currentComboIndex = 0;
                            _targetComboCount = Random.Range(_comboCountRange.x, _comboCountRange.y + 1);

                            _animator.SetBool("Walk", false);
                            _animator.SetTrigger("Attack");
                            _NPC_controller.SetState(NPCController.NPC_State.Freeze);
                        }
                        else
                        {
                            _NPC_controller.SetState(NPCController.NPC_State.Chase, col.transform);
                        }
                    }
                    // 2. クールタイム中の場合（視野距離の半分以内に入っていれば逃げる）
                    else
                    {
                        float halfSearchRadius = _searchArea.radius * 0.5f;

                        if (dis <= halfSearchRadius)
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

    /// <summary>
    /// ターゲット（プレイヤー）の方向へ親オブジェクト（NPC本体）を回転させる
    /// </summary>
    private void LookAtPlayer(Vector3 targetPosition)
    {
        Transform parentTransform = transform.parent;
        if (parentTransform == null) return;

        Vector3 direction = (targetPosition - parentTransform.position);
        direction.y = 0; // 上下の傾きを防ぐためY軸を固定

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
            if (_NPC_controller.GetState() != NPCController.NPC_State.Freeze)
            {
                _NPC_controller.SetState(NPCController.NPC_State.Return);
            }
        }
    }

    public void PlayerAttack()
    {
        if (_playerTransform != null && _playerDamageble != null)
        {
            Vector3 dirToPlayer = (_playerTransform.position - transform.position);
            dirToPlayer.y = 0;

            float distance = dirToPlayer.magnitude;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (angle <= _attackAngle && distance <= _attackRadius)
            {
                _playerDamageble.AddDamage(_baseData.GetAttack());
            }
        }

        _currentComboIndex++;

        if (_currentComboIndex < _targetComboCount)
        {
            if (_playerTransform != null)
            {
                float currentDistance = Vector3.Distance(transform.position, _playerTransform.position);

                // 距離が離れすぎたらコンボ中断
                if (currentDistance > _attackRadius * 1.3f)
                {
                    _currentComboIndex = 0;
                    _NPC_controller.AttackStop();
                    return;
                }
            }

            _animator.SetTrigger("Attack");
        }
        else
        {
            if (_NPC_controller != null)
            {
                _NPC_controller.AttackStop();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_baseData == null || _searchArea == null) return;

        float attackRadius = _baseData.GetAttackRadius();
        float attackAngle = _baseData.GetAttackAngle();
        float proximityRadius = _baseData.GetProximityRadius();

        // 1. 全方位近接感知エリア（緑色・円形）
        Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.05f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, 360.0f, proximityRadius);
        Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, 360.0f, proximityRadius);

        // 2. 扇形視野角（緑色）
        float effectiveAngle = _baseData.GetSearchAngle() * 0.5f;
        float searchRadius = _baseData.GetSearchRadius();
        Vector3 searchFromDir = Quaternion.Euler(0.0f, -effectiveAngle, 0.0f) * transform.forward;

        Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.05f);
        Handles.DrawSolidArc(transform.position, Vector3.up, searchFromDir, effectiveAngle * 2.0f, searchRadius);
        Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        Handles.DrawWireArc(transform.position, Vector3.up, searchFromDir, effectiveAngle * 2.0f, searchRadius);

        // 3. 逃走境界線（青）: 視野距離の半分
        Handles.color = new Color(0.0f, 0.5f, 1.0f, 0.4f);
        Handles.DrawWireArc(transform.position, Vector3.up, searchFromDir, effectiveAngle * 2.0f, searchRadius * 0.5f);

        // 4. 攻撃エリア（赤）
        Vector3 attackFromDir = Quaternion.Euler(0.0f, -attackAngle, 0.0f) * transform.forward;

        Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.08f);
        Handles.DrawSolidArc(transform.position, Vector3.up, attackFromDir, attackAngle * 2.0f, attackRadius);
        Handles.color = new Color(1.0f, 0.2f, 0.2f, 0.8f);
        Handles.DrawWireArc(transform.position, Vector3.up, attackFromDir, attackAngle * 2.0f, attackRadius);
    }
#endif
}