using UnityEngine;

[CreateAssetMenu(fileName = "Satellite", menuName = "My Create Asset / MoveType / Satellite")]
public class SatelliteMove : MoveType
{
    [SerializeField] private float _dropHeight = 10.0f;     // 落下開始の高さ
    [SerializeField] private float _areaRadius = 2.5f;      // スリップダメージエリアの半径
    [SerializeField] private float _areaDuration = 4.0f;    // エリアの持続時間（秒）
    [SerializeField] private float _damageInterval = 0.5f;  // ダメージ発生間隔（秒）
    [SerializeField] private GameObject _areaEffect;       // 継続エリア用エフェクト

    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (Magic == null) return;

        SatelliteBullet satellite = Magic.GetComponent<SatelliteBullet>();
        if (satellite == null)
        {
            satellite = Magic.gameObject.AddComponent<SatelliteBullet>();
            satellite.Init(target, moveSpeed, _dropHeight, _areaRadius, _areaDuration, _damageInterval, _areaEffect);
        }
    }
}

public class SatelliteBullet : MonoBehaviour
{
    private float _speed;
    private float _areaRadius;
    private float _areaDuration;
    private float _damageInterval;
    private GameObject _areaEffect;
    private MagicController _controller;
    private bool _isDropping = true;

    public void Init(Transform target, float speed, float dropHeight, float radius, float duration, float interval, GameObject effect)
    {
        _speed = speed;
        _areaRadius = radius;
        _areaDuration = duration;
        _damageInterval = interval;
        _areaEffect = effect;
        _controller = GetComponent<MagicController>();

        // ターゲットがいる場合はその上空、いなければ前方の地面の上空に配置
        Vector3 targetPos = (target != null) ? target.position : transform.position + transform.forward * 5f;
        transform.position = new Vector3(targetPos.x, targetPos.y + dropHeight, targetPos.z);
    }

    private void Update()
    {
        if (_isDropping)
        {
            // 下方向へ高速落下
            transform.position += Vector3.down * _speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!_isDropping) return;

        // 地面や壁、敵に着弾した瞬間エリア展開へ移行
        if (col.CompareTag("Stage") || col.CompareTag("Wall") || col.CompareTag("Enemy"))
        {
            StartDamageArea();
        }
    }

    private void StartDamageArea()
    {
        _isDropping = false;

        // エリアエフェクト生成（自身の子としてアタッチ）
        if (_areaEffect != null)
        {
            GameObject areaObj = Instantiate(_areaEffect, transform.position, Quaternion.identity);
            areaObj.transform.SetParent(transform);
        }

        // スリップダメージ処理を開始
        StartCoroutine(DoSlipDamage());

        // 指定時間後にエリア消滅
        Destroy(gameObject, _areaDuration);
    }

    private System.Collections.IEnumerator DoSlipDamage()
    {
        float timer = 0f;

        while (timer < _areaDuration)
        {
            // エリア内の敵全員に定期ダメージ
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _areaRadius);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    IDamageble damageObj = hit.GetComponent<IDamageble>();
                    if (damageObj == null) damageObj = hit.GetComponentInParent<IDamageble>();

                    if (damageObj != null && _controller != null)
                    {
                        damageObj.AddDamage(Mathf.Max(1, _controller.TotalAttack));
                    }
                }
            }

            yield return new WaitForSeconds(_damageInterval);
            timer += _damageInterval;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _areaRadius);
    }
}