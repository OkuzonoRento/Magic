using UnityEngine;

[CreateAssetMenu(fileName = "Homing", menuName = "My Create Asset / MoveType / Homing")]
public class HomingMove : MoveType
{
    [SerializeField, Min(0)] private float _turnSpeed;
    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        rb.linearVelocity = Magic.transform.forward * moveSpeed;
        if (target == null) return;

        Vector3 dir = (target.position - Magic.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        Magic.rotation = Quaternion.Slerp(Magic.rotation, targetRot, _turnSpeed * Time.deltaTime);
    }
}