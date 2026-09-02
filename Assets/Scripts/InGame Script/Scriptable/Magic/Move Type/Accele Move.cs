using UnityEngine;

[CreateAssetMenu(fileName = "Accele", menuName = "My Create Asset / MoveType / Accele")]
public class AcceleMove : MoveType
{
    [SerializeField] private float _acceleDelay;
    [SerializeField] private float _acceleRate;
    [SerializeField] private float _slowRate;

    public override void MagicMove(Rigidbody rb, float moveSpeed, Transform Magic, Transform target)
    {
        MagicController magic = Magic.GetComponent<MagicController>();

        magic.MyTimer += Time.deltaTime;
        if (magic.MyTimer > _acceleDelay)
        {
            rb.linearVelocity = (Magic.transform.forward * _acceleRate);
        }
        else
        {
            rb.linearVelocity = (Magic.transform.forward * _slowRate);
        }
    }
}
