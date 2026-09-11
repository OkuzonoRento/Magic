using UnityEngine;

[CreateAssetMenu(fileName = "Homing", menuName = "My Create Asset / MoveType / Homing")]
public class HomingMove : MoveType
{
    [SerializeField, Min(0)] private float _turnSpeed = 5.0f;

    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (rb == null || Magic == null) return;

        rb.linearVelocity = Magic.forward * moveSpeed;

        if (target != null)
        {
            Vector3 dir = (target.position - Magic.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                Magic.rotation = Quaternion.Slerp(Magic.rotation, targetRot, _turnSpeed * Time.fixedDeltaTime);
            }
        }

        // ヒット時消滅コンポーネントをアタッチ
        if (Magic.GetComponent<StandardBullet>() == null)
        {
            Magic.gameObject.AddComponent<StandardBullet>();
        }
    }
}