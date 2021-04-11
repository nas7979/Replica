using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapt_Threat : Adapt
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
		mEnumAdapt = EnumAdapts.Threat;
        mName = "위협";
        mInformation = "부딪힌 포식자의 레벨을 낮춤";
    }

    protected override void MaxTierFunc()
    {
        
    }

}
