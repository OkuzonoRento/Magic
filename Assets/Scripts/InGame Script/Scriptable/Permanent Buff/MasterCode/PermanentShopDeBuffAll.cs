using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PermanentShopDeBuffAll", menuName = "Permanent/PermanentAll/PermanentShopDeBuffAll")]
public class PermanentShopDeBuffAll : PermanentBuffDeBuffBase
{
    [SerializeField] private List<PermanentDebuff> ShopDebuffs;


    public void Set_Initialize()
    {
        for(int i= 0;i < ShopDebuffs.Count;i++)
        {
            ShopDebuffs[i].Set_isUseData = false;
        }
    }


    public int Get_CreditCost(int Cost)
    {   
        for (int i = 0; i < ShopDebuffs.Count; i++)
        {
            if (ShopDebuffs[i].Get_DebuffData.Get_DeBuffName == "CreditCost") //デバフ名
            {
                return ShopDebuffs[i].Get_isUseData ? Cost * ShopDebuffs[i].Get_DebuffData.Get_DeBuffValueInt /100 : Cost;
            }
        }
        return Cost;
    }

    public int Get_CreditRem(int Credit)
    {
        for (int i = 0; i < ShopDebuffs.Count; i++)
        {
            if (ShopDebuffs[i].Get_DebuffData.Get_DeBuffName == "CreditRem") //デバフ名
            {
                return ShopDebuffs[i].Get_isUseData ? Credit * ShopDebuffs[i].Get_DebuffData.Get_DeBuffValueInt / 100 : Credit;
            }
        }
        return Credit;
    }

    public int Get_ReRollCost(int Cost)
    {
        for (int i = 0; i < ShopDebuffs.Count; i++)
        {
            if (ShopDebuffs[i].Get_DebuffData.Get_DeBuffName == "ReRollCost") //デバフ名
            {
                return ShopDebuffs[i].Get_isUseData ? Cost * ShopDebuffs[i].Get_DebuffData.Get_DeBuffValueInt / 100 : Cost;
            }
        }
        return Cost;
    }

    public int Get_SkillUp(int LvUpFileRate)
    {
        for (int i = 0; i < ShopDebuffs.Count; i++)
        {
            if (ShopDebuffs[i].Get_DebuffData.Get_DeBuffName == "ReRollCost") //デバフ名
            {
                return ShopDebuffs[i].Get_isUseData ? ShopDebuffs[i].Get_DebuffData.Get_DeBuffValueInt: LvUpFileRate;
            }
        }
        return LvUpFileRate;
    }

    public int Get_UnGet(int UnGetRate)
    {
        for(int i = 0;i < ShopDebuffs.Count;i++)
        {
            if (ShopDebuffs[i].Get_DebuffData.Get_DeBuffName == "UnGet")
            {
                return ShopDebuffs[i].Get_isUseData ? ShopDebuffs[i].Get_DebuffData.Get_DeBuffValueInt : UnGetRate;
            }
        }
        return UnGetRate;
    }


    public PermanentDebuff Get_DeBuffsData(int value)
    {
        return ShopDebuffs[value];
    }

    public int Get_DeBuffCount { get => ShopDebuffs.Count; }

}
