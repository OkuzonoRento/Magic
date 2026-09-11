using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser", menuName = "My Create Asset / MoveType / PiercingLaser")]
public class PiercingLaserMove : MoveType
{
    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (Magic == null) return;

        PiercingLaserBullet laser = Magic.GetComponent<PiercingLaserBullet>();
        if (laser == null)
        {
            laser = Magic.gameObject.AddComponent<PiercingLaserBullet>();
            laser.Init(moveSpeed, rb);
        }
    }
}

// レーザー（貫通弾）の挙動を管理するコンポーネント
public class PiercingLaserBullet : MonoBehaviour
{
    [Header("レーザー設定")]
    [SerializeField] private float _lifeTime = 3.0f; // 弾が消滅するまでの時間（秒）
    [SerializeField] private int _maxPenetrateCount = 99; // 最大貫通敵数（実質無限）

    private float _speed;
    private Rigidbody _rb;
    private Vector3 _moveDirection;
    private int _hitCount = 0;
    private MagicController _controller;

    // 一度ダメージを与えた敵を記録（重複ヒット防止）
    private HashSet<int> _hitEnemyInstanceIds = new HashSet<int>();

    public void Init(float speed, Rigidbody rb)
    {
        _speed = speed;
        _rb = rb;
        _moveDirection = transform.forward;
        _controller = GetComponent<MagicController>();

        // 一定時間後に自動消滅
        Destroy(gameObject, _lifeTime);
    }

    private void FixedUpdate()
    {
        if (_rb != null)
        {
            // 直線上にそのまま高速移動（壁も敵も無視して貫通）
            _rb.MovePosition(_rb.position + _moveDirection * _speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 敵に当たった場合
        if (other.CompareTag("Enemy"))
        {
            int instanceId = other.gameObject.GetInstanceID();

            // まだダメージを与えていない敵の場合のみ処理
            if (!_hitEnemyInstanceIds.Contains(instanceId))
            {
                _hitEnemyInstanceIds.Add(instanceId);

                // 子要素のコライダーに当たった場合も考慮して親要素まで判定を探す
                IDamageble damageable = other.GetComponent<IDamageble>();
                if (damageable == null)
                {
                    damageable = other.GetComponentInParent<IDamageble>();
                }

                if (damageable != null && _controller != null)
                {
                    // ヒット時スパークエフェクト生成
                    if (_controller.Spark != null)
                    {
                        Instantiate(_controller.Spark, transform.position, Quaternion.identity);
                    }

                    // ダメージ計算（0以下の場合は最低1ダメを通す安全策）
                    int finalDamage = Mathf.Max(1, _controller.TotalAttack);
                    damageable.AddDamage(finalDamage);
                }

                _hitCount++;

                // 貫通上限数に達した場合は消滅
                if (_hitCount >= _maxPenetrateCount)
                {
                    Destroy(gameObject);
                }
            }
        }
        // 壁や障害物も貫通する場合は何も処理せずそのまま通り抜けます
    }
}