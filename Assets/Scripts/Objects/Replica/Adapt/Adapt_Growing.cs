using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_Growing : Adapt
{
    public override void Init()
    {
        base.Init();
        mValue[0] = 1f;
        mValue[1] = 1.25f;
        mValue[2] = 1.5f;
        mValue[3] = 2f;
        MaxTier = 3;
        mEnumPart = EnumParts.Core;
        mEnumAdapt = EnumAdapts.Growing;
        mName = "성장기";
        mInformation = "열매 경험치\n추가 획득";
    }

    public float PlusExp(float _Exp)
    {
        return _Exp * mValue[mTier];
    }

    protected override void MaxTierFunc()
    {
        
    }
}
