using System.Collections.Generic;
using UnityEngine;
using BuffSystem.Core;

namespace BuffSystem.Map
{
    public class MapBuffManager : MonoBehaviour
    {
        [Header("このマップ固有の環境バフ・デバフ")]
        [SerializeField] private List<BuffData> _currentMapBuffs = new List<BuffData>();

        // マップバフをプレイヤー等に適用
        public void ApplyMapBuffsTo(BuffHandler targetHandler)
        {
            if (targetHandler == null) return;

            foreach (var buff in _currentMapBuffs)
            {
                if (buff.Scope == BuffScope.Map)
                {
                    targetHandler.AddBuff(buff);
                }
            }
        }

        // 次のマップへ移動する時のリセット処理
        public void OnLeaveMap(BuffHandler targetHandler)
        {
            if (targetHandler != null)
            {
                targetHandler.ClearMapBuffs();
            }
        }
    }
}