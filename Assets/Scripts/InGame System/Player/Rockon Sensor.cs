using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RockonSensor : MonoBehaviour
{
    public GameObject _nowTarget;
    [SerializeField] private List<GameObject> _enemyLists;

    private void Start()
    {
        _nowTarget = null;
        _enemyLists = new List<GameObject>();
    }

    private void Update()
    {
        if (_enemyLists.Count == 0)
        {
            _nowTarget = null;
            return;
        }
        else if( _enemyLists.Count != 0 && _nowTarget == null)
        {
            SetNowTarget();
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Enemy") && !_enemyLists.Contains(col.gameObject))
        {
            _enemyLists.Add(col.gameObject);
            if (_nowTarget == null)
            {
                _nowTarget = col.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Enemy") && _enemyLists.Contains(col.gameObject))
        {
            if (col.gameObject == _nowTarget)
            {
                _nowTarget = null;
            }
            _enemyLists.Remove(col.gameObject);
        }
    }

    public GameObject GetNowTarget()
    {
        return _nowTarget;
    }

    public void SetNowTarget()
    {
        foreach (var enemy in _enemyLists)
        {
            if (_nowTarget == null)
            {
                _nowTarget = enemy;
            }
        }
    }

    public void OnRockonSwitch(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if(_enemyLists.IndexOf(_nowTarget) != _enemyLists.Count - 1)
            {
                _nowTarget = _enemyLists[_enemyLists.IndexOf(_nowTarget) + 1];
            }
            else
            {
                _nowTarget = _enemyLists[0];
            }
        }
    }
}
