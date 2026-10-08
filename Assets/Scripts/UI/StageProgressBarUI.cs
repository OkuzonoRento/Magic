using UnityEngine;
using UnityEngine.UI;

namespace MagicRogue
{
    public class StageProgressBarUI : MonoBehaviour
    {
        [Header("UI要素参照")]
        [SerializeField] private Slider _progressSlider;          // クリア状況 / ボスHP用スライダー

        [Header("ゲージ演出設定")]
        [SerializeField] private bool _invertTimeBar = false;     // Survival時: trueで時間増加方向、falseで時間減少方向
        [SerializeField] private float _smoothSpeed = 5f;         // ゲージの追従スピード（値が大きいほど速く追従）

        private MapData _currentMapData;
        private EnemyController _activeBoss;                     // 現在進行中のボス参照

        private float _targetValue = 0f;                          // 目標とするスライダー値

        private void Start()
        {
            if (_progressSlider != null)
            {
                _progressSlider.interactable = false; // ユーザーのドラッグ操作を無効化
            }
        }

        private void Update()
        {
            if (EnemySpawner.Instance == null) return;

            // SelectedMapData を取得
            if (_currentMapData == null && GameSceneManager.Instance != null)
            {
                _currentMapData = GameSceneManager.Instance.SelectedMapData;
            }

            if (_currentMapData == null) return;

            // 目標値の更新
            UpdateGaugeTarget(_currentMapData);

            // スライダーの値を目標値に向かって滑らかにアニメーション
            AnimateSlider();
        }

        /// <summary>
        /// ボス生成時に外部（BossController や Spawner）からボス参照を直接セットする場合用
        /// </summary>
        public void SetBossTarget(EnemyController boss)
        {
            _activeBoss = boss;
        }

        private void UpdateGaugeTarget(MapData mapData)
        {
            switch (mapData.clearConditionType)
            {
                case ClearConditionType.SurviveTime:
                    UpdateSurviveTimeTarget(mapData);
                    break;

                case ClearConditionType.KillCount:
                    UpdateKillCountTarget(mapData);
                    break;

                case ClearConditionType.BossDefeat:
                    UpdateBossHpTarget();
                    break;

                default:
                    gameObject.SetActive(false);
                    break;
            }
        }

        private void UpdateSurviveTimeTarget(MapData mapData)
        {
            float maxTime = mapData.targetSurviveTime;
            float remainingTime = EnemySpawner.Instance.RemainingTime;

            if (maxTime <= 0f || _progressSlider == null) return;

            _progressSlider.minValue = 0f;
            _progressSlider.maxValue = maxTime;

            _targetValue = _invertTimeBar ? (maxTime - remainingTime) : remainingTime;
        }

        private void UpdateKillCountTarget(MapData mapData)
        {
            int targetKills = mapData.targetKillCount;
            int currentKills = EnemySpawner.Instance.CurrentKillCount;

            if (targetKills <= 0 || _progressSlider == null) return;

            _progressSlider.minValue = 0f;
            _progressSlider.maxValue = targetKills;
            _targetValue = Mathf.Min(currentKills, targetKills);
        }

        private void UpdateBossHpTarget()
        {
            if (_progressSlider == null) return;

            if (_activeBoss == null)
            {
                GameObject bossObj = GameObject.FindWithTag("Boss");
                if (bossObj != null)
                {
                    _activeBoss = bossObj.GetComponent<EnemyController>();
                }
            }

            if (_activeBoss == null)
            {
                _progressSlider.minValue = 0f;
                _progressSlider.maxValue = 1f;
                _targetValue = 1f;
                return;
            }

            float maxHp = _activeBoss.MaxHp;
            float currentHp = _activeBoss.CurrentHp;

            _progressSlider.minValue = 0f;
            _progressSlider.maxValue = maxHp;
            _targetValue = Mathf.Max(0f, currentHp);
        }

        /// <summary>
        /// 毎フレーム Lerp を使ってスライダーの値を滑らかに目標値へアプローチさせる
        /// </summary>
        private void AnimateSlider()
        {
            if (_progressSlider == null) return;

            // Mathf.Lerp で現在の value から _targetValue へ滑らかに移動
            _progressSlider.value = Mathf.Lerp(_progressSlider.value, _targetValue, Time.deltaTime * _smoothSpeed);
        }
    }
}