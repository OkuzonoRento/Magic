using UnityEngine;

public class ItemController : MonoBehaviour
{
    [Header("アニメーション設定")]
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private float _rotationSpeed = 100.0f;
    [SerializeField] private float _amplitude = 0.2f;
    [SerializeField] private Transform _model;

    [Header("引き寄せ設定")]
    [SerializeField] private float _attractRadius = 5.0f; // 吸収が始まる距離
    [SerializeField] private float _initialSpeed = 4.0f;  // 吸収の初速
    [SerializeField] private float _accelerate = 8.0f;    // 吸収の加速度

    [Header("ドロップ放物線設定（Rigidbodyなし）")]
    [SerializeField] private float _dropDuration = 0.5f;  // ドロップしてから着地するまでの時間
    [SerializeField] private float _popHeight = 1.0f;     // 跳ね上がる高さ

    public Item _data;

    private Transform _playerTransform;
    private Collider _collider;

    private bool _hasLanded = false;       // 着地したか
    private bool _isAttracting = false;    // 吸い寄せ中か
    private float _currentMoveSpeed;
    private Vector3 _startModelLocalPos;

    // ドロップアニメーション用変数
    private Vector3 _spawnPos;
    private Vector3 _landPos;
    private float _dropTimer = 0f;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _currentMoveSpeed = _initialSpeed;

        if (_model == null)
        {
            _model = transform;
        }
        _startModelLocalPos = _model.localPosition;

        // Playerタグからプレイヤーを取得
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        // --- ドロップ時の着地目標座標をレイキャストで計算 ---
        _spawnPos = transform.position;
        Vector3 randomOffset = new Vector3(Random.Range(-0.8f, 0.8f), 0, Random.Range(-0.8f, 0.8f));
        Vector3 targetCheckPos = _spawnPos + randomOffset;

        // 下方向にレイを飛ばして地面のY座標を取得
        if (Physics.Raycast(targetCheckPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
        {
            _landPos = hit.point;
        }
        else
        {
            _landPos = targetCheckPos; // 地面がない場合のフォールバック
        }

        // 判定用コライダーをTriggerにしておく（着地前でもプレイヤーが拾えるように）
        if (_collider != null)
        {
            _collider.isTrigger = true;
        }
    }

    private void Update()
    {
        // 1. 吸い寄せ中
        if (_isAttracting)
        {
            if (_playerTransform == null) return;

            _currentMoveSpeed += _accelerate * Time.deltaTime;
            Vector3 targetPos = _playerTransform.position + Vector3.up * 0.8f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _currentMoveSpeed * Time.deltaTime);

            // 吸い込まれ中の回転
            _model.Rotate(Vector3.up, _rotationSpeed * 2f * Time.deltaTime, Space.World);
            return;
        }

        // 2. 着地前（ぽろっと落ちる放物線移動）
        if (!_hasLanded)
        {
            _dropTimer += Time.deltaTime;
            float t = _dropTimer / _dropDuration;

            if (t >= 1.0f)
            {
                _hasLanded = true;
                transform.position = _landPos;
            }
            else
            {
                // XZ平面の補間移動 ＋ Y軸の放物線（山なり）計算
                Vector3 currentXZ = Vector3.Lerp(_spawnPos, _landPos, t);
                float height = Mathf.Sin(t * Mathf.PI) * _popHeight;
                transform.position = new Vector3(currentXZ.x, Mathf.Lerp(_spawnPos.y, _landPos.y, t) + height, currentXZ.z);
            }
        }
        // 3. 着地後（元の浮遊・回転アニメーション）
        else
        {
            float targetPosY = Mathf.Sin(Time.time * _speed) * _amplitude;
            _model.localPosition = _startModelLocalPos + new Vector3(0, targetPosY, 0);
            _model.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
        }

        // --- 吸い寄せ開始チェック ---
        if (_playerTransform != null && !_isAttracting)
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            if (distance <= _attractRadius)
            {
                _isAttracting = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attractRadius);
    }
}