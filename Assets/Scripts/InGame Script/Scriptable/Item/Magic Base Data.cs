using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "My Create Asset / Item / Create Magic")]
public class MagicBaseData : Item
{
    [Space(20)]
    [SerializeField, Min(1)] private int _magicLevel = 1;
    [SerializeField, Min(0)] private int _magicAttack = 0;
    [SerializeField, Min(0)] private float _magicCoolTime = 0;
    [SerializeField, Min(0)] private float _magicMoveSpeed = 0;
    [SerializeField, Min(1)] private int _multiShotCount = 1;
    [SerializeField, Min(0)] private int _shotAngle = 0;
    [SerializeField] GameObject _magicParticle;
    [SerializeField] private MagicSpawner _instantiate;
    [SerializeField] private MoveType _moveType;


    public int GetMagicLevel()
    {
        return _magicLevel;
    }

    public int GetMagicAttack()
    {
        return _magicAttack;
    }

    public float GetMagicMoveSpeed()
    {
        return _magicMoveSpeed;
    }

    public int GetMultiShotCount()
    {
        return _multiShotCount;
    }

    public int GetShotAngle()
    {
        return _shotAngle;
    }

    public float GetMagicCoolTime()
    {
        return _magicCoolTime;
    }

    public MagicSpawner GetInstantiate()
    {
        return _instantiate;
    }

    public MoveType GetMoveType()
    {
        return _moveType;
    }

    public GameObject GetMagicParticle()
    {
        return _magicParticle;
    }
}