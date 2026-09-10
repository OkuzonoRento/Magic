using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PermanentBuffManager : MonoBehaviour
{
    [SerializeField, Header("UIの親オブジェクト")] private Transform _uiParent;
    [SerializeField, ReadOnly] private List<PermanentBuffUI> _UIList;
    [SerializeField, Unity.Collections.ReadOnly] private List<PermanentBuff> m_buffList;
    [SerializeField, Unity.Collections.ReadOnly] private List<PermanentBuff> RemoveList;

    [SerializeField] private PermanentPlayerBuffAll _PlayerBuffAll;
    [SerializeField] private PermanentShopBuffAll _ShopBuffAll;

    [SerializeField] private GameObject _BuffUI;

    private int m_reRollCount;

    [SerializeField] private int m_reRollCountMax;

    [SerializeField] private Button m_reRollButton;

    [Header("PermanentCredit")]
    [SerializeField] private PermanentCredit m_Credit;
    [SerializeField] private PermanentCredit _CreditMaster;
    [SerializeField] private Text _SelectUI;

    private SceneManager _sceneManager;


    [ContextMenu("SetUIList")]
    private void SetUIList()
    {
        _UIList.Clear();
        for (int i = 0; i < _uiParent.childCount; i++)
        {
            PermanentBuffUI _UIData = _uiParent.GetChild(i).GetComponent<PermanentBuffUI>();
            if (_UIData != null)
            {
                _UIList.Add(_UIData);
            }
        }
    }


    private void OnEnable()
    {
        _PlayerBuffAll.Set_Initialize();
        _ShopBuffAll.Set_Initialize();
        DebuffListSetUp();
        DebuffSelectInitialize();
        m_Credit.Set_Initialize();
        m_Credit.Set_PermanentText = _SelectUI;
        BuffChack();
        _sceneManager = SceneManager._instance;
    }



    private void DebuffListSetUp()
    {
        m_buffList.Clear();
        for (int i = 0; i < _PlayerBuffAll.Get_BuffCount; i++)
        {
            m_buffList.Add(_PlayerBuffAll.Get_BuffsData(i));


        }
        for (int i = 0; i < _ShopBuffAll.Get_BuffCount; i++)
        {
            m_buffList.Add(_ShopBuffAll.Get_BuffsData(i));

        }
    }

    private void DebuffSelectInitialize()
    {
        for (int i = 0; i < _UIList.Count; i++)
        {
            if (m_buffList.Count < 1) return;
            int RondNum = UnityEngine.Random.Range(0, m_buffList.Count);
            PermanentBuff SetBuff = m_buffList[RondNum];
            _UIList[i].Set_Buff = SetBuff;
            RemoveList.Add(SetBuff);
            m_buffList.Remove(SetBuff);
        }
    }

    public void OnReroll()
    {
        Reroll();
    }

    private void Reroll()
    {
        for (int i = 0; i < _UIList.Count; i++)
        {
            _UIList[i].ReRollSetUp();
        }
        for (int i = 0; i < RemoveList.Count; i++)
        {
            m_buffList.Add(RemoveList[0]);
            RemoveList.Remove(RemoveList[0]);
        }
        DebuffSelectInitialize();
        m_reRollCount++;
        if (m_reRollCount >= m_reRollCountMax)
        {
            RerollButtonOff();
        }
    }

    private void RerollButtonOff()
    {
        m_reRollButton.interactable = false;
    }

    public void OnSelect()
    {
        Select();
    }

    private void Select()
    {
        _SelectUI.text = m_Credit.Get_PermanentCredit.ToString();
        _CreditMaster.Set_CreditDown = m_Credit.Get_PermanentCredit;
        _BuffUI.SetActive(false);
        _sceneManager.ChangeScene();
    }


    public void OnBuffChack()
    {
        BuffChack();
    }

    private void BuffChack()
    {
        for (int i = 0; i < _UIList.Count; i++)
        {
            if (_UIList[i].Get_isSelect) continue;
            int gainCost = m_Credit.Get_PermanentCredit + _UIList[i].Get_Buff.Get_BuffData.Get_GainCost;
            Debug.Log(gainCost);
            if (_CreditMaster.Get_PermanentCredit - gainCost < 0)
            {
                _UIList[i].Set_Intaractive = false;
            }
            else
            {
                _UIList[i].Set_Intaractive = true;
            }
        }
    }








}
