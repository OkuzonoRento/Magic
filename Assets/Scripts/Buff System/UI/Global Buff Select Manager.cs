using System.Collections.Generic;
using UnityEngine;
using BuffSystem.Core;
using BuffSystem.UI;

namespace BuffSystem.Global
{
    public class GlobalBuffSelectManager : MonoBehaviour
    {
        [Header("UIパネル設定")]
        [SerializeField] private GameObject _debuffPanel;
        [SerializeField] private GameObject _buffPanel;

        [Header("カード生成設定")]
        [SerializeField] private GameObject _cardPrefab;       // GlobalBuffCardUI プレハブ
        [SerializeField] private Transform _debuffContainer;  // デバフカード用親Transform (Horizontal Layout Group等)
        [SerializeField] private Transform _buffContainer;    // バフカード用親Transform

        [Header("カードプール（マスターデータ）")]
        [SerializeField] private List<BuffData> _allGlobalDebuffs = new List<BuffData>();
        [SerializeField] private List<BuffData> _allGlobalBuffs = new List<BuffData>();

        [Header("リロール設定")]
        [SerializeField] private int _initialRerollCount = 4;
        [SerializeField] private int _maxRerollCap = 5;
        [SerializeField] private int _recoveryAmount = 1;

        public int RemainingRerollCount { get; private set; }

        [Header("初期クレジット設定")]
        [SerializeField] private int _initialCredit = 0;
        public int CurrentCredit { get; private set; }

        public List<BuffData> DisplayedDebuffs { get; private set; } = new List<BuffData>();
        public List<BuffData> DisplayedBuffs { get; private set; } = new List<BuffData>();

        private HashSet<BuffData> _selectedDebuffs = new HashSet<BuffData>();
        private HashSet<BuffData> _selectedBuffs = new HashSet<BuffData>();

        private void Start()
        {
            ResetSelection();
        }

        public void ResetSelection()
        {
            CurrentCredit = _initialCredit;
            RemainingRerollCount = _initialRerollCount;
            _selectedDebuffs.Clear();
            _selectedBuffs.Clear();

            if (_debuffPanel != null) _debuffPanel.SetActive(true);
            if (_buffPanel != null) _buffPanel.SetActive(false);

            RerollDebuffs(cardCount: 3, isFreeCount: true);
        }

        // --- カード生成ロジック ---

        private void CreateCardUI(BuffData data, Transform parent, bool isDebuff)
        {
            if (_cardPrefab == null || parent == null) return;

            GameObject cardObj = Instantiate(_cardPrefab, parent);
            var cardUI = cardObj.GetComponent<GlobalBuffCardUI>();

            if (cardUI != null)
            {
                bool canAfford = isDebuff ? true : (CurrentCredit >= data.CreditCost);

                cardUI.Setup(data, isDebuff, canAfford, (selectedData, isSelected) =>
                {
                    OnCardToggled(selectedData, isSelected, isDebuff);
                });
            }
        }

        private void ClearContainer(Transform parent)
        {
            if (parent == null) return;
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }

        // --- カード選択時のトグル＆クレジット計算 ---

        private void OnCardToggled(BuffData data, bool isSelected, bool isDebuff)
        {
            if (isDebuff)
            {
                if (isSelected)
                {
                    _selectedDebuffs.Add(data);
                    CurrentCredit += data.CreditCost;
                }
                else
                {
                    _selectedDebuffs.Remove(data);
                    CurrentCredit -= data.CreditCost;
                }
            }
            else
            {
                if (isSelected)
                {
                    _selectedBuffs.Add(data);
                    CurrentCredit -= data.CreditCost;
                }
                else
                {
                    _selectedBuffs.Remove(data);
                    CurrentCredit += data.CreditCost;
                }
            }

            UpdateCreditUI();
            RefreshBuffAffordability();
        }

