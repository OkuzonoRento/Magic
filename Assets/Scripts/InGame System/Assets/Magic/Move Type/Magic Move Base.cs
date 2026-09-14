using UnityEngine;

public abstract class MoveType : ScriptableObject
{
    public abstract void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target);
}
