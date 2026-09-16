using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using BuffSystem.Core;

public class PopController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private MapData _selectMap;
    [SerializeField] private List<GameObject> _popLists = new List<GameObject>();
    [SerializeField, Min(0)] private float _popDistance = 5.0f;
    private MapController _mapController;

    public MapData Select => _selectMap;

    private void FixedUpdate()
    {
        if (_selectMap == null || _mapController == null || _mapController.MapArea == null) return;

        for (int i = _popLists.Count - 1; i >= 0; i--)
        {
            if (_popLists[i] == null)
            {
                _popLists.RemoveAt(i);
            }
        }

        if (_selectMap._popCount > _popLists.Count)
        {
            int popCount = _selectMap._popCount - _popLists.Count;
            for (int i = 0; i < popCount; i++)
            {
                int randomIndex = Random.Range(0, _selectMap._popEnemy.Count);
                Pop(_selectMap._popEnemy[randomIndex]);
            }
        }
    }

    private void Pop(GameObject target)
    {
        if (target == null) return;

        Bounds bounds = _mapController.MapArea.bounds;
        Vector3 pos = Vector3.zero;
        NavMeshHit hit;
        int retry = 0;
        bool foundPosition = false;

        while (retry < 100)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                _player.transform.position.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (_player != null && Vector3.Distance(randomPos, _player.transform.position) < _popDistance)
            {
                retry++;
                continue;
            }

            if (NavMesh.SamplePosition(randomPos, out hit, 10f, NavMesh.AllAreas))
            {
                pos = hit.position;
                foundPosition = true;
                break;
            }

            retry++;
        }

        if (!foundPosition)
        {
            Debug.LogWarning("スポーン位置が見つかりませんでした。");
            return;
        }

        GameObject popObj = Instantiate(target, pos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        _popLists.Add(popObj);

        // 敵へのバフ自動適用
        InGameBuffManager.ApplyBuffsToEnemy(popObj);
    }

    public void Initialize(MapData mapData, MapController mapController)
    {
        _selectMap = mapData;
        _mapController = mapController;
    }

    public void SetMapController(MapController controller)
    {
        _mapController = controller;
    }
}