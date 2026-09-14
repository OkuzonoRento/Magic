using UnityEngine;

public class TargetIconAnimation : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _amplitude;
    [SerializeField] private Transform _model;
    private Vector3 _setPos;

    private void Update()
    {
        float tagetPosY = Mathf.Sin(Time.time * _speed) * _amplitude;
        _setPos = transform.position;
        _model.position = new Vector3(transform.position.x, _setPos.y + tagetPosY, transform.position.z);
        transform.LookAt(Camera.main.transform);
    }
}
