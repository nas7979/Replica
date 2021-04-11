using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_River : Adapt
{
    public override void Init()
	{
		base.Init();
		mValue[0] = 0f;
		mValue[1] = 0.25f;
		mValue[2] = 0.5f;
		mValue[3] = 1f;
        MaxTier = 3;
        mEnumPart = EnumParts.Parts;
		mEnumAdapt = EnumAdapts.River;
        mName = "강의 축복";
        mInformation = "홍수 발생 시, 포식자 처치";
    }

    protected override void MaxTierFunc()
    {
        
    }
}
