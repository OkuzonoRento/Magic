using UnityEngine;
using UnityEngine.UI;

namespace MagicRogue
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("スライダー参照")]
        [SerializeField] private Slider hpSlider;

        [Header("アニメーション速度")]
        [SerializeField] private float changeSpeed = 8f;

        private float targetHp = 100f;

        private void Update()
        {
            if (hpSlider == null) return;

            // targetHp に向かって slider.value を滑らかに変化させる
            hpSlider.value = Mathf.Lerp(hpSlider.value, targetHp, Time.unscaledDeltaTime * changeSpeed);
        }

        /// <summary>
        /// 最大HPと初期HPのセットアップ
        /// </summary>
        public void Initialize(float maxHp, float currentHp)
        {
            if (hpSlider != null)
            {
                hpSlider.maxValue = maxHp;
                hpSlider.value = currentHp;
            }
            targetHp = currentHp;
        }

        /// <summary>
        /// HP目標値を更新（ダメージ・回復時）
        /// </summary>
        public void UpdateHealth(float newHp)
        {
            targetHp = newHp;
        }
    }
}