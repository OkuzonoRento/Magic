using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Vector3 _cameraSpeed;
    [Header("カメラ感度")]
    [SerializeField, Range(0.0f, 1.0f)] private Vector2 _cameraSensitivity = new Vector2(0.1f, 0.1f);

    public GameObject _targetObject;
    private float _cameraHeight = 1.5f;
    private float _cameraDistance = 10.0f;
    private float _cameraRotAngle = 0.0f;
    private float _cameraHeightAngle = 10.0f;
    private float _camera_dis_min = 5.0f;
    private float _camera_dis_mdl = 12.5f;
    private Vector3 _cameraNowPos;
    private float _cameraNowRotAngle;
    private float _cameraNowHeightAngle = 30.0f;


    public bool _enableAtten = true;
    private float _attenRate = 3.0f;
    private float _forwardDistance = 2.0f;
    private Vector3 _addForward;
    private Vector3 _prevTargetPos;
    private float _rotAngleAttenRate = 5.0f;
    //private float _angleAttenRate = 1.0f;


    public bool _rock = false;
    public GameObject _rockonTarget;
    public GameObject _searchCircle;

    public GameObject _targetIcon;

    void Start()
    {
        _cameraNowPos = _targetObject.transform.position;
    }

    void LateUpdate()
    {
        _cameraRotAngle -= _cameraSpeed.x * Time.deltaTime * _cameraSensitivity.x * 10;
        _cameraHeightAngle += _cameraSpeed.z * Time.deltaTime * _cameraSensitivity.y * 10;
        _cameraHeightAngle = Mathf.Clamp(_cameraHeightAngle, 0.0f, 40.0f);        //垂直方向の角度制限
        _cameraDistance = Mathf.Clamp(_cameraDistance, 5.0f, 40.0f);                //カメラ距離制限

        _rockonTarget = _searchCircle.GetComponent<RockonSensor>()._nowTarget;

        if(_rockonTarget != null)
        {
            _rock = true;
        }
        else
        {
            _rock = false;
        }

        if (_enableAtten)
        {
            var target = _targetObject.transform.position;

            if (_rock)
            {
                if (_rockonTarget != null)
                {
                    target = _rockonTarget.transform.position;
                }
                else
                {
                    _rock = false;
                }
            }

            var halfPoint = (_targetObject.transform.position + target) / 2;
            var deltaPos = halfPoint - _prevTargetPos;
            _prevTargetPos = halfPoint;
            deltaPos *= _forwardDistance;

            _addForward += deltaPos * Time.deltaTime * 20.0f;
            _addForward = Vector3.Lerp(_addForward, Vector3.zero, Time.deltaTime * _attenRate);

            _cameraNowPos = Vector3.Lerp(_cameraNowPos, halfPoint + Vector3.up * _cameraHeight + _addForward, Mathf.Clamp01(Time.deltaTime * _attenRate));
            _cameraNowRotAngle = Mathf.Lerp(_cameraNowRotAngle, _cameraRotAngle, Time.deltaTime * _rotAngleAttenRate);
            _cameraNowHeightAngle = Mathf.Lerp(_cameraNowHeightAngle, _cameraHeightAngle, Time.deltaTime * _rotAngleAttenRate);
        }
        else
        {
            _cameraNowPos = _targetObject.transform.position + Vector3.up * _cameraHeight;
            _cameraNowRotAngle = _cameraRotAngle;
            _cameraNowHeightAngle = _cameraHeightAngle;
        }

        if (_rock)
        {
            var dis = Vector3.Distance(_targetObject.transform.position, _rockonTarget.transform.position);

            if (_cameraHeightAngle > 30.0f)
            {
                _cameraDistance = Mathf.Lerp(_cameraDistance, _camera_dis_mdl * dis / 10 * _cameraNowHeightAngle / 30.0f, Time.deltaTime);
            }
            else if (_cameraHeightAngle <= 30.0f && _cameraHeightAngle >= -3)
            {
                _cameraDistance = Mathf.Lerp(_cameraDistance, _camera_dis_mdl * dis / 10, Time.deltaTime);
            }
            else if (_cameraHeightAngle < -3)
            {
                _rock = false;
            }
        }
        else
        {
            if (_cameraHeightAngle > 30.0f)
            {
                _cameraDistance = Mathf.Lerp(_cameraDistance, 20.0f * _cameraHeightAngle / 30.0f, Time.deltaTime);
            }
            else if (_cameraHeightAngle <= 30.0f && _cameraHeightAngle >= -3)
            {
                _cameraDistance = Mathf.Lerp(_cameraDistance, 20.0f, Time.deltaTime);
            }
            else if (_cameraHeightAngle < -3)
            {
                _cameraDistance = Mathf.Lerp(_cameraDistance, _camera_dis_min, Time.deltaTime);
            }
        }

        var deg = Mathf.Deg2Rad;
        var cameraX = Mathf.Sin(_cameraNowRotAngle * deg) * Mathf.Cos(_cameraNowHeightAngle * deg) * _cameraDistance;
        var cameraZ = -Mathf.Cos(_cameraNowRotAngle * deg) * Mathf.Cos(_cameraNowHeightAngle * deg) * _cameraDistance;
        var cameraY = Mathf.Sin(_cameraNowHeightAngle * deg) * _cameraDistance;
        transform.position = _cameraNowPos + new Vector3(cameraX, cameraY, cameraZ);

        var cameraRot = Quaternion.LookRotation((_cameraNowPos - transform.position).normalized);
        transform.rotation = cameraRot;

        TargetIcon();
    }

    private void TargetIcon()
    {
        if(_rock && _rockonTarget != null && _rockonTarget.transform.GetChild(1) != null)
        {
            _targetIcon.SetActive(true);
            _targetIcon.transform.position = _rockonTarget.transform.GetChild(1).transform.position;
        }
        else
        {
            _targetIcon.SetActive(false);
        }
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        _cameraSpeed = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }
}
