using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager _instance;

    public enum SceneState
    {
        MainMenu,
        PermanentMenu,
        MapSelectMenu,
        StageBuffMenu,
        InGame,
        Shop,
    }
    [SerializeField] private SceneState _sceneState;
    [SerializeField] private Inventory _inventory;

    // 追加：選択されたマップデータを保持するプロパティ
    private MapData _selectMapData;
    public MapData SelectMapData { get => _selectMapData; set => _selectMapData = value; }

    public SceneState MySceneState { get => _sceneState; set => _sceneState = value; }

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

    public void ChangeScene()
    {

        switch (_sceneState)
        {
            case SceneState.MainMenu:
                _sceneState = SceneState.PermanentMenu;
                UnityEngine.SceneManagement.SceneManager.LoadScene("2_Permanent");
                //_sceneState = SceneState.MapSelectMenu;
                //UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelect");
                break;

            case SceneState.PermanentMenu:
                _sceneState = SceneState.MapSelectMenu;
                UnityEngine.SceneManagement.SceneManager.LoadScene("3_MapSelect");
                break;

            case SceneState.MapSelectMenu:
                //_sceneState = SceneState.StageBuffMenu;
                //UnityEngine.SceneManagement.SceneManager.LoadScene("4_StageBuff");
                _sceneState = SceneState.InGame;
                UnityEngine.SceneManagement.SceneManager.LoadScene("5_InGame");
                break;

            case SceneState.StageBuffMenu:
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

}