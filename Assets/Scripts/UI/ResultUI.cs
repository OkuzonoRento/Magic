using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MagicRogue
{
    public class ResultUI : MonoBehaviour
    {
        public static ResultUI Instance { get; private set; }

        [Header("--- パネル参照 ---")]
        [SerializeField] private GameObject _deathPanel;         // 死亡時用パネル
        [SerializeField] private GameObject _bossClearPanel;     // ボスClear時用パネル

        [Header("--- 超高速叩きつけ（スタンプ）演出設定 ---")]
        [SerializeField] private float _dropOffsetY = 500f;       // 開始時の上空位置
        [SerializeField] private float _startScale = 0.3f;        // 開始時の縮小率（奥に見える状態）
        [SerializeField] private float _slamDuration = 0.08f;     // 叩きつけにかかる時間

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
            // 初期状態の設定（パネル類を非表示）
            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);

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

            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);

            // ゲーム時間停止
            Time.timeScale = 0f;

            // テキスト情報の更新
            if (_deathTitleText != null) _deathTitleText.text = "YOU DIED";
            if (_deathKillText != null) _deathKillText.text = $"撃破数: {killCount} 体";
            if (_deathTimeText != null) _deathTimeText.text = $"生存時間: {surviveTime:F1} 秒";
            if (_deathGoldText != null) _deathGoldText.text = $"獲得ゴールド: +{earnedGold} G";

            // パネルを表示してスタンプ移動
            if (_deathPanel != null)
            {
                _deathPanel.SetActive(true);
                yield return StartCoroutine(AnimateStampDropRoutine(_deathPanel.GetComponent<RectTransform>()));
            }

            _isTransitioning = false;
        }

        #endregion

        #region --- 2. クリア時（ポータル到達時） ---

        public void OnEnterPortal(int earnedGold)
        {
            if (_isTransitioning) return;

            // ★ MapData.mapType をチェックしてボスマップ判定を行う ★
            bool isBossMap = false;
            if (GameSceneManager.Instance != null && GameSceneManager.Instance.SelectedMapData != null)
            {
                var mapData = GameSceneManager.Instance.SelectedMapData;
                // mapType が Boss かどうかを判定
                isBossMap = (mapData.mapType == MapType.Boss);
            }

            // ★ 通常マップの場合はResult演出を出さず、直接ショップへ遷移 ★
            if (!isBossMap)
            {
                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.OnStageCleared();
                }
                return;
            }

            // ★ ボスマップの場合のみResult画面表示
            StartCoroutine(PortalSequenceRoutine(earnedGold));
        }

        private IEnumerator PortalSequenceRoutine(int earnedGold)
        {
            _isTransitioning = true;

            if (_deathPanel != null) _deathPanel.SetActive(false);
            if (_bossClearPanel != null) _bossClearPanel.SetActive(false);

            // ゲーム時間停止
            Time.timeScale = 0f;

            // テキスト更新
            if (_clearTitleText != null) _clearTitleText.text = "STAGE CLEAR!";
            if (_clearGoldText != null) _clearGoldText.text = $"+{earnedGold} G";

            // ボスクリアパネルを超高速で叩きつける
            if (_bossClearPanel != null)
            {
                _bossClearPanel.SetActive(true);
                yield return StartCoroutine(AnimateStampDropRoutine(_bossClearPanel.GetComponent<RectTransform>()));
            }

            _isTransitioning = false;
        }

        #endregion

        #region --- 3. 一瞬で叩きつけられる超高速アニメーション ---

        private IEnumerator AnimateStampDropRoutine(RectTransform rectTransform)
        {
            if (rectTransform == null) yield break;

            Vector2 targetPos = rectTransform.anchoredPosition;
            Vector2 startPos = targetPos + new Vector2(0f, _dropOffsetY);

            Vector3 targetScale = Vector3.one;
            Vector3 startScale = Vector3.one * _startScale;

            float timer = 0f;

            while (timer < _slamDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(timer / _slamDuration);
                float easeIn = t * t;

                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, easeIn);
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, easeIn);

                yield return null;
            }

            rectTransform.anchoredPosition = targetPos;
            rectTransform.localScale = targetScale;

            // 着地後のバウンド演出
            float impactTimer = 0f;
            float impactDuration = 0.12f;

            while (impactTimer < impactDuration)
            {
                impactTimer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(impactTimer / impactDuration);
                float wave = Mathf.Sin(t * Mathf.PI * 2f) * (1f - t);

                float scaleY = 1f - (0.2f * wave);
                float scaleX = 1f + (0.12f * wave);

                rectTransform.localScale = new Vector3(scaleX, scaleY, 1f);

                yield return null;
            }

            rectTransform.localScale = targetScale;
        }

        #endregion

        #region --- 4. ボタン操作とシーン遷移 ---

        private void OnReturnToTitleClicked()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;

            SetButtonsInteractable(false);
            Time.timeScale = 1f;

            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.ChangeSceneWithFade(_titleSceneName);
            }
            else if (SceneFader.Instance != null)
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

        #endregion

        private void OnDestroy()
        {
            if (_returnToTitleButton != null) _returnToTitleButton.onClick.RemoveListener(OnReturnToTitleClicked);
            if (_goToShopButton != null) _goToShopButton.onClick.RemoveListener(OnGoToShopClicked);
        }
    }
}