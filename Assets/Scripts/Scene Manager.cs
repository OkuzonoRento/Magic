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
                UnityEngine.SceneManagement.SceneManager.LoadScene("Permanent");
                //_sceneState = SceneState.MapSelectMenu;
                //UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelect");
                break;

            case SceneState.PermanentMenu:
                _sceneState = SceneState.MapSelectMenu;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelect");
                break;

            case SceneState.MapSelectMenu:
                _sceneState = SceneState.InGame;
                UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
                break;

            case SceneState.StageBuffMenu:
                break;

            case SceneState.InGame:
                _sceneState = SceneState.Shop;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Shop");
                break;

            case SceneState.Shop:
                _sceneState = SceneState.MapSelectMenu;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelect");
                break;
        }
    }

}