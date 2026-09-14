using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

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

        // 1. 先にリストから削除済みの参照（null）をクリーンアップ
        for (int i = _popLists.Count - 1; i >= 0; i--)
        {
            if (_popLists[i] == null)
            {
                _popLists.RemoveAt(i);
            }
        }

        // 2. 足りない分だけ生成
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
                _player.transform.position.y, // プレイヤーのY座標基準に設定するとSamplePositionが成功しやすい
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // プレイヤーに近すぎたらやり直し（retryをしっかりインクリメント）
            if (_player != null && Vector3.Distance(randomPos, _player.transform.position) < _popDistance)
            {
                retry++;
                continue;
            }

            // NavMesh上か確認
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

        // 位置が決まってからInstantiateを行う
        GameObject popObj = Instantiate(target, pos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        _popLists.Add(popObj);
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