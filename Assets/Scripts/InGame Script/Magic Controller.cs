using UnityEngine;

public class MagicController : MonoBehaviour
{
    public MagicBaseData _baseData;
    public Transform _target;

    private Rigidbody _rb;
    private MoveType _moveData;
    [SerializeField] private int _playerAttack;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _atack;
    [SerializeField] private int _lv;
    [SerializeField] private GameObject _spark;
    private float _timer;

    public float MyTimer { get => _timer; set => _timer = value; }
    public int TotalAttack => _atack + _playerAttack + _lv;
    public GameObject Spark => _spark;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _moveData = _baseData.GetMoveType();
        _moveSpeed = _baseData.GetMagicMoveSpeed();
        _atack = _baseData.GetMagicAttack();
        _lv = _baseData.GetMagicLevel();
        Destroy(gameObject, 5.0f); // à¿ëSëŒçÙÇÃéıñΩÅií∑ÇﬂÅj
    }

    void FixedUpdate()
    {
        if (_moveData != null)
        {
            _moveData.MagicMove(_rb, _moveSpeed, gameObject.transform, _target);
        }
    }
}