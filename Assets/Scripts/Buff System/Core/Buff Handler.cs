using System.Collections.Generic;
using UnityEngine;

namespace BuffSystem.Core
{
    public class BuffHandler : MonoBehaviour
    {
        private Dictionary<BuffData, int> _activeBuffs = new Dictionary<BuffData, int>();

        // バフの付与
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

        // 指定ステータスIDの最終倍率を計算
        public float GetStatMultiplier(string statId)
        {
            float multiplier = 1.0f;

            foreach (var pair in _activeBuffs)
            {
                BuffData buff = pair.Key;
                int stackCount = pair.Value;

                if (buff.TargetStatId == statId)
                {
                    multiplier += buff.Value * stackCount;
                }
            }

            return Mathf.Max(0.05f, multiplier); // 最低値ガード
        }

        // マップ限定バフの消去
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

        // 全バフの消去（死亡時・メインメニュー移動時）
        public void ClearAllBuffs()
        {
            _activeBuffs.Clear();
        }
    }
}