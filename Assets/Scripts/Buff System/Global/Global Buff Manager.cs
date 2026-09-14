using System.Collections.Generic;
using UnityEngine;
using BuffSystem.Core;

namespace BuffSystem.Global
{
    public class GlobalBuffManager : MonoBehaviour
    {
        public static GlobalBuffManager Instance { get; private set; }

        [Header("所持中の全体バフ・デバフ一覧")]
        [SerializeField] private List<BuffData> _globalBuffs = new List<BuffData>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // 全体バフの登録
        public void RegisterGlobalBuff(BuffData buff)
        {
            if (buff != null && buff.Scope == BuffScope.Global)
            {
                _globalBuffs.Add(buff);
            }
        }

        // プレイヤー等の BuffHandler に所持バフを一括適用
        public void ApplyGlobalBuffsTo(BuffHandler targetHandler)
        {
            if (targetHandler == null) return;

            foreach (var buff in _globalBuffs)
            {
                targetHandler.AddBuff(buff);
            }
        }

        // 死亡時・メインメニュー復帰時にリセット
        public void ResetGlobalBuffs()
        {
            _globalBuffs.Clear();
        }
    }
}