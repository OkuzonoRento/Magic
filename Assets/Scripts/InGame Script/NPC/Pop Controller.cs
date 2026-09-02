using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
public class PopController : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] private MapData _selectMap;
    [SerializeField] private List<GameObject> _popLists = new List<GameObject>();
    [SerializeField, Min(0)] private float _popDistance = 5.0f;
    private MapController _mapController;

    public MapData Select => _selectMap;

    private void FixedUpdate()
    {
        if (_selectMap._popCount > _popLists.Count)
        {
            int popCount = _selectMap._popCount - _popLists.Count;
            for (int i = 0; i < popCount; i++)
            {
                int randomIndex = Random.Range(0, _selectMap._popEnemy.Count);
                Pop(_selectMap._popEnemy[randomIndex]);
            }
        }

        for (int i = _popLists.Count - 1; i >= 0; i--)
        {
            if (_popLists[i] == null)
            {
                _popLists.RemoveAt(i);
            }
        }
    }

    private void Pop(GameObject target)
    {
        GameObject popObj = Instantiate(target);
        _popLists.Add(popObj);

        Bounds bounds = _mapController.MapArea.bounds;

        Vector3 pos;
        NavMeshHit hit;
        int retry = 0;

        while (true)
        {
            pos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), 0, Random.Range(bounds.min.z, bounds.max.z));

            // プレイヤーに近すぎたらやり直し
            if (Vector3.Distance(pos, _player.transform.position) < _popDistance)
            {
                retry++;
                continue;
            }

            // NavMesh上か確認
            if (NavMesh.SamplePosition(pos, out hit, 10f, NavMesh.AllAreas))
            {
                pos = hit.position;
                break;
            }

            retry++;

            if (retry >= 100)
            {
                Debug.LogWarning("スポーン位置が見つかりません。");
                Destroy(popObj);
                _popLists.Remove(popObj);
                return;
            }
        }

        popObj.transform.position = pos;
        popObj.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        popObj.SetActive(true);
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