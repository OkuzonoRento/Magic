using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "My Create Asset / Enemy")]
public class EnemyStatusBaseData : EnemyStatus
{
    [SerializeField, Min(0)] private int _hp;
    [SerializeField, Min(0)] private int _attack;
    [SerializeField, Min(0)] private float _attackCoolTime;

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
