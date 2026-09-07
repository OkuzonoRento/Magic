using UnityEngine;

public class StageBuffBase : ScriptableObject
{
    
    [SerializeField] private string m_BuffName;
    [SerializeField] private string m_BuffTips;
    [SerializeField] private Sprite m_icon;
    [SerializeField] private int m_BuffValueInt;
    [SerializeField] private float m_BuffValueFloat;

    public int Set_BuffValueInt { set => m_BuffValueInt = value; }

    public Sprite Get_icon { get => m_icon; }
    public string Get_BuffName { get => m_BuffName; }

    public string Get_BuffTips { get => m_BuffTips; }

    public int Get_BuffValueInt { get => m_BuffValueInt; }

    public float Get_BuffValueFloat { get => m_BuffValueFloat; }

}
