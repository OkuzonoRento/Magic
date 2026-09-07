using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "StageBuffAll", menuName = "StageBuff/StageBuffAll")]
public class StageBuffAll : StageBuffBase
{
    [SerializeField] private List<StageBuff> StageBuffs;
    [SerializeField, ReadOnly] private List<StageBuff> sortStageBuffs = new List<StageBuff>();

    [ContextMenu("SetSortStageBuffs")]

    private void SetSortStageBuffs()
    {
        sortStageBuffs.Clear();

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "AGI_UP")
            {
                sortStageBuffs.Add(StageBuffs[i]);
            }
        }

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "HP_UP")
            {
                 sortStageBuffs.Add(StageBuffs[i]);
            }
        }

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "ATKCoolTime")
            {
                sortStageBuffs.Add(StageBuffs[i]);
            }
        }

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "shield")
            {
                sortStageBuffs.Add(StageBuffs[i]);
            }
        }

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "drain")
            {
                sortStageBuffs.Add(StageBuffs[i]);
            }
        }

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "revenge")
            {
                sortStageBuffs.Add(StageBuffs[i]);
            }
        }

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "‹¶—³Ç")
            {
                sortStageBuffs.Add(StageBuffs[i]);
            }
        }

        for (int i = 0; i < StageBuffs.Count; i++)
        {
            if (StageBuffs[i].Get_StageBuffData.Get_BuffName == "ƒlƒR‚Ì¶–½•ÛŒ¯")
            {
                 sortStageBuffs.Add(StageBuffs[i]);
            }
        }

    }

    public void Set_Initialize()
    {
        SetSortStageBuffs();
        for (int i = 0; i < StageBuffs.Count; i++)
        {
            StageBuffs[i].Set_isUseData = false;
        }
    }

    //public float Get_AGIUP(float value)
    //{
    //    Debug.Log($"sortStageBuffs.Count = {sortStageBuffs.Count}");

    //    return sortStageBuffs[0].Get_isUseData
    //        ? value * sortStageBuffs[0].Get_StageBuffData.Get_BuffValueFloat
    //        : value;
    //}
    public float Get_AGIUP(float value)
    {
        return sortStageBuffs[0].Get_isUseData ? value * sortStageBuffs[0].Get_StageBuffData.Get_BuffValueFloat : value;
    }
    public int Get_HPUP(int value)
    {
        return sortStageBuffs[1].Get_isUseData ? value + sortStageBuffs[1].Get_StageBuffData.Get_BuffValueInt : value;
    }
    public float Get_ATKCoolTime(float value)
    {
        Debug.LogWarning(value);
        Debug.LogWarning(sortStageBuffs[2].Get_isUseData ? value * sortStageBuffs[2].Get_StageBuffData.Get_BuffValueFloat : value);
        return sortStageBuffs[2].Get_isUseData ? value * sortStageBuffs[2].Get_StageBuffData.Get_BuffValueFloat : value;
    }
    public bool Get_Sheild()
    {
       if(!sortStageBuffs[3].Get_isUseData) return false;
        sortStageBuffs[3].Get_StageBuffData.Set_BuffValueInt = sortStageBuffs[3].Get_StageBuffData.Get_BuffValueInt -1;

        return sortStageBuffs[3].Get_StageBuffData.Get_BuffValueInt > 0 ? true : false;
    }
    public int Get_Revenge(int value)//‚ê‚ñ‚Æ‚­‚ñŠæ’£‚Á‚Ä‚Ë
    {
        return sortStageBuffs[4].Get_isUseData ? 0 : value;
    }

    public float Get_Drain(float value)
    {
        return sortStageBuffs[5].Get_isUseData ? value * sortStageBuffs[5].Get_StageBuffData.Get_BuffValueFloat : value;
    }

    public float Get_FullPower(float value)
    {
        return sortStageBuffs[6].Get_isUseData ? value * sortStageBuffs[6].Get_StageBuffData.Get_BuffValueFloat : value;
    }
    public int Get_FullPowerHP(int value)
    {
        return sortStageBuffs[6].Get_isUseData ? value - sortStageBuffs[6].Get_StageBuffData.Get_BuffValueInt : value;
    }

    public float Get_Revive(int value, int MaxValue)
    {
        if (!sortStageBuffs[7].Get_isUseData) return value;
        sortStageBuffs[7].Get_StageBuffData.Set_BuffValueInt = sortStageBuffs[7].Get_StageBuffData.Get_BuffValueInt - 1;
        return sortStageBuffs[7].Get_StageBuffData.Get_BuffValueInt > 0 ? MaxValue * 5/100 : value;
    }
    public StageBuff Get_StageBuffsData(int value)
    {
        return StageBuffs[value];
    }

    public int Get_StageBuffCount { get => StageBuffs.Count; }

}