using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapData _mapData;
    [SerializeField] private BoxCollider _mapArea;

    public MapData MapData => _mapData;
    public BoxCollider MapArea => _mapArea;
}