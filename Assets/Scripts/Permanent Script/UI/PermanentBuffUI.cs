using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PermanentBuffUI : MonoBehaviour
{
    private PermanentBuff m_Buff;

    [SerializeField] private PermanentCredit m_Credit;
    [SerializeField,ReadOnly] private Button m_Button;

    [SerializeField]private bool m_isSelect;


    public Button Get_Button { get => m_Button; }

    public PermanentBuff Get_Buff { get => m_Buff; }

    private void SetUI()
    {

        Text SetLv = transform.transform.GetChild(1).GetComponent<Text>();
        Text SetName = transform.transform.GetChild(2).GetComponent<Text>();
        Text SetTips = transform.transform.GetChild(3).GetComponent<Text>();
        Text SetCost = transform.transform.GetChild(4).GetComponent<Text>();

        if (m_Buff == null)
            return;
        SetLv.text =  m_Buff.Get_BuffData.Get_GainLv.ToString();
        SetName.text = m_Buff.Get_BuffData.Get_BuffName.ToString();
        SetTips.text = m_Buff.Get_BuffData.Get_BuffTips.ToString();
        SetCost.text = "-" + m_Buff.Get_BuffData.Get_GainCost.ToString();
    }

    private void SetButton()
    {
        m_Button = GetComponent<Button>();
    }

    public PermanentBuff Set_Buff{ 
        set 
        { 
            if (m_Buff == value) return;
            m_Buff = value;
            SetUI();
            SetButton();
        } 
    }


    public bool Set_Intaractive
    {
        set
        {
            ColorBlock m_ButtonColors = GetComponent<Button>().colors;
            m_Button.interactable = value;
            if(!value)
            {
                transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.disabledColor;
            }
            else
            {
                if (!m_isSelect)
                {
                    transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.normalColor;
                }
            }
        }
    }

    public bool Get_isSelect { get => m_isSelect; }

    public void OnBuff()
    {
        m_isSelect = m_isSelect ? false : true;
        ColorBlock m_ButtonColors = GetComponent<Button>().colors;
        if (m_isSelect)
        {

            transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.selectedColor;
            
            m_Buff.Set_isUseData = true;
            m_Credit.Set_CreditUp =m_Buff.Get_BuffData.Get_GainCost;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.normalColor;
            m_Buff.Set_isUseData = false;
            m_Credit.Set_CreditDown = m_Buff.Get_BuffData.Get_GainCost;
        }
        m_Credit.Set_CreditTextModeBuff();
    }

    public void ReRollSetUp()
    {
        if (!m_isSelect)
        {
            return;
        }
        ColorBlock m_ButtonColors = GetComponent<Button>().colors;
        transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.normalColor;
        m_Buff.Set_isUseData = false;
        m_isSelect = false;
        m_Credit.Set_CreditDown = m_Buff.Get_BuffData.Get_GainCost;
    }
}
