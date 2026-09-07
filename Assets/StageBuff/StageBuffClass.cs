using UnityEngine;


[System.Serializable]
public class StageBuff
{
    [SerializeField] private bool m_isUseData;
    [SerializeField] private StageBuffBase m_Stagebuff;

    public bool Set_isUseData { set => m_isUseData = value; }

    public bool Get_isUseData { get => m_isUseData; }
    public StageBuffBase Get_StageBuffData { get => m_Stagebuff; }

    public StageBuff()
    {
        m_isUseData = false;
    }
}
