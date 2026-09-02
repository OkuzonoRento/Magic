using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PermanentDebuffUI : MonoBehaviour
{
    private PermanentDebuff m_Debuff;

    [SerializeField] private PermanentCredit m_Credit;
    [SerializeField,ReadOnly]private bool m_isSelect;

    private void SetUI()
    {
        Text SetLv = transform.transform.GetChild(1).GetComponent<Text>();
        Text SetName = transform.transform.GetChild(2).GetComponent<Text>();
        Text SetTips = transform.transform.GetChild(3).GetComponent<Text>();
        Text SetCost = transform.transform.GetChild(4).GetComponent<Text>();

        if (m_Debuff == null)
            return;

        SetLv.text =  m_Debuff.Get_DebuffData.Get_DangerLv.ToString();
        SetName.text = m_Debuff.Get_DebuffData.Get_DeBuffName.ToString();
        SetTips.text = m_Debuff.Get_DebuffData.Get_DeBuffTips.ToString();
        SetCost.text = "+" + m_Debuff.Get_DebuffData.Get_DangerCost.ToString();
    }


    public PermanentDebuff Set_Debuff{ 
        set 
        { 
            if (m_Debuff == value) return;
            m_Debuff = value;
            SetUI();
        } 
    }

   

    public void ReRollSetUp()
    {
        if (!m_isSelect)
        {
            return;
        }
        ColorBlock m_ButtonColors = GetComponent<Button>().colors;
        transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.normalColor;
        m_Debuff.Set_isUseData = false;
        m_isSelect = false;
        m_Credit.Set_CreditDown = m_Debuff.Get_DebuffData.Get_DangerCost;
    }

    public void OnDeBuff()
    {
      m_isSelect = m_isSelect ? false : true;
        ColorBlock m_ButtonColors = GetComponent<Button>().colors;
        if (m_isSelect)
        {
            
            transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.selectedColor;
            m_Debuff.Set_isUseData = true;
            m_Credit.Set_CreditUp =m_Debuff.Get_DebuffData.Get_DangerCost;
        }

        else
        {
            transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.normalColor;
            m_Debuff.Set_isUseData = false;
            m_Credit.Set_CreditDown = m_Debuff.Get_DebuffData.Get_DangerCost;
        }
        m_Credit.Set_CreditTextModeDebuff();
    }

  

}
