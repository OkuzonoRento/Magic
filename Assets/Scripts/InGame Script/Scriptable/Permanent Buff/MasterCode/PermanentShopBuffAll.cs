using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PermanentShopBuffAll", menuName = "Permanent/PermanentAll/PermanentShopBuffAll")]
public class PermanentShopBuffAll : PermanentBuffDeBuffBase
{

    [SerializeField] private List<PermanentBuff> ShopBuffs;

    public void Set_Initialize()
    {
        for (int i = 0; i < ShopBuffs.Count; i++)
        {
            ShopBuffs[i].Set_isUseData = false;
        }
    }
    public int Get_CreditRem(int Credit)
    {
        for(int i =0;i <ShopBuffs.Count;i++)
        {
            if (ShopBuffs[i].Get_BuffData.Get_BuffName =="CreditRem")
            {
                return ShopBuffs[i].Get_isUseData ? Credit * ShopBuffs[i].Get_BuffData.Get_BuffValueInt /100 : Credit;
            }
        }
        return Credit;
    }

    public int Get_PurchaseNoneCreditRate(int NoneCreditRate)
    {
        for (int i = 0; i < ShopBuffs.Count; i++)
        {
            if (ShopBuffs[i].Get_BuffData.Get_BuffName == "Purchase")
            {
                return ShopBuffs[i].Get_isUseData ? ShopBuffs[i].Get_BuffData.Get_BuffValueInt: NoneCreditRate;
            }
        }
        return NoneCreditRate;
    }

    public int Get_ReRollNoneCost()
    {
        for (int i = 0; i < ShopBuffs.Count; i++)
        {
            if (ShopBuffs[i].Get_BuffData.Get_BuffName == "ReRollCost")
            {
                return ShopBuffs[i].Get_isUseData ? ShopBuffs[i].Get_BuffData.Get_BuffValueInt : 0;
            }
        }
        return 0;
    }

    public int Get_SkillLvUpBonus()
    {
        for (int i = 0; i < ShopBuffs.Count; i++)
        {
            if (ShopBuffs[i].Get_BuffData.Get_BuffName == "SkillLv")
            {
                return ShopBuffs[i].Get_isUseData ? ShopBuffs[i].Get_BuffData.Get_BuffValueInt : 0;
            }
        }
        return 0;
    }

    public PermanentBuff Get_BuffsData(int value)
    {
        return ShopBuffs[value];
    }

    public int Get_BuffCount { get => ShopBuffs.Count; }

}
