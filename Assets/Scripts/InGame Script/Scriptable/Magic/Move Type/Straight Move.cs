using UnityEngine;

[CreateAssetMenu(fileName = "Straight", menuName = "My Create Asset / MoveType / Straight")]
public class StraightMove : MoveType
{
    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        rb.linearVelocity = (Magic.transform.forward * moveSpeed);
    }
}
