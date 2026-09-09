using UnityEngine;

public class PermanentBuffBase : PermanentBuffDeBuffBase
{
    [SerializeField] private string m_BuffName;
    [SerializeField] private string m_BuffTips;
    [SerializeField] private int m_GainLv;
    [SerializeField] private int m_GainCost;
    [SerializeField] private int m_BuffValueInt;
    [SerializeField] private float m_BuffValueFloat;


    public string Get_BuffName { get => m_BuffName; }

    public string Get_BuffTips { get => m_BuffTips; }

    public int Get_GainLv { get => m_GainLv; }

    public int Get_GainCost {  get => m_GainCost; }

    public int Get_BuffValueInt { get => m_BuffValueInt; }

    public float Get_BuffValueFloat { get => m_BuffValueFloat; }
}
