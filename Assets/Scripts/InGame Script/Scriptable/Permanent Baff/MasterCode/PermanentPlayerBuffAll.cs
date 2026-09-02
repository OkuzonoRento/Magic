using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "PermanentPlayerBuffAll", menuName = "Permanent/PermanentAll/PermanentPlayerBuffAll")]
public class PermanentPlayerBuffAll : PermanentBuffDeBuffBase
{

    [SerializeField]private List<PermanentBuff> PlayerBuffs;

    public void Set_Initialize()
    {
        for (int i = 0; i < PlayerBuffs.Count; i++)
        {
            PlayerBuffs[i].Set_isUseData = false;
        }
    }


    public float Get_AttackSpan(float AttackSpan)
    {
        for (int i = 0; i < PlayerBuffs.Count; i++)
        {
            if (PlayerBuffs[i].Get_BuffData.Get_BuffName == "CTData")
            {
                return PlayerBuffs[i].Get_isUseData ? AttackSpan * PlayerBuffs[i].Get_BuffData.Get_BuffValueFloat : AttackSpan;
            }
        }
        return AttackSpan;
    }

    public PermanentBuff Get_BuffsData(int value)
    {
        return PlayerBuffs[value];
    }

    public int Get_BuffCount { get=>  PlayerBuffs.Count;}
}
