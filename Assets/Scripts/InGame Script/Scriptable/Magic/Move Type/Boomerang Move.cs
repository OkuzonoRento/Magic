using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boomerang", menuName = "My Create Asset / MoveType / Boomerang")]
public class BoomerangMove : MoveType
{
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _returnSpeedMultiplier = 1.5f;

    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (Magic == null) return;

        BoomerangBullet boomerang = Magic.GetComponent<BoomerangBullet>();
        if (boomerang == null)
        {
            boomerang = Magic.gameObject.AddComponent<BoomerangBullet>();
            boomerang.Init(moveSpeed, _maxDistance, _returnSpeedMultiplier, rb);
        }
    }
}

public class BoomerangBullet : MonoBehaviour
{
    private float _speed;
    private float _maxDist;
    private float _returnMult;
    private Rigidbody _rb;
    private Vector3 _startPos;
    private Transform _playerTransform;
    private bool _isReturning = false;

    private MagicController _controller;
    private HashSet<int> _hitEnemyInstanceIds = new HashSet<int>();

    public void Init(float speed, float maxDist, float returnMult, Rigidbody rb)
    {
        _speed = speed;
        _maxDist = maxDist;
        _returnMult = returnMult;
        _rb = rb;
        _startPos = transform.position;
        _controller = GetComponent<MagicController>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    private void FixedUpdate()
    {
        if (!_isReturning)
        {
            if (_rb != null) _rb.linearVelocity = transform.forward * _speed;

            if (Vector3.Distance(_startPos, transform.position) >= _maxDist)
            {
                _isReturning = true;
                _hitEnemyInstanceIds.Clear(); // 帰り用にヒット履歴をリセット
            }
        }
        else
        {
            if (_playerTransform != null)
            {
                Vector3 returnDir = (_playerTransform.position - transform.position).normalized;
                if (_rb != null) _rb.linearVelocity = returnDir * (_speed * _returnMult);

                if (Vector3.Distance(transform.position, _playerTransform.position) < 1.5f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            int id = col.gameObject.GetInstanceID();
            if (!_hitEnemyInstanceIds.Contains(id))
            {
                _hitEnemyInstanceIds.Add(id);

                // 親要素も含めて IDamageble を検索
                IDamageble damageObj = col.GetComponent<IDamageble>();
                if (damageObj == null)
                {
                    damageObj = col.GetComponentInParent<IDamageble>();
                }

                if (damageObj != null && _controller != null)
                {
                    if (_controller.Spark != null)
                    {
                        Instantiate(_controller.Spark, transform.position, Quaternion.identity);
                    }

                    int finalDamage = Mathf.Max(1, _controller.TotalAttack);
                    damageObj.AddDamage(finalDamage);
                }
            }
        }
    }
}