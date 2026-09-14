using System.Collections.Generic;
using UnityEngine;
using BuffSystem.Core;

public class SceneManager : MonoBehaviour
{
    public static SceneManager _instance;

    public enum SceneState
    {
        MainMenu,
        GlobalBuffMenu,
        MapSelectMenu,
        MapBuffMenu,
        InGame,
        Shop,
    }
    [SerializeField] private SceneState _sceneState;
    [SerializeField] private Inventory _inventory;

    // 選択されたマップデータを保持するプロパティ
    private MapData _selectMapData;
    public MapData SelectMapData { get => _selectMapData; set => _selectMapData = value; }

    public SceneState MySceneState { get => _sceneState; set => _sceneState = value; }

    // --- 追加：選択されたゲーム全体バフ・デバフの保持リスト ---
    [SerializeField] private List<BuffData> _globalBuffs = new List<BuffData>();
    public List<BuffData> GlobalBuffs => _globalBuffs;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- 追加：バフ管理用関数 ---
    
    
    /// <summary>
    /// 選択画面で確定したデバフ・バフの一括設定
    /// </summary>
    public void SetGlobalBuffs(List<BuffData> debuffs, List<BuffData> buffs)
    {
        _globalBuffs.Clear();

        if (debuffs != null)
        {
            _globalBuffs.AddRange(debuffs);
        }

        if (buffs != null)
        {
            _globalBuffs.AddRange(buffs);
        }
    }

    /// <summary>
    /// 全体バフの登録
    /// </summary>
    public void RegisterGlobalBuff(BuffData buff)
    {
        if (buff != null && buff.Scope == BuffScope.Global)
        {
            _globalBuffs.Add(buff);
        }
    }

    /// <summary>
    /// プレイヤー等の BuffHandler に所持バフを一括適用
    /// </summary>
    public void ApplyGlobalBuffsTo(BuffHandler targetHandler)
    {
        if (targetHandler == null) return;

        foreach (var buff in _globalBuffs)
        {
            targetHandler.AddBuff(buff);
        }
    }

    /// <summary>
    /// リトライ・メインメニュー復帰時のバフリセット
    /// </summary>
    public void ResetGlobalBuffs()
    {
        _globalBuffs.Clear();
    }

    // --- シーン遷移処理 ---

    public void ChangeScene()
    {

        switch (_sceneState)
        {
            case SceneState.MainMenu:
                _sceneState = SceneState.GlobalBuffMenu;
                UnityEngine.SceneManagement.SceneManager.LoadScene("2_GlobalBuff");
                //_sceneState = SceneState.MapSelectMenu;
                //UnityEngine.SceneManagement.SceneManager.LoadScene("3_MapSelect");
                break;

            case SceneState.GlobalBuffMenu:
                _sceneState = SceneState.MapSelectMenu;
                UnityEngine.SceneManagement.SceneManager.LoadScene("3_MapSelect");
                break;

            case SceneState.MapSelectMenu:
                //_sceneState = SceneState.MapBuffMenu;
                //UnityEngine.SceneManagement.SceneManager.LoadScene("4_MapBuff");
                _sceneState = SceneState.InGame;
                UnityEngine.SceneManagement.SceneManager.LoadScene("5_InGame");
                break;

            case SceneState.MapBuffMenu:
                _sceneState = SceneState.InGame;
                UnityEngine.SceneManagement.SceneManager.LoadScene("5_InGame");
                break;

            case SceneState.InGame:
                _sceneState = SceneState.Shop;
                UnityEngine.SceneManagement.SceneManager.LoadScene("6_Shop");
                break;

            case SceneState.Shop:
                _sceneState = SceneState.MapSelectMenu;
                UnityEngine.SceneManagement.SceneManager.LoadScene("3_MapSelect");
                break;
        }
    }

    // メインメニューに戻る時や、Runをやり直す時にバフを消去する
    public void ReturnToMainMenu()
    {
        ResetGlobalBuffs(); // バフのリセット
        _sceneState = SceneState.MainMenu;
        UnityEngine.SceneManagement.SceneManager.LoadScene("1_MainMenu"); // ※実際のメインメニューシーン名
    }
}