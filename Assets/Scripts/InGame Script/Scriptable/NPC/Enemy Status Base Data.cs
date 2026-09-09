using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "My Create Asset / Enemy")]
public class EnemyStatusBaseData : EnemyStatus
{
    [SerializeField, Min(0)] private int _hp;
    [SerializeField, Min(0)] private int _attack;
    [SerializeField, Min(0)] private float _attackCoolTime;

    [Header("複数攻撃パターンの設定")]
    [SerializeField] private List<AttackPattern> _attackPatterns = new List<AttackPattern>();

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

    public int GetHP() => _hp;
    public int GetAttack() => _attack;
    public float GetAttackCoolTime() => _attackCoolTime;
    public List<AttackPattern> GetAttackPatterns() => _attackPatterns;
    public float GetProximityRadius() => _proximityRadius;
    public int GetSearchRadius() => _searchRadius;
    public int GetSearchAngle() => _searchAngle;
    public int GetMoveSpeed() => _moveSpeed;
    public int GetBackDis() => _backDis;
    public int GetDefRate() => _defRate;
    public bool GetIsBoss() => _isBoss;
}