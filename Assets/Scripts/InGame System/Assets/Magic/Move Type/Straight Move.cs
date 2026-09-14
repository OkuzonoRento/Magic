using UnityEngine;

[CreateAssetMenu(fileName = "Straight", menuName = "My Create Asset / MoveType / Straight")]
public class StraightMove : MoveType
{
    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (rb == null || Magic == null) return;

        rb.linearVelocity = Magic.forward * moveSpeed;

        // 当たったら消えるコンポーネントが無ければ追加
        if (Magic.GetComponent<StandardBullet>() == null)
        {
            Magic.gameObject.AddComponent<StandardBullet>();
        }
    }
}

// 通常弾のヒット・消滅判定
public class StandardBullet : MonoBehaviour
{
    private MagicController _controller;

    private void Start()
    {
        _controller = GetComponent<MagicController>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            IDamageble damageObj = col.GetComponent<IDamageble>();
            if (damageObj != null && _controller != null)
            {
                if (_controller.Spark != null)
                {
                    Instantiate(_controller.Spark, transform.position, Quaternion.identity);
                }
                damageObj.AddDamage(_controller.TotalAttack);
            }
            Destroy(gameObject);
        }
        else if (col.CompareTag("Wall") || col.CompareTag("Stage"))
        {
            Destroy(gameObject);
        }
    }
}