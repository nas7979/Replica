using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_Appetite : Adapt
{
    public override void Init()
    {
        base.Init();
        mValue[0] = 1;
        mValue[1] = 2;
        mValue[2] = 3;
        mValue[3] = 4;
        MaxTier = 3;
        mEnumAdapt = EnumAdapts.Appetite;
        mEnumPart = EnumParts.Slime;
        mName = "식탐";
        mInformation = "한 번에 여러 개까지 먹이를 획득";
    }

    protected override void MaxTierFunc()
    {
        
    }
}
