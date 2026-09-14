using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "My Create Asset / MoveType / Explosion")]
public class ExplosionMove : MoveType
{
    [SerializeField] private float _explosionRadius = 3.0f; // 爆発範囲
    [SerializeField] private GameObject _explosionEffect; // 爆発エフェクト

    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (Magic == null) return;

        if (rb != null)
        {
            rb.linearVelocity = Magic.forward * moveSpeed;
        }

        ExplosionBullet explosion = Magic.GetComponent<ExplosionBullet>();
        if (explosion == null)
        {
            explosion = Magic.gameObject.AddComponent<ExplosionBullet>();
            explosion.Init(_explosionRadius, _explosionEffect);
        }
    }
}

public class ExplosionBullet : MonoBehaviour
{
    private float _radius;
    private GameObject _effect;
    private MagicController _controller;
    private bool _hasExploded = false;

    public void Init(float radius, GameObject effect)
    {
        _radius = radius;
        _effect = effect;
        _controller = GetComponent<MagicController>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (_hasExploded) return;

        if (col.CompareTag("Enemy") || col.CompareTag("Wall") || col.CompareTag("Stage"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        _hasExploded = true;

        // 爆発エフェクト生成
        if (_effect != null)
        {
            Instantiate(_effect, transform.position, Quaternion.identity);
        }

        // 爆発範囲内のコライダーをすべて取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius);
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

        Destroy(gameObject);
    }

    // Sceneビューで爆発範囲を可視化（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}