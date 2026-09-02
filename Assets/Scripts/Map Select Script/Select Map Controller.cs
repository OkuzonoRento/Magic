using UnityEngine;

public class SelectMapController : MonoBehaviour
{
    [SerializeField] private PopController _popController;
    [SerializeField] private GameObject _player;

    private void Start()
    {
        // SceneManager‚©‚ç‘I‘ð‚³‚ê‚½MapData‚ðŽæ“¾
        MapData selectMap = SceneManager._instance != null ? SceneManager._instance.SelectMapData : null;

        if (selectMap == null)
        {
            Debug.LogError("‘I‘ð‚³‚ê‚½MapData‚ª‚ ‚è‚Ü‚¹‚ñ");
            return;
        }

        MapController[] maps = FindObjectsByType<MapController>(FindObjectsSortMode.None);

        MapController mapController = null;

        foreach (var map in maps)
        {
            if (map.MapData == selectMap)
            {
                mapController = map;
                break;
            }
        }

        if (mapController == null)
        {
            Debug.LogError("MapController‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
            return;
        }

        Debug.Log($"Map Position : {mapController.transform.position}");
        Debug.Log($"Spawn(Local) : {selectMap._spawnPos}");

        Vector3 spawnPos = mapController.transform.TransformPoint(selectMap._spawnPos);

        Debug.Log($"Spawn(World) : {spawnPos}");

        _player.transform.position = spawnPos;

        _popController.Initialize(selectMap, mapController);
    }
}