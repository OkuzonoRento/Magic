using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PermanentPlayerDebuffAll", menuName = "Permanent/PermanentAll/PermanentPlayerDebuffAll")]


public class PermanentPlayerDebuffAll : PermanentBuffDeBuffBase
{

    [SerializeField] private List<PermanentDebuff> PlayerDebuffs;

    public void Set_Initialize()
    {
        for (int i = 0; i < PlayerDebuffs.Count; i++)
        {
            PlayerDebuffs[i].Set_isUseData = false;
        }
    }

    public int Get_ATK(int ATK)    
    {
        for(int i=0;i<  PlayerDebuffs.Count;i++)
        {
            if (PlayerDebuffs[i].Get_DebuffData.Get_DeBuffName == "DamageDown") //デバフ名
            {
                return PlayerDebuffs[i].Get_isUseData ? PlayerDebuffs[i].Get_DebuffData.Get_DeBuffValueInt : ATK;
            }
        }
        return ATK;
    }

    public float Get_SkillCT(float SkillCT)
    {
        for (int i = 0; i < PlayerDebuffs.Count; i++)
        {
            if (PlayerDebuffs[i].Get_DebuffData.Get_DeBuffName == "CoolTime") //デバフ名
            {
                return PlayerDebuffs[i].Get_isUseData ? SkillCT * PlayerDebuffs[i].Get_DebuffData.Get_DeBuffValueFloat : SkillCT;
            }
        }
        return SkillCT;
    }

    public int Get_HP(int HP)
    {
        for (int i = 0; i < PlayerDebuffs.Count; i++)
        {
            if (PlayerDebuffs[i].Get_DebuffData.Get_DeBuffName == "HP") //デバフ名
            {
                return PlayerDebuffs[i].Get_isUseData ? PlayerDebuffs[i].Get_DebuffData.Get_DeBuffValueInt : HP;
            }
        }
        return HP;
    }


    public PermanentDebuff Get_DeBuffsData(int value)
    {
        return PlayerDebuffs[value];
    }
    public int Get_DeBuffCount { get => PlayerDebuffs.Count; }

}