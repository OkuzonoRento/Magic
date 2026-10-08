using UnityEngine;
using UnityEngine.UI;

namespace MagicRogue
{
    public class SpellSlotUI : MonoBehaviour
    {
        [Header("UI要素参照")]
        [SerializeField] private Image _iconImage; // クールダウン表示を行うアイコンImage

        private void Awake()
        {
            // スクリプト起動時にコード上で Image の表示形式を Filled / Radial 360 / Top に自動設定
            if (_iconImage != null)
            {
                _iconImage.type = Image.Type.Filled;
                _iconImage.fillMethod = Image.FillMethod.Radial360;
                _iconImage.fillOrigin = (int)Image.Origin360.Top;
                _iconImage.fillClockwise = true; // ★ 大文字の fillClockwise に修正
            }
        }

        /// <summary>
        /// スロットの表示・アイコン更新・クールダウン計算
        /// </summary>
        /// <param name="magic">装備されている魔法データ</param>
        /// <param name="cooldownProgress">1.0(発動直後/冷却中) ～ 0.0(完了/発動可能)</param>
        public void UpdateSlot(MagicData magic, float cooldownProgress)
        {
            // 魔法が装備されていない場合は非表示
            if (magic == null)
            {
                gameObject.SetActive(false);
                return;
            }

            // 装備されていれば表示
            gameObject.SetActive(true);

            if (_iconImage != null)
            {
                if (magic.spellIcon != null)
                {
                    _iconImage.sprite = magic.spellIcon;
                    _iconImage.enabled = true;

                    // クールダウン完了(0.0)で fillAmount = 1.0 (全表示)
                    // クールダウン中(1.0 -> 0.0)で時計回りに徐々にアイコンが復元される演出
                    _iconImage.fillAmount = 1f - Mathf.Clamp01(cooldownProgress);
                }
                else
                {
                    _iconImage.enabled = false;
                }
            }
        }
    }
}