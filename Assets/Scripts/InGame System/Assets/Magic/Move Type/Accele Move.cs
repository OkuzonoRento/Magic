using UnityEngine;

[CreateAssetMenu(fileName = "Accele", menuName = "My Create Asset / MoveType / Accele")]
public class AcceleMove : MoveType
{
    [SerializeField] private float _acceleDelay = 0.5f;
    [SerializeField] private float _acceleRate = 20f;
    [SerializeField] private float _slowRate = 2f;

    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        if (rb == null || Magic == null) return;

        MagicController magic = Magic.GetComponent<MagicController>();
        if (magic != null)
        {
            magic.MyTimer += Time.fixedDeltaTime;
            if (magic.MyTimer > _acceleDelay)
            {
                rb.linearVelocity = Magic.forward * _acceleRate;
            }
            else
            {
                rb.linearVelocity = Magic.forward * _slowRate;
            }
        }
        else
        {
            rb.linearVelocity = Magic.forward * moveSpeed;
        }

        if (Magic.GetComponent<StandardBullet>() == null)
        {
            Magic.gameObject.AddComponent<StandardBullet>();
        }
    }
}