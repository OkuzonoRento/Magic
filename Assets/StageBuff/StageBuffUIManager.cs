using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageBuffUIManager : MonoBehaviour
{
    [SerializeField, Header("UIの親オブジェクト")] private Transform _uiParent;
    [SerializeField, ReadOnly] private List<StageBuffUI> _UIList;
    [SerializeField, ReadOnly] private List<StageBuff> m_StagebuffList;
    [SerializeField, ReadOnly] private List<StageBuff> RemoveList;
    [SerializeField, ReadOnly] private StageBuffUI _currentSelect;

    [SerializeField] private StageBuffAll _stageBuffAll;

    [SerializeField] private GameObject _StageBuffUI;

    [SerializeField] private GameObject _StageBuffManager;

    [ContextMenu("SetUIList")]
    private void SetUIList()
    {
        _UIList.Clear();
        for (int i = 0; i < _uiParent.childCount; i++)
        {
            StageBuffUI _UIData = _uiParent.GetChild(i).GetComponent<StageBuffUI>();
            if (_UIData != null)
            {
                _UIList.Add(_UIData);
            }
        }
    }

    private void Awake()
    {
        Debug.LogWarning("StageBuffManagerCall\n ItemName:"+ gameObject.name);
        _stageBuffAll.Set_Initialize();
        StageBuffListSetUp();
        StageBuffSelectInitialize();
        _StageBuffManager.SetActive(false);
        BuffCheck();
    }

    private void StageBuffListSetUp()
    {
        m_StagebuffList.Clear();
        for(int i = 0; i < _stageBuffAll.Get_StageBuffCount; i++)
        {
            m_StagebuffList.Add(_stageBuffAll.Get_StageBuffsData(i));
        }
    }
    private void StageBuffSelectInitialize()
    {
        for (int i = 0; i < _UIList.Count; i++)
        {
            if (m_StagebuffList.Count < 1) return;
            int RondNum = UnityEngine.Random.Range(0, m_StagebuffList.Count);
            StageBuff SetBuff = m_StagebuffList[RondNum];
            _UIList[i].Set_StageBuff = SetBuff;
            RemoveList.Add(SetBuff);
            m_StagebuffList.Remove(SetBuff);
            _UIList[i].Set_StageBuff = SetBuff;
            _UIList[i].SetManager(this);
        }

    }

    public void OnSelect()
    {
        Select();
    }
    private void Select()
    {
        _StageBuffUI.SetActive(false);
        _StageBuffManager.SetActive(true);
        //_stageBuffAll.Set_StageBuff();
        SceneManager._instance.ChangeScene();
    }
    public void OnBuffCheck()
    {
        BuffCheck();
    }

    private void BuffCheck()
    {
        for (int i = 0; i < _UIList.Count; i++)
        {
            if (_UIList[i].Get_isSelect) continue;
            if (_UIList[i].Get_isSelect)
            {
                _UIList[i].Set_Intaractive = false;
            }
            else
            {
                _UIList[i].Set_Intaractive = true;
            }
        }
    }
    public void SelectBuff(StageBuffUI selectUI)
    {
        // 再選択解除
        if (_currentSelect == selectUI)
        {
            selectUI.SetSelect(false);
            _currentSelect = null;
            return;
        }

        // 前選択解除
        if (_currentSelect != null)
        {
            _currentSelect.SetSelect(false);
        }

        // 新選択
        _currentSelect = selectUI;
        _currentSelect.SetSelect(true);
    }
}


