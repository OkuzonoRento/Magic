using UnityEngine;

[System.Serializable]
public class PermanentBuff
{
    [SerializeField] private bool m_isUseData;
    [SerializeField] private PermanentBuffBase m_buff;

    public bool Set_isUseData { set => m_isUseData = value; }

    public bool Get_isUseData { get => m_isUseData; }
    public PermanentBuffBase Get_BuffData { get => m_buff; }

    public PermanentBuff()
    {
        m_isUseData = false;
    }
}
