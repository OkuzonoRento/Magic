using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "My Create Asset / Enemy")]
public class EnemyStatusBaseData : EnemyStatus
{
    [SerializeField, Min(0)] private int _hp;
    [SerializeField, Min(0)] private int _attack;
    [SerializeField, Min(0)] private float _attackRadius = 2.0f;
    [SerializeField, Min(0)] private float _attackCoolTime;

    [Header("攻撃判定パラメータ")]
    [SerializeField, Tooltip("攻撃の判定角度（片側）")] private float _attackAngle = 45.0f;

    [Header("連続攻撃パラメータ")]
    [SerializeField, Tooltip("1回の攻撃で出す連続攻撃の最小・最大回数")] private Vector2Int _comboCountRange = new Vector2Int(1, 3);

    [Header("感知パラメータ")]
    [SerializeField, Tooltip("全方位で気配を察知できる近距離感知半径")] private float _proximityRadius = 2.0f;

    [Header("移動・索敵パラメータ")]
    [SerializeField, Min(0)] private int _searchRadius;
    [SerializeField, Min(0)] private int _searchAngle;
    [SerializeField, Min(0)] private int _moveSpeed;
    [SerializeField, Min(0)] private int _backDis;
    [SerializeField, Range(0, 100)] private int _defRate;
    [SerializeField] private bool _isBoss;
    [SerializeField] private bool _editor;


    public int GetHP()
    {
        return _hp;
    }

    public int GetAttack()
    {
        return _attack;
    }

    public float GetAttackRadius()
    {
        return _attackRadius;
    }

    public float GetAttackAngle()
    {
        return _attackAngle;
    }

    public Vector2Int GetComboCountRange()
    {
        return _comboCountRange;
    }

    public float GetProximityRadius()
    {
        return _proximityRadius;
    }

    public float GetAttackCoolTime()
    {
        return _attackCoolTime;
    }

    public int GetSearchRadius()
    {
        return _searchRadius;
    }

    public int GetSearchAngle()
    {
        return _searchAngle;
    }

    public int GetMoveSpeed()
    {
        return _moveSpeed;
    }

    public int GetBackDis()
    {
        return _backDis;
    }

    public int GetDefRate()
    {
        return _defRate;
    }

    public bool GetIsBoss()
    {
        return _isBoss;
    }

    public bool GetEditor()
    {
        return _editor;
    }
}