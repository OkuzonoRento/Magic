using UnityEngine;

namespace BuffSystem.Core
{
    public enum BuffScope
    {
        Global,
        Map
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

        [Header("経済パラメータ")]
        [SerializeField] private int _creditCost; // デバフ時は獲得量、バフ時は消費量

        [Header("ステータス変化パラメータ")]
        [SerializeField] private string _targetStatId; //例: Attack, MoveSpeed, MaxHP, Cooldown, SearchRange
        [SerializeField] private float _value;          // 補正値 (例: 0.2, -0.3)
        [SerializeField] private int _maxStacks = 1;    // 追加：最大重複制限（基本は1）

        [Header("特殊挙動（該当バフのみ設定）")]
        [SerializeField] private bool _isSelfDamageBuff;
        [SerializeField] private float _selfDamagePercent = 0.02f; // 発動時自傷割合 (2%)
        [SerializeField] private float _stopSelfDamageHpThreshold = 0.2f; // 自傷停止HP閾値 (20%)
        [SerializeField] private float _debuffPenaltyValue = -0.3f; // 閾値以下の攻撃力低下率 (-30%)

        // プロパティ
        public string BuffId => _buffId;
        public string BuffName => _buffName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public BuffScope Scope => _scope;
        public int CreditCost => _creditCost;
        public string TargetStatId => _targetStatId;
        public float Value => _value;
        public int MaxStacks => _maxStacks; // 追加：プロパティ
        public bool IsSelfDamageBuff => _isSelfDamageBuff;
        public float SelfDamagePercent => _selfDamagePercent;
        public float StopSelfDamageHpThreshold => _stopSelfDamageHpThreshold;
        public float DebuffPenaltyValue => _debuffPenaltyValue;
    }
}