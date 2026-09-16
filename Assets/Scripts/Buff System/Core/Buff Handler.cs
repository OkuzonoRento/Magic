using System.Collections.Generic;
using UnityEngine;

namespace BuffSystem.Core
{
    public class BuffHandler : MonoBehaviour
    {
        private Dictionary<BuffData, int> _activeBuffs = new Dictionary<BuffData, int>();
        private IDamageble _damageable;

        private void Awake()
        {
            _damageable = GetComponent<IDamageble>();
        }

        public void AddBuff(BuffData buff)
        {
            if (buff == null) return;

            if (_activeBuffs.ContainsKey(buff))
            {
                if (_activeBuffs[buff] < buff.MaxStacks)
                {
                    _activeBuffs[buff]++;
                }
            }
            else
            {
                _activeBuffs.Add(buff, 1);
            }
        }

        // 一般的なステータス倍率の計算
        public float GetStatMultiplier(string statId, float currentHpRatio = 1.0f)
        {
            float multiplier = 1.0f;

            foreach (var pair in _activeBuffs)
            {
                BuffData buff = pair.Key;
                int stackCount = pair.Value;

                // 自傷バフのHP低下による攻撃力ペナルティ
                if (buff.IsSelfDamageBuff && currentHpRatio <= buff.StopSelfDamageHpThreshold)
                {
                    if (statId == "Attack")
                    {
                        multiplier += buff.DebuffPenaltyValue * stackCount;
                        continue;
                    }
                }

                if (buff.TargetStatId == statId)
                {
                    multiplier += buff.Value * stackCount;
                }
            }

            return Mathf.Max(0.01f, multiplier);
        }

        // 確率不発（FailChance）の判定（0.0〜1.0）
        public bool ShouldFailAction()
        {
            float failChance = 0f;
            foreach (var pair in _activeBuffs)
            {
                if (pair.Key.TargetStatId == "FailChance")
                {
                    failChance += pair.Key.Value * pair.Value;
                }
            }

            if (failChance <= 0f) return false;
            return Random.value < failChance;
        }

        // 被ダメージ倍率の計算
        public float GetDamageTakenMultiplier()
        {
            return GetStatMultiplier("DamageTaken");
        }

        // 攻撃不可スロット数の計算
        public int GetDisabledSlotCount()
        {
            int disabledSlots = 0;
            foreach (var pair in _activeBuffs)
            {
                if (pair.Key.TargetStatId == "DisabledSlots")
                {
                    disabledSlots += Mathf.RoundToInt(pair.Key.Value) * pair.Value;
                }
            }
            return disabledSlots;
        }

        // 自傷処理
        public void TriggerSelfDamage(int currentHp, int maxHp)
        {
            float hpRatio = (float)currentHp / maxHp;

            foreach (var pair in _activeBuffs)
            {
                BuffData buff = pair.Key;
                if (buff.IsSelfDamageBuff && hpRatio > buff.StopSelfDamageHpThreshold)
                {
                    int damage = Mathf.Max(1, Mathf.RoundToInt(maxHp * buff.SelfDamagePercent * pair.Value));
                    if (_damageable != null)
                    {
                        _damageable.AddDamage(damage);
                    }
                }
            }
        }

        public void ClearMapBuffs()
        {
            List<BuffData> toRemove = new List<BuffData>();
            foreach (var buff in _activeBuffs.Keys)
            {
                if (buff.Scope == BuffScope.Map)
                {
                    toRemove.Add(buff);
                }
            }

            foreach (var buff in toRemove)
            {
                _activeBuffs.Remove(buff);
            }
        }

        public void ClearAllBuffs()
        {
            _activeBuffs.Clear();
        }
    }
}