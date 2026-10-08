using System.Collections.Generic;
using UnityEngine;

namespace MagicRogue
{
    public class InGameHUDUI : MonoBehaviour
    {
        [Header("プレイヤー参照")]
        [SerializeField] private PlayerController _playerController;

        [Header("攻撃スロットUI参照")]
        [SerializeField] private List<SpellSlotUI> _spellSlots = new List<SpellSlotUI>();

        private void Start()
        {
            if (_playerController == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    _playerController = player.GetComponent<PlayerController>();
                }
            }
        }

        private void Update()
        {
            if (_playerController == null || _playerController.Inventory == null) return;

            var spellSlotsData = _playerController.Inventory.spellSlots;
            if (spellSlotsData == null) return;

            for (int i = 0; i < _spellSlots.Count; i++)
            {
                if (_spellSlots[i] == null) continue;

                // スロット枠内に MagicData が装備されているか確認
                if (i < spellSlotsData.Length && spellSlotsData[i] != null)
                {
                    MagicData magic = spellSlotsData[i];
                    float cdProgress = _playerController.GetCooldownProgress(i);
                    _spellSlots[i].UpdateSlot(magic, cdProgress);
                }
                else
                {
                    // 未装備時は非表示にする
                    _spellSlots[i].UpdateSlot(null, 0f);
                }
            }
        }
    }
}