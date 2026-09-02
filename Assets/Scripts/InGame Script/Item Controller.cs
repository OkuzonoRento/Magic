using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _amplitude;
    [SerializeField] private Transform _model;
    public Item _data;
    private Vector3 _setPos;

    private void Update()
    {
        float tagetPosY = Mathf.Sin(Time.time * _speed) * _amplitude;
        _setPos = transform.position;
        _model.position = new Vector3(transform.position.x, _setPos.y + tagetPosY, transform.position.z);
        transform.rotation = Quaternion.Euler(0, Time.time * _rotationSpeed, 0);
    }
}
