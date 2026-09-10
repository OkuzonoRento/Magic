using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PermanentDebuffManager: MonoBehaviour
{
    [SerializeField, Header("UIの親オブジェクト")] private Transform _uiParent;
    [SerializeField,ReadOnly] private List<PermanentDebuffUI> _UIList;
    [SerializeField,ReadOnly] private List<PermanentDebuff> m_debuffList;
    [SerializeField,ReadOnly] private List<PermanentDebuff> RemoveList;

    [SerializeField] private PermanentPlayerDebuffAll _PlayerDeBuffAll;
    [SerializeField] private PermanentShopDeBuffAll _ShopDeBuffAll;

    [SerializeField] private GameObject _DebuffUI;
    [SerializeField] private GameObject _BuffUI;

    [SerializeField] private GameObject _BuffManager;


    private int m_reRollCount;

    [SerializeField] private int m_reRollCountMax;

    [SerializeField] private Button m_reRollButton;

    [Header("PermanentCredit")]
    [SerializeField] private PermanentCredit m_Credit;
    [SerializeField] private PermanentCredit _CreditMaster;
    [SerializeField] private Text _SelectUI;

    [ContextMenu("SetUIList")]
    private void SetUIList()
    {
        _UIList.Clear();
        for (int i = 0; i < _uiParent.childCount; i++)
        {
            PermanentDebuffUI _UIData = _uiParent.GetChild(i).GetComponent<PermanentDebuffUI>();
            if (_UIData != null)
            {
                _UIList.Add(_UIData);
            }
        }
    }

    private void OnEnable()
    {
        _PlayerDeBuffAll.Set_Initialize();
        _ShopDeBuffAll.Set_Initialize();
        DebuffListSetUp();
        DebuffSelectInitialize();
        m_Credit.Set_Initialize();
        _CreditMaster.Set_Initialize();
        m_Credit.Set_PermanentText = _SelectUI;
        _BuffManager.SetActive(false);
    }

    

    private void DebuffListSetUp()
    {

        for (int i = 0; i < _PlayerDeBuffAll.Get_DeBuffCount; i++)
        {
            m_debuffList.Add(_PlayerDeBuffAll.Get_DeBuffsData(i));
            

        }
        for (int i = 0; i < _ShopDeBuffAll.Get_DeBuffCount; i++)
        {
            m_debuffList.Add(_ShopDeBuffAll.Get_DeBuffsData(i));
        }
    }

    private void DebuffSelectInitialize()
    {
        for(int i =0;i<_UIList.Count;i++)
        {
        
            int RondNum = UnityEngine.Random.Range(0,_UIList.Count);
            PermanentDebuff SetDebuff = m_debuffList[RondNum];
            _UIList[i].Set_Debuff =SetDebuff;
            RemoveList.Add(SetDebuff);
            m_debuffList.Remove(SetDebuff);
        }
    }

    public void OnReroll()
    {
        Reroll();
    }

    private void Reroll()
    {
        for(int i =0;i < _UIList.Count;i++)
        {
            _UIList[i].ReRollSetUp();
        }

        for (int i = 0; i < RemoveList.Count; i++)
        {
            m_debuffList.Add(RemoveList[0]);
            RemoveList.Remove(RemoveList[0]);
        }
        DebuffSelectInitialize();
        m_reRollCount++;
        if(m_reRollCount >= m_reRollCountMax)
        {
            RerollButtonOff();
        }
        _SelectUI.text = "+" + m_Credit.Get_PermanentCredit.ToString();
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
        _SelectUI.text = "+" + m_Credit.Get_PermanentCredit.ToString();
        _CreditMaster.Set_CreditUp = m_Credit.Get_PermanentCredit;
        _DebuffUI.SetActive(false);
        _BuffManager.SetActive(true);
        _BuffUI.SetActive(true);
        
    }
    
  
    

    

   


}
