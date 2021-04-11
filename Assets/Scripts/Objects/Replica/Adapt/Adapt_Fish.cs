using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_Fish : Adapt
{
    public override void Init()
    {
        Tier = 0;
        MaxTier = 1;
        mEnumPart = EnumParts.Slime;
        mEnumAdapt = EnumAdapts.Fish;
        mName = "첨벙첨벙";
        mInformation = "홍수의 영향을\n받지 않음";
    }
    public bool CheckTier()
    {
        return Tier >= 1;
    }

    protected override void MaxTierFunc()
    {
        
    }
}