        // 全バフカードの「クレジット不足」状態を再更新
        private void RefreshBuffAffordability()
        {
            if (_buffContainer == null) return;

            foreach (Transform child in _buffContainer)
            {
                var cardUI = child.GetComponent<GlobalBuffCardUI>();
                if (cardUI != null && cardUI.GetData() != null)
                {
                    bool canAfford = CurrentCredit >= cardUI.GetData().CreditCost;
                    cardUI.UpdateAffordability(canAfford);
                }
            }
        }

        private void UpdateCreditUI()
        {
            // 必要に応じて CreditText への表示更新をここに記述
        }

        // --- ボタン呼び出し用（引数なし） ---
        public void OnClickRerollDebuffs()
        {
            RerollDebuffs();
        }

        public void OnClickRerollBuffs()
        {
            RerollBuffs();
        }

        // --- リロール処理 ---

        public bool RerollDebuffs(int cardCount = 3, bool isFreeCount = false)
        {
            if (!isFreeCount)
            {
                if (RemainingRerollCount <= 0) return false;
                RemainingRerollCount--;
            }

            // リロール時に現在の選択状態をリセット（加算されたクレジットを元に戻す）
            foreach (var debuff in _selectedDebuffs)
            {
                CurrentCredit -= debuff.CreditCost;
            }
            _selectedDebuffs.Clear();
            UpdateCreditUI();

            ClearContainer(_debuffContainer);
            DisplayedDebuffs = GetRandomCards(_allGlobalDebuffs, cardCount);

            foreach (var debuff in DisplayedDebuffs)
            {
                CreateCardUI(debuff, _debuffContainer, isDebuff: true);
            }

            return true;
        }

        public bool RerollBuffs(int cardCount = 3, bool isFreeCount = false)
        {
            if (!isFreeCount)
            {
                if (RemainingRerollCount <= 0) return false;
                RemainingRerollCount--;
            }

            // リロール時に現在の選択状態をリセット（消費されたクレジットを払い戻す）
            foreach (var buff in _selectedBuffs)
            {
                CurrentCredit += buff.CreditCost;
            }
            _selectedBuffs.Clear();
            UpdateCreditUI();

            ClearContainer(_buffContainer);
            DisplayedBuffs = GetRandomCards(_allGlobalBuffs, cardCount);

            foreach (var buff in DisplayedBuffs)
            {
                CreateCardUI(buff, _buffContainer, isDebuff: false);
            }

            RefreshBuffAffordability();
            return true;
        }

        // --- 画面遷移処理 ---

        public void ConfirmDebuffsAndGoToBuffs()
        {
            if (_debuffPanel != null) _debuffPanel.SetActive(false);
            if (_buffPanel != null) _buffPanel.SetActive(true);

            RemainingRerollCount = Mathf.Min(RemainingRerollCount + _recoveryAmount, _maxRerollCap);
            RerollBuffs(cardCount: 3, isFreeCount: true);
        }

        public void ConfirmAndStartRun()
        {
            if (global::SceneManager._instance != null)
            {
                global::SceneManager._instance.ResetGlobalBuffs();

                foreach (var debuff in _selectedDebuffs)
                {
                    global::SceneManager._instance.RegisterGlobalBuff(debuff);
                }

                foreach (var buff in _selectedBuffs)
                {
                    global::SceneManager._instance.RegisterGlobalBuff(buff);
                }

                global::SceneManager._instance.MySceneState = global::SceneManager.SceneState.GlobalBuffMenu;
                global::SceneManager._instance.ChangeScene();
            }
        }

        private List<BuffData> GetRandomCards(List<BuffData> sourcePool, int count)
        {
            List<BuffData> tempPool = new List<BuffData>(sourcePool);
            List<BuffData> result = new List<BuffData>();

            int selectCount = Mathf.Min(count, tempPool.Count);
            for (int i = 0; i < selectCount; i++)
            {
                int randomIndex = Random.Range(0, tempPool.Count);
                result.Add(tempPool[randomIndex]);
                tempPool.RemoveAt(randomIndex);
            }

            return result;
        }
    }
}