using UnityEngine.EventSystems;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
public class StageBuffUI : MonoBehaviour
{
    private StageBuff m_Stagebuff;

    [SerializeField, ReadOnly] private Button m_Button;
    [SerializeField, ReadOnly] private bool m_isSelect;

    public Button Get_Button { get => m_Button; }

    public StageBuff Get_StageBuff { get => m_Stagebuff; }

    private void SetUI()
    {

        Text SetName = transform.transform.GetChild(1).GetComponent<Text>();
        Text SetTips = transform.transform.GetChild(2).GetComponent<Text>();

        if (m_Stagebuff == null)
            return;
        SetName.text = m_Stagebuff.Get_StageBuffData.Get_BuffName.ToString();
        SetTips.text = m_Stagebuff.Get_StageBuffData.Get_BuffTips.ToString();
    }
    private void SetButton()
    {
        m_Button = GetComponent<Button>();
    }

    public StageBuff Set_StageBuff
    {
        set
        {
            if (m_Stagebuff == value) return;
            m_Stagebuff = value;
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
            if (!value)
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

    /*    public void OnStageBuff()
        {
            m_isSelect = m_isSelect ? false : true;
            ColorBlock m_ButtonColors = GetComponent<Button>().colors;
            if (m_isSelect)
            {

                transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.selectedColor;

                m_Stagebuff.Set_isUseData = true;
            }
            else
            {
                transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.normalColor;
                m_Stagebuff.Set_isUseData = false;
            }
        }
    */
    private StageBuffUIManager _manager;

    public void SetManager(StageBuffUIManager manager)
    {
        _manager = manager;
    }

    public void OnStageBuff()
    {
        _manager.SelectBuff(this);
    }
    public void SetSelect(bool select)
    {
        m_isSelect = select;

        ColorBlock m_ButtonColors = GetComponent<Button>().colors;

        if (m_isSelect)
        {
            transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.selectedColor;
            m_Stagebuff.Set_isUseData = true;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().color = m_ButtonColors.normalColor;
            m_Stagebuff.Set_isUseData = false;
        }
    }
}
