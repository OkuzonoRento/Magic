using UnityEngine;

public class MagicController : MonoBehaviour
{
    public MagicBaseData _baseData;
    public Transform _target;

    private Rigidbody _rb;
    private MoveType _moveData;
    [SerializeField] private int _playerAttack;
    [SerializeField] private float _moveSpeed;  //Debug
    [SerializeField] private int _atack;        //Debug
    [SerializeField] private int _lv;           //Debug
    [SerializeField] private GameObject _spark;
    private float _timer;

    public float MyTimer { get => _timer; set => _timer = value; }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _moveData = _baseData.GetMoveType();
        _moveSpeed = _baseData.GetMagicMoveSpeed();
        _atack = _baseData.GetMagicAttack();
        _lv = _baseData.GetMagicLevel();
        Destroy(gameObject, 2.5f);
    }

    void FixedUpdate()
    {
        _moveData.MagicMove(_rb, _moveSpeed, gameObject.transform, _target);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            IDamageble damageObj = col.gameObject.GetComponent<IDamageble>();
            if (damageObj != null)
            {
                if (_spark != null)
                {
                    Instantiate(_spark, transform.position, Quaternion.identity);
                }

                damageObj.AddDamage(_atack + _playerAttack + _lv);
            }
            Destroy(gameObject);
        }
        
        if(col.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
