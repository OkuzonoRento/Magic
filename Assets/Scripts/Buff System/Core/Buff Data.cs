using UnityEngine;

namespace BuffSystem.Core
{
    public enum BuffScope
    {
        Global,
        Map
    }

    public enum BuffTargetType
    {
        Player,
        Enemy,
        Both
    }

    [CreateAssetMenu(fileName = "NewBuffData", menuName = "Buff System/Buff Data")]
    public class BuffData : ScriptableObject
    {
        [Header("基本情報")]
        [SerializeField] private string _buffId;
        [SerializeField] private string _buffName;
        [TextArea][SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private BuffScope _scope = BuffScope.Global;
        [SerializeField] private BuffTargetType _targetType = BuffTargetType.Player;

        [Header("経済パラメータ（売却額・購入額など）")]
        [SerializeField] private int _creditCost;

        [Header("ステータス変化パラメータ")]
        // TargetStatIdの一覧:
        // 【バフ】 Attack, Cooldown, MaxHP, LockOnRange, DropRate, EnemyStat
        // 【デバフ】 DamageTaken, FailChance, LockOnRange, SellPrice, BuyPrice, DisabledSlots
        [SerializeField] private string _targetStatId;
        [SerializeField] private float _value;          // 補正値 (例: 0.2, -0.3, 1.0)
        [SerializeField] private int _maxStacks = 1;

        [Header("特殊挙動")]
        [SerializeField] private bool _isSelfDamageBuff;
        [SerializeField] private float _selfDamagePercent = 0.02f;
        [SerializeField] private float _stopSelfDamageHpThreshold = 0.2f;
        [SerializeField] private float _debuffPenaltyValue = -0.3f;

        public string BuffId => _buffId;
        public string BuffName => _buffName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public BuffScope Scope => _scope;
        public BuffTargetType TargetType => _targetType;
        public int CreditCost => _creditCost;
        public string TargetStatId => _targetStatId;
        public float Value => _value;
        public int MaxStacks => _maxStacks;
        public bool IsSelfDamageBuff => _isSelfDamageBuff;
        public float SelfDamagePercent => _selfDamagePercent;
        public float StopSelfDamageHpThreshold => _stopSelfDamageHpThreshold;
        public float DebuffPenaltyValue => _debuffPenaltyValue;
    }
}