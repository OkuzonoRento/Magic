using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace MagicRogue
{
    public class ResultUI : MonoBehaviour
    {
        [Header("--- 画面フェード用 ---")]
        [SerializeField] private CanvasGroup _fadeCanvasGroup;     // 画面全体を覆う黒パネルのCanvasGroup
        [SerializeField] private float _fadeDuration = 0.8f;      // 暗転にかける時間（秒）

        [Header("--- パネル参照 ---")]
        [SerializeField] private GameObject _deathPanel;         // 死亡時用パネル
        [SerializeField] private GameObject _bossClearPanel;     // ボスClear時用パネル

        [Header("--- 死亡時UI要素 ---")]
        [SerializeField] private TextMeshProUGUI _deathTitleText;
        [SerializeField] private TextMeshProUGUI _deathKillText;
        [SerializeField] private TextMeshProUGUI _deathTimeText;
        [SerializeField] private TextMeshProUGUI _deathGoldText;
        [SerializeField] private Button _returnToTitleButton;

        [Header("--- ボスClear時UI要素 ---")]
        [SerializeField] private TextMeshProUGUI _clearTitleText;
        [SerializeField] private TextMeshProUGUI _clearGoldText;
        [SerializeField] private Button _goToShopButton;

        [Header("--- 遷移先シーン名 ---")]
        [SerializeField] private string _titleSceneName = "TitleScene";
        [SerializeField] private string _shopSceneName = "ShopScene";

        private bool _isTransitioning = false;

        private void Start()
        {
            // 初期状態の設定
            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);

            if (_fadeCanvasGroup != null)
            {
                _fadeCanvasGroup.alpha = 0f;
                _fadeCanvasGroup.blocksRaycasts = false;
            }

            // ボタンイベント登録
            if (_returnToTitleButton != null)
                _returnToTitleButton.onClick.AddListener(OnReturnToTitleClicked);

            if (_goToShopButton != null)
                _goToShopButton.onClick.AddListener(OnGoToShopClicked);
        }

        #region --- 1. 死亡時（フェード暗転 >> 詳細リザルト表示 >> Title） ---

        /// <summary>
        /// プレイヤー死亡時に呼び出す
        /// </summary>
        public void ShowDeathResult(int killCount, float surviveTime, int earnedGold)
        {
            if (_isTransitioning) return;
            StartCoroutine(DeathSequenceRoutine(killCount, surviveTime, earnedGold));
        }

        private IEnumerator DeathSequenceRoutine(int killCount, float surviveTime, int earnedGold)
        {
            _isTransitioning = true;

            // 1. 画面を黒く暗転（フェードアウト）
            yield return StartCoroutine(FadeRoutine(0f, 1f));

            // ゲーム一時停止
            Time.timeScale = 0f;

            // 2. 死亡リザルトパネルを最上部に表示
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);
            if (_deathPanel != null)
            {
                _deathPanel.SetActive(true);
                // パネル自体を最前面に移動
                _deathPanel.transform.SetAsLastSibling();
            }

            if (_deathTitleText != null) _deathTitleText.text = "YOU DIED";
            if (_deathKillText != null) _deathKillText.text = $"撃破数: {killCount} 体";
            if (_deathTimeText != null) _deathTimeText.text = $"生存時間: {surviveTime:F1} 秒";
            if (_deathGoldText != null) _deathGoldText.text = $"獲得ゴールド: +{earnedGold} G";
        }

        #endregion

        #region --- 2. ボスClear時（ポータル進入後フェード >> 簡素リザルト >> Shop） ---

        /// <summary>
        /// プレイヤーがポータルに入った時に呼び出す
        /// </summary>
        public void OnEnterPortal(int earnedGold)
        {
            if (_isTransitioning) return;
            StartCoroutine(PortalSequenceRoutine(earnedGold));
        }

        private IEnumerator PortalSequenceRoutine(int earnedGold)
        {
            _isTransitioning = true;

            // 1. 画面を黒く暗転（フェードアウト）
            yield return StartCoroutine(FadeRoutine(0f, 1f));

            // ゲーム一時停止
            Time.timeScale = 0f;

            // 2. 簡素なボスクリアリザルトを最上部に表示
            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null)
            {
                _bossClearPanel.SetActive(true);
                // パネル自体を最前面に移動
                _bossClearPanel.transform.SetAsLastSibling();
            }

            if (_clearTitleText != null) _clearTitleText.text = "STAGE CLEAR!";
            if (_clearGoldText != null) _clearGoldText.text = $"+{earnedGold} G";
        }

        #endregion

        #region --- 3. シーン遷移処理 ---

        private void OnReturnToTitleClicked()
        {
            Time.timeScale = 1f;
            if (!string.IsNullOrEmpty(_titleSceneName))
            {
                SceneManager.LoadScene(_titleSceneName);
            }
        }

        private void OnGoToShopClicked()
        {
            Time.timeScale = 1f;
            if (!string.IsNullOrEmpty(_shopSceneName))
            {
                SceneManager.LoadScene(_shopSceneName);
            }
        }

        /// <summary>
        /// 時間停止中（Time.timeScale = 0）でも正しく動くフェードルーチン
        /// </summary>
        private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
        {
            if (_fadeCanvasGroup == null) yield break;

            _fadeCanvasGroup.blocksRaycasts = true;
            float timer = 0f;

            while (timer < _fadeDuration)
            {
                timer += Time.unscaledDeltaTime; // Time.timeScaleに影響されない時間計測
                _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / _fadeDuration);
                yield return null;
            }

            _fadeCanvasGroup.alpha = endAlpha;
        }

        #endregion

        private void OnDestroy()
        {
            if (_returnToTitleButton != null) _returnToTitleButton.onClick.RemoveListener(OnReturnToTitleClicked);
            if (_goToShopButton != null) _goToShopButton.onClick.RemoveListener(OnGoToShopClicked);
        }
    }
}