using UnityEngine;

[System.Serializable]
public class PermanentDebuff
{
    [SerializeField] private bool m_isUseData;
    [SerializeField] private PermanentDebuffBase m_debuff;

    public bool Set_isUseData { set => m_isUseData = value; }

    public bool Get_isUseData { get => m_isUseData; }
    public PermanentDebuffBase Get_DebuffData { get => m_debuff; }

    public PermanentDebuff()
    {
        m_isUseData = false;
    }

}
