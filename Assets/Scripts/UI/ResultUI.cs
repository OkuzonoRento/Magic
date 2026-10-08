using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MagicRogue
{
    public class ResultUI : MonoBehaviour
    {
        public static ResultUI Instance { get; private set; }

        [Header("--- 画面フェード用 ---")]
        [SerializeField] private CanvasGroup _fadeCanvasGroup;     // 画面全体を覆う黒パネルのCanvasGroup
        [SerializeField] private float _fadeDuration = 0.8f;      // リザルト表示時の暗転にかける時間（秒）

        [Header("--- パネル参照 ---")]
        [SerializeField] private GameObject _deathPanel;         // 死亡時用パネル
        [SerializeField] private GameObject _bossClearPanel;     // ボスClear時用パネル

        [Header("--- 超高速叩きつけ（スタンプ）演出設定 ---")]
        [SerializeField] private float _dropOffsetY = 500f;       // 開始時の上空位置
        [SerializeField] private float _startScale = 0.3f;        // 開始時の縮小率（奥に見える状態）
        [SerializeField] private float _slamDuration = 0.08f;     // 叩きつけにかかる時間（超高速：0.06～0.1秒推奨）

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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 初期状態の設定
            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);

            if (_fadeCanvasGroup != null)
            {
                _fadeCanvasGroup.alpha = 0f;
                _fadeCanvasGroup.blocksRaycasts = false;
                _fadeCanvasGroup.interactable = false;
            }

            // ボタンイベント登録
            if (_returnToTitleButton != null)
                _returnToTitleButton.onClick.AddListener(OnReturnToTitleClicked);

            if (_goToShopButton != null)
                _goToShopButton.onClick.AddListener(OnGoToShopClicked);
        }

        #region --- 1. 死亡時 ---

        public void ShowDeathResult(int killCount, float surviveTime, int earnedGold)
        {
            if (_isTransitioning) return;
            StartCoroutine(DeathSequenceRoutine(killCount, surviveTime, earnedGold));
        }

        private IEnumerator DeathSequenceRoutine(int killCount, float surviveTime, int earnedGold)
        {
            _isTransitioning = true;

            // 0. パネル類を一旦非表示
            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);

            // 1. 画面を黒く暗転（フェードアウト）
            yield return StartCoroutine(FadeRoutine(0f, 1f));

            // 2. ゲーム時間停止（フェード完了後）
            Time.timeScale = 0f;

            // 3. テキスト情報の更新
            if (_deathTitleText != null) _deathTitleText.text = "YOU DIED";
            if (_deathKillText != null) _deathKillText.text = $"撃破数: {killCount} 体";
            if (_deathTimeText != null) _deathTimeText.text = $"生存時間: {surviveTime:F1} 秒";
            if (_deathGoldText != null) _deathGoldText.text = $"獲得ゴールド: +{earnedGold} G";

            // 4. 黒画面の上にパネルを表示し、超高速でバンッ！と叩きつける
            if (_deathPanel != null)
            {
                _deathPanel.SetActive(true);
                yield return StartCoroutine(AnimateStampDropRoutine(_deathPanel.GetComponent<RectTransform>()));
            }

            // 5. UI操作の受け取りを有効化
            if (_fadeCanvasGroup != null)
            {
                _fadeCanvasGroup.blocksRaycasts = false;
            }

            _isTransitioning = false;
        }

        #endregion

        #region --- 2. ボスClear時 ---

        public void OnEnterPortal(int earnedGold)
        {
            if (_isTransitioning) return;
            StartCoroutine(PortalSequenceRoutine(earnedGold));
        }

        private IEnumerator PortalSequenceRoutine(int earnedGold)
        {
            _isTransitioning = true;

            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);

            // 1. 画面を黒く暗転（フェードアウト）
            yield return StartCoroutine(FadeRoutine(0f, 1f));

            // 2. ゲーム時間停止
            Time.timeScale = 0f;

            // 3. テキスト更新
            if (_clearTitleText != null) _clearTitleText.text = "STAGE CLEAR!";
            if (_clearGoldText != null) _clearGoldText.text = $"+{earnedGold} G";

            // 4. ボスクリアパネルを超高速でバンッ！と叩きつける
            if (_bossClearPanel != null)
            {
                _bossClearPanel.SetActive(true);
                yield return StartCoroutine(AnimateStampDropRoutine(_bossClearPanel.GetComponent<RectTransform>()));
            }

            if (_fadeCanvasGroup != null)
            {
                _fadeCanvasGroup.blocksRaycasts = false;
            }

            _isTransitioning = false;
        }

        #endregion

        #region --- 3. 一瞬で叩きつけられる超高速アニメーション ---

        private IEnumerator AnimateStampDropRoutine(RectTransform rectTransform)
        {
            if (rectTransform == null) yield break;

            Vector2 targetPos = rectTransform.anchoredPosition; // 本来の表示位置
            Vector2 startPos = targetPos + new Vector2(0f, _dropOffsetY);

            Vector3 targetScale = Vector3.one;
            Vector3 startScale = Vector3.one * _startScale;

            float timer = 0f;

            // Phase 1: 目にも留まらぬ速さで一気に着地位置まで叩きつける
            while (timer < _slamDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(timer / _slamDuration);

                // 重力による急加速（EaseInQuad）
                float easeIn = t * t;

                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, easeIn);
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, easeIn);

                yield return null;
            }

            // 着地位置・拡大率を確実に合わせる
            rectTransform.anchoredPosition = targetPos;
            rectTransform.localScale = targetScale;

            // Phase 2: 叩きつけ直後の「ドスン！」という短い着地バウンド（衝撃）
            float impactTimer = 0f;
            float impactDuration = 0.12f;

            while (impactTimer < impactDuration)
            {
                impactTimer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(impactTimer / impactDuration);

                // 叩きつけの衝撃で縦が一瞬潰れ、横に微振動して戻る
                float wave = Mathf.Sin(t * Mathf.PI * 2f) * (1f - t);

                float scaleY = 1f - (0.2f * wave);
                float scaleX = 1f + (0.12f * wave);

                rectTransform.localScale = new Vector3(scaleX, scaleY, 1f);

                yield return null;
            }

            rectTransform.localScale = targetScale; // ジャストサイズに固定
        }

        #endregion

        #region --- 4. SceneFader を使ったシーン遷移 ---

        private void OnReturnToTitleClicked()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;

            SetButtonsInteractable(false);
            Time.timeScale = 1f;

            if (SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeAndLoadScene(_titleSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(_titleSceneName);
            }
        }

        private void OnGoToShopClicked()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;

            SetButtonsInteractable(false);
            Time.timeScale = 1f;

            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.OnStageCleared();
            }
            else if (SceneFader.Instance != null)
            {
                SceneFader.Instance.FadeAndLoadScene(_shopSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(_shopSceneName);
            }
        }

        private void SetButtonsInteractable(bool state)
        {
            if (_returnToTitleButton != null) _returnToTitleButton.interactable = state;
            if (_goToShopButton != null) _goToShopButton.interactable = state;
        }

        private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
        {
            if (_fadeCanvasGroup == null) yield break;

            _fadeCanvasGroup.gameObject.SetActive(true);
            _fadeCanvasGroup.blocksRaycasts = true;
            _fadeCanvasGroup.interactable = false;

            float timer = 0f;

            while (timer < _fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
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