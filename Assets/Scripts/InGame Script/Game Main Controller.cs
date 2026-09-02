using UnityEngine;
using UnityEngine.UI;

public class GameMainController : MonoBehaviour
{
    [SerializeField, Min(0)] private float _clearTime = 0.0f;
    [SerializeField] private Slider _gameTimer;
    private float _cleartimer;
    private SceneManager _sceneManager;

    [SerializeField] private MapID _mapID;

    public enum MapID
    {
        NomalMap,
        BossMap,
    }

    public MapID SelectMap { get => _mapID; }

    private void Awake()
    {
        _sceneManager = SceneManager._instance;
        _cleartimer = 0.0f;
        _gameTimer.maxValue = _clearTime;
        _gameTimer.value = 0.0f;
    }

    private void FixedUpdate()
    {
        _cleartimer += Time.deltaTime;
        _gameTimer.value = _cleartimer;
        if (_cleartimer >= _clearTime) _sceneManager.ChangeScene();
    }
}
