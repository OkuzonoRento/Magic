using UnityEngine;

public class PermanentDebuffBase : PermanentBuffDeBuffBase
{
    [SerializeField] private string m_DeBuffName;
    [SerializeField] private string m_DeBuffTips;
    [SerializeField] private int m_DangerLv;
    [SerializeField] private int m_DangerCost;
    [SerializeField] private int m_DeBuffValueInt;
    [SerializeField] private float m_DeBuffValueFloat;

    public string Get_DeBuffName { get => m_DeBuffName; }

    public string Get_DeBuffTips {  get => m_DeBuffTips; }

    public int Get_DangerLv { get => m_DangerLv; }

    public int Get_DangerCost { get => m_DangerCost; }

    public int Get_DeBuffValueInt { get => m_DeBuffValueInt; }

    public float Get_DeBuffValueFloat { get => m_DeBuffValueFloat; }


}
